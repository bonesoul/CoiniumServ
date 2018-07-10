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
using CoiniumServ.Algorithms;
using CoiniumServ.Daemon;
using CoiniumServ.Daemon.Errors;
using CoiniumServ.Daemon.Exceptions;
using CoiniumServ.Utils.Helpers;
using Serilog;

namespace CoiniumServ.Pools
{
    public class NetworkInfo : INetworkInfo
    {
        public double Difficulty { get; private set; }

        public int Round { get; private set; }

        public double Hashrate { get; private set; }

        public UInt64 Reward { get; private set; }

        public string CoinVersion { get; private set; }

        public int ProtocolVersion { get; private set; }

        public int WalletVersion { get; private set; }

        public bool Testnet { get; private set; }

        public long Connections { get; private set; }

        public string Errors { get; private set; }

        public bool Healthy { get; private set; }

        public string ServiceResponse { get; private set; } // todo implement this too for /pool/COIN/network

        private readonly IDaemonClient _daemonClient;

        private readonly IHashAlgorithm _hashAlgorithm;

        private readonly IPoolConfig _poolConfig;

        private readonly ILogger _logger;

        public NetworkInfo(IDaemonClient daemonClient, IHashAlgorithm hashAlgorithm, IPoolConfig poolConfig)
        {
            _daemonClient = daemonClient;
            _hashAlgorithm = hashAlgorithm;
            _poolConfig = poolConfig;
            _logger = Log.ForContext<NetworkInfo>().ForContext("Component", poolConfig.Coin.Name);

            DetectProofOfStakeCoin(); // detect if we are running on a proof-of-stake coin.
            DetectSubmitBlockSupport(); // detect if the coin daemon supports submitblock call.
            Recache(); // recache the data initially.
            PrintNetworkInfo(); // print the collected network info.
        }

        public void Recache()
        {

            try // read getnetworkinfo() followed by getwalletinfo() based data.
            {
                var info = _daemonClient.GetNetworkInfo();

                // read Getnetwork
                CoinVersion = info.Version;
                ProtocolVersion = info.ProtocolVersion;
                Connections = info.Connections;
                Errors = info.Errors;

                try // read getwalletinfo() based data.
                {
                    var infoWall = _daemonClient.GetWalletInfo();

                    // read data
                    WalletVersion = infoWall.WalletVersion;
                }
                catch (RpcException e)
                {
                    _logger.Error("Can not read getwalletinfo(): {0:l}", e.Message);
                    Healthy = false; // set healthy status to false as we couldn't get a reply.
                }

                // check if our network connection is healthy. info: based errors are warnings only so ignore.
                Healthy = Connections >= 0 && (string.IsNullOrEmpty(Errors) || Errors.Contains("Info:"));

            }
            catch (RpcException) // catch exception, provide backwards compatability for getinfo() based data.
            {
                // do not log this as an actual error, but rather as info only, then proceed to try getinfo().
                //_logger.Error("Can not read getnetworkinfo(), trying getinfo() instead: {0:l}", c.Message); // do not log original error, try getinfo() first.   

                try // catch exception, provide backwards compatability for getinfo() based data.
                {
                    var info = _daemonClient.GetInfo();

                    // read data.
                    CoinVersion = info.Version;
                    ProtocolVersion = info.ProtocolVersion;
                    WalletVersion = info.WalletVersion;
                    Testnet = info.Testnet;
                    Connections = info.Connections;
                    Errors = info.Errors;

                    // check if our network connection is healthy. info: based errors are warnings only so ignore.
                    Healthy = Connections >= 0 && (string.IsNullOrEmpty(Errors) || Errors.Contains("Info:"));
                }
                catch (RpcException ee)
                {
                    _logger.Error("Can not read getinfo(): {0:l}", ee.Message);
                    Healthy = false; // set healthy status to false as we couldn't get a reply.
                }

            }

            try // read getmininginfo() based data.
            {
                var miningInfo = _daemonClient.GetMiningInfo();

                // read data.
                Hashrate = miningInfo.NetworkHashPerSec;
                Difficulty = miningInfo.Difficulty;
                Round = miningInfo.Blocks + 1;
                if (!Testnet)
                    Testnet = miningInfo.Testnet;
            }
            catch (RpcException e)
            {
                _logger.Error("Can not read getmininginfo(): {0:l}", e.Message);
                Hashrate = 0;
                Difficulty = 0;
                Round = -1;
                Healthy = false; // set healthy status to false as we couldn't get a reply.
            }

            try // read getblocktemplate() based data.
            {
                var blockTemplate = _daemonClient.GetBlockTemplate(_poolConfig.Coin.Options.BlockTemplateModeRequired);
                Reward = (UInt64)blockTemplate.Coinbasevalue / 100000000; // coinbasevalue is in satoshis, convert it to actual coins.
            }
            catch (RpcException e)
            {
                _logger.Error("Can not read getblocktemplate(): {0:l}", e.Message);
                Reward = 0;
            }
        }

        private void PrintNetworkInfo()
        {
            _logger.Information("symbol: {0:l} algorithm: {1:l} " +
                                "version: {2:l} protocol: {3} wallet: {4} " +
                                "network difficulty: {5:0.00000000} block difficulty: {6:0.00} network hashrate: {7:l} " +
                                "network: {8:l} peers: {9} blocks: {10} errors: {11:l} ",
                _poolConfig.Coin.Symbol,
                _poolConfig.Coin.Algorithm,
                CoinVersion,
                ProtocolVersion,
                WalletVersion,
                Difficulty,
                Difficulty * _hashAlgorithm.Multiplier,
                Hashrate.GetReadableHashrate(),
                Testnet ? "testnet" : "mainnet",
                Connections,
                Round - 1,
                string.IsNullOrEmpty(Errors) ? "none" : Errors);
        }


        private void DetectSubmitBlockSupport()
        {
            // issue a submitblock() call too see if it's supported.
            // If the coin supports the submitblock() call it's should return a RPC_DESERIALIZATION_ERROR (-22) - 'Block decode failed' as we just supplied an empty string as block hash.
            // otherwise if it doesn't support the call, it should return a RPC_METHOD_NOT_FOUND (-32601) - 'Method not found' error.

            try
            {
                var response = _daemonClient.SubmitBlock(string.Empty);
            }
            catch (RpcException e)
            {
                if (e is RpcErrorException error)
                {
                    switch (error.Code)
                    {
                        case (int)RpcErrorCode.RPC_METHOD_NOT_FOUND:
                            _poolConfig.Coin.Options.SubmitBlockSupported = false; // the coin doesn't support submitblock().
                            _logger.Debug("submitblock() is NOT SUPPORTED by your wallet software.");
                            break;
                        case (int)RpcErrorCode.RPC_DESERIALIZATION_ERROR:
                            _poolConfig.Coin.Options.SubmitBlockSupported = true; // the coin supports submitblock().
                            break;
                        default:
                            _logger.Error("Recieved an unexpected response for DetectSubmitBlockSupport() - {0}, {1:l}", error.Code, e.Message);
                            break;
                    }
                }
                else
                    _logger.Error("Can not probe submitblock() support: {0:l}", e.Message);
            }
        }

        private void DetectProofOfStakeCoin()
        {
            // use getdifficulty() to determine if it's POS coin.

            try
            {
                /*  By default proof-of-work coins return a floating point as difficulty (https://en.bitcoin.it/wiki/Original_Bitcoin_client/API_calls_lis).
                 *  Though proof-of-stake coins returns a json-object;
                 *  { "proof-of-work" : 41867.16992903, "proof-of-stake" : 0.00390625, "search-interval" : 0 }
                 *  So basically we can use this info to determine if assigned coin is a proof-of-stake one.
                 */

                var response = _daemonClient.MakeRawRequest("getdifficulty");
                if (response.Contains("proof-of-stake")) // if response contains proof-of-stake field
                    _poolConfig.Coin.Options.IsProofOfStakeHybrid = true; // then automatically set coin-config.IsPOS to true.
            }
            catch (RpcException e)
            {
                _logger.Error("Can not read getdifficulty(): {0:l}", e.Message);
            }
        }
    }
}