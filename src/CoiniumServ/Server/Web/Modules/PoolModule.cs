#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System;
using CoiniumServ.Accounts;
using CoiniumServ.Persistance.Query;
using CoiniumServ.Pools;
using CoiniumServ.Server.Web.Models;
using CoiniumServ.Server.Web.Models.Pool;
using Nancy;
using Nancy.CustomErrors;
using Nancy.Helpers;

namespace CoiniumServ.Server.Web.Modules
{
    public class PoolModule : NancyModule
    {
        public PoolModule(IPoolManager poolManager)
            : base("/pool")
        {
            Get["/{slug}"] = _ =>
            {
                var pool = poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                ViewBag.Title = string.Format("{0} Pool", pool.Config.Coin.Name);
                ViewBag.Heading = string.Format("{0} Pool", pool.Config.Coin.Name);

                // return our view
                return View["pool", new PoolModel
                {
                    Pool = pool
                }];
            };

            Get["/{slug}/workers"] = _ =>
            {
                var pool = poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.                

                if (pool == null) // make sure queried pool exists.
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                ViewBag.Header = string.Format("{0} Workers", pool.Config.Coin.Name);

                // return our view
                return View["workers", new WorkersModel
                {
                    Workers = pool.MinerManager.Miners
                }];
            };

            Get["/{slug}/round"] = _ =>
            {
                var pool = poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null) // make sure queried pool exists.
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                ViewBag.Header = string.Format("{0} Current Round", pool.Config.Coin.Name);

                // return our view
                return View["round", new RoundModel
                {
                    Round = pool.NetworkInfo.Round,
                    Shares = pool.RoundShares
                }];
            };

            Get["/{slug}/blocks/{page?1}"] = _ =>
            {
                var pool = (IPool)poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null) // make sure queried pool exists.
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                int page;
                if (!Int32.TryParse(_.page, out page))
                    page = 1;

                var paginationQuery = new PaginationQuery(page);

                var blocks = pool.BlockRepository.GetBlocks(paginationQuery);

                if (blocks.Count == 0)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = "No more blocks exist"
                    }];
                }

                var model = new BlocksModel
                {
                    Blocks = blocks,
                    Coin = pool.Config.Coin,
                    Filter = BlockFilter.All,
                    PaginationQuery = paginationQuery
                };

                return View["blocks", model];
            };

            Get["/{slug}/blocks/paid/{page?1}"] = _ =>
            {
                var pool = (IPool)poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null) // make sure queried pool exists.
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                int page;
                if (!Int32.TryParse(_.page, out page))
                    page = 1;

                var paginationQuery = new PaginationQuery(page);

                var blocks = pool.BlockRepository.GetPaidBlocks(paginationQuery);

                if (blocks.Count == 0)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = "No more blocks exist"
                    }];
                }

                var model = new BlocksModel
                {
                    Blocks = blocks,
                    Coin = pool.Config.Coin,
                    Filter = BlockFilter.PaidOnly,
                    PaginationQuery = paginationQuery
                };

                return View["blocks", model];
            };

            Get["/{slug}/block/{height:int}"] = _ =>
            {
                var pool = (IPool)poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null) // make sure queried pool exists.
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                var block = pool.BlockRepository.Get((uint)_.height);

                if (block == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested block does not exist: {0}", _.height)
                    }];
                }

                var model = new BlockModel
                {
                    Block = block,
                    Coin = pool.Config.Coin,
                    Payments = pool.PaymentRepository.GetPaymentDetailsForBlock((uint)_.height)
                };

                ViewBag.Header = string.Format("Block {0}", block.Height);
                ViewBag.SubHeader = string.Format("{0} block", pool.Config.Coin.Name);

                return View["block", model];
            };

            Get["/{slug}/tx/{id:int}"] = _ =>
            {
                var pool = (IPool)poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null) // make sure queried pool exists.
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                var details = pool.PaymentRepository.GetPaymentDetailsByTransactionId((uint)_.id);

                if (details == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested transaction does not exist.")
                    }];
                }

                var account = pool.AccountManager.GetAccountById(details.AccountId);
                var block = pool.BlockRepository.Get((uint) details.Block);

                ViewBag.Header = string.Format("Transaction Details");
                ViewBag.SubHeader = string.Format("{0}", details.TransactionId);

                var model = new PaymentDetailsModel
                {
                    Details = details,
                    Account = account,
                    Block = block,
                    Coin = pool.Config.Coin
                };

                return View["paymentdetails", model];
            };

            Get["/{slug}/payment/{id:int}"] = _ =>
            {
                var pool = (IPool)poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null) // make sure queried pool exists.
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", _.slug)
                    }];
                }

                var details = pool.PaymentRepository.GeyPaymentDetailsByPaymentId((uint)_.id);

                if (details == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested payment does not exist.")
                    }];
                }

                var account = pool.AccountManager.GetAccountById(details.AccountId);
                var block = pool.BlockRepository.Get((uint)details.Block);

                ViewBag.Header = string.Format("Payment Details");
                ViewBag.SubHeader = string.Format("{0}", details.PaymentId);

                var model = new PaymentDetailsModel
                {
                    Details = details,
                    Account = account,
                    Block = block,
                    Coin = pool.Config.Coin
                };

                return View["paymentdetails", model];
            };

            Get["/{slug}/account/address/{address:length(26,34)}/{page?1}"] = _ =>
            {
                var pool = (IPool)poolManager.Get(HttpUtility.HtmlEncode(_.slug)); // find the requested pool.

                if (pool == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested pool does not exist: {0}", HttpUtility.HtmlEncode(_.slug))
                    }];
                }

                var account = (IAccount)pool.AccountManager.GetAccountByAddress(_.address);

                if (account == null)
                {
                    return View["error", new ErrorViewModel
                    {
                        Details = string.Format("The requested account does not exist: {0}", _.address)
                    }];
                }

                int page;
                if (!Int32.TryParse(_.page, out page))
                    page = 1;

                var paginationQuery = new PaginationQuery(page);

                // get the payments for the account.
                var payments = pool.AccountManager.GetPaymentsForAccount(account.Id, paginationQuery);

                ViewBag.Header = string.Format("Account Details");
                ViewBag.SubHeader = account.Username;

                // return our view
                return View["account", new AccountModel
                {
                    Account = account,
                    Coin = pool.Config.Coin,
                    Payments = payments,
                    PaginationQuery = paginationQuery
                }];
            };
        }
    }
}
