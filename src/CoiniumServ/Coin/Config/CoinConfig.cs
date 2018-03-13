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
using Serilog;

namespace CoiniumServ.Coin.Config
{
    public class CoinConfig : ICoinConfig
    {
        public bool Valid { get; private set; }
        
        public bool RpcUpdate { get; private set; }

        public string Name { get; private set; }

        public string Symbol { get; private set; }

        public string Algorithm { get; private set; }

        public string Site { get; private set; }

        public ICoinOptions Options { get; private set; }

        public IBlockExplorerOptions BlockExplorer { get; private set; }

        public dynamic Extra { get; private set; }

        public CoinConfig(dynamic config)
        {
            try
            {
                // set the coin data.
                Name = config.name;
                Symbol = config.symbol;
                Algorithm = config.algorithm;
                Site = string.IsNullOrEmpty(config.site) ? string.Empty : config.site;
                RpcUpdate = config.rpcupdate;
                Options = new CoinOptions(config.options);
                BlockExplorer = new BlockExplorerOptions(config.blockExplorer);
                Extra = config.extra;

                if (Name == null || Symbol == null || Algorithm == null) // make sure we have valid name, symbol and algorithm data.
                    Valid = false;
                else
                    Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<CoinConfig>().Error("Error loading coin configuration: {0:l}", e.Message);
            }
        }
    }
}
