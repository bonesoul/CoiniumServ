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

namespace CoiniumServ.Daemon.Responses
{
    // Documentation: 
    // https://en.bitcoin.it/wiki/Getwork
    // https://github.com/sinisterchipmunk/bitpool/wiki/Bitcoin-Mining-Pool-Developer's-Reference
    // https://bitcointalk.org/index.php?topic=51281.0

    // Note: Do not rename member field names with uppercase ones as it will break getwork protocol @ json-rpc 1.0.

    public class Getwork
    {        
        /// <summary>
        /// This should be advertised iff the miner supports generating its own midstates. In this case, the pool may decide to omit the now-deprecated "midstate" and "hash1" fields in the work response.
        /// </summary>
        public string midstate { get; set; }

        /// <summary>
        /// Pre-processed SHA-2 input chunks, in little-endian order, as a hexadecimal-encoded string
        /// </summary>
        public string data { get; set; }

        public string hash1 { get; set; }

        /// <summary>
        /// Proof-of-work hash target as a hexadecimal-encoded string
        /// </summary>
        public string target { get; set; }

        /// <summary>
        /// Brief specification of proof-of-work algorithm. Not provided by bitcoind or most poolservers.
        /// </summary>
        public string algorithm { get; set; }
    }
}
