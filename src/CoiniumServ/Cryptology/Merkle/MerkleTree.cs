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
using System.Linq;
using CoiniumServ.Utils.Extensions;
using CoiniumServ.Utils.Helpers.Misc;

namespace CoiniumServ.Cryptology.Merkle
{
    /// <summary>
    /// Merkle tree builder.
    /// </summary>
    /// <remarks>
    /// To get a better understanding of merkle trees check: http://www.youtube.com/watch?v=gUwXCt1qkBU#t=09m09s 
    /// </remarks>
    /// <specification>https://en.bitcoin.it/wiki/Protocol_specification#Merkle_Trees</specification>
    /// <example>
    /// Python implementation: http://runnable.com/U3HnDaMrJFk3gkGW/bitcoin-block-merkle-root-2-for-python
    /// Original implementation: https://code.google.com/p/bitcoinsharp/source/browse/src/Core/Block.cs#330
    /// </example>
    public class MerkleTree : IMerkleTree
    {
        /// <summary>
        /// The steps in tree.
        /// </summary>
        public IList<byte[]> Steps { get; private set; }

        /// <summary>
        /// List of hashes, will be used for calculation of merkle root. 
        /// <remarks>This is not a list of all transactions, it only contains prepared hashes of steps of merkle tree algorithm. Please read some materials (http://en.wikipedia.org/wiki/Hash_tree) for understanding how merkle trees calculation works. (http://mining.bitcoin.cz/stratum-mining)</remarks>
        /// <remarks>The coinbase transaction is hashed against the merkle branches to build the final merkle root.</remarks>
        /// </summary>
        public List<string> Branches
        {
            get
            {
                return Steps.Select(step => step.ToHexString()).ToList();
            }
        }

        /// <summary>
        /// Creates a new merkle-tree instance.
        /// </summary>
        /// <param name="hashList"></param>
        public MerkleTree(IEnumerable<byte[]> hashList)
        {
            Steps = CalculateSteps(hashList);
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// python: http://runnable.com/U3jqtyYUmAUxtsSS/bitcoin-block-merkle-root-python
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/master/lib/merkleTree.js#L9
        /// </example>
        /// <param name="hashList"></param>
        /// <returns></returns>
        private IList<byte[]> CalculateSteps(IEnumerable<byte[]> hashList)
        {
            var steps = new List<byte[]>();

            var L = new List<byte[]> {null};
            L.AddRange(hashList);

            var startL = 2;
            var Ll = L.Count;

            if (Ll > 1)
            {
                while (true)
                {
                    if (Ll == 1)
                        break;

                    steps.Add(L[1]);

                    if (Ll%2 == 1)
                        L.Add(L[L.Count - 1]);

                    var Ld = new List<byte[]>();

                    foreach (int i in Range.From(startL).To(Ll).WithStepSize(2))
                    {
                        Ld.Add(MerkleJoin(L[i], L[i + 1]));
                    }

                    L = new List<byte[]> {null};
                    L.AddRange(Ld);
                    Ll = L.Count;
                }
            }
            return steps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// nodejs: https://github.com/zone117x/node-stratum-pool/blob/master/lib/merkleTree.js#L11
        /// </example>
        /// <param name="hash1"></param>
        /// <param name="hash2"></param>
        /// <returns></returns>
        private byte[] MerkleJoin(byte[] hash1, byte[] hash2)
        {
            var joined = hash1.Append(hash2);
            var dHashed = joined.DoubleDigest();
            return dHashed;
        }

        public byte[] WithFirst(byte[] first)
        {
            foreach (var step in Steps)
            {
                first = first.Append(step).DoubleDigest();
            }

            return first;
        }
    }
}
