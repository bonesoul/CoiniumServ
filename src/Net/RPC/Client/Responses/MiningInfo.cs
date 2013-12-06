/*
 * Coinium project, Copyright (C) 2013, Int6 Studios - All Rights Reserved. - http://www.coinium.org
 *
 */

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

namespace coinium.Net.RPC.Client.Responses
{
    public class MiningInfo
    {
        public int Blocks { get; set; }
        public int CurrentBockSize { get; set; }
        public int CurrentBlockTx { get; set; }
        public double Difficulty { get; set; }
        public string Errors { get; set; }
        public bool Generate { get; set; }
        public int GenProcLimit { get; set; }
        public int HashesPerSec { get; set; }
        public int PooledTx { get; set; }
        public bool Testnet { get; set; }
    }
}
