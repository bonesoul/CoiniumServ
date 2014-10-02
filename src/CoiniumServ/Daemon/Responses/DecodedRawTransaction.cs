#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using System.Collections.Generic;

namespace CoiniumServ.Daemon.Responses
{
    public class DecodedRawTransaction
    {
        public string Hex { get; set; }
        public string TxId { get; set; }
        public int Version { get; set; }
        public int Locktime { get; set; }
        public List<Vin> VIn { get; set; }
        public List<Vout> VOut { get; set; }
    }

    public class Vin
    {
        public string TxId { get; set; }
        public int VOut { get; set; }
        public ScriptSig ScriptSig { get; set; }
        public long Sequence { get; set; }
    }

    public class ScriptSig
    {
        public string Asm { get; set; }
        public string Hex { get; set; }
    }

    public class Vout
    {
        public double Value { get; set; }
        public int N { get; set; }
        public ScriptPubKey ScriptPubKey { get; set; }
    }

    public class ScriptPubKey
    {
        public string Asm { get; set; }
        public string Hex { get; set; }
        public int ReqSigs { get; set; }
        public string Type { get; set; }
        public List<string> Addresses { get; set; }
    }
}
