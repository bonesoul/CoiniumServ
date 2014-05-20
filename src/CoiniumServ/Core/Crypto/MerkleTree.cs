/*
 *   Coinium - Crypto Currency Pool Software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using Coinium.Common.Extensions;
using Coinium.Common.Helpers.Misc;

namespace Coinium.Core.Crypto
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
    public class MerkleTree
    {
        /// <summary>
        /// Leaves of the merkle tree.
        /// </summary>
        public IList<byte[]> Tree { get; private set; }

        /// <summary>
        /// The steps in tree.
        /// </summary>
        public IList<byte[]> Steps { get; private set; } 

        /// <summary>
        /// The root of the merkle tree.
        /// </summary>
        public Hash Root { get; private set; }

        /// <summary>
        /// Creates a new merkle-tree instance.
        /// </summary>
        /// <param name="hashList"></param>
        public MerkleTree(List<byte[]> hashList)
        {
            this.Tree = this.Build(hashList);
            this.Steps = this.CalculateSteps(hashList);
            this.Root = this.Tree.Count > 0 ? new Hash(this.Tree[this.Tree.Count - 1]) : Hash.ZeroHash;
        }

        /// <summary>
        /// Builds merkle tree.
        /// </summary>
        /// <param name="hashList"></param>
        /// <returns></returns>
        private IList<byte[]> Build(List<byte[]> hashList)
        {
            // The Merkle root is based on a tree of hashes calculated from the transactions:
            //
            //          root
            //             /\
            //            /  \
            //          A      B
            //         / \    / \
            //       t1 t2  t3 t4
            //
            // The tree is represented as a list: t1,t2,t3,t4,A,B,root where each entry is a hash.
            //
            // The hashing algorithm is double SHA-256. The leaves are a hash of the serialized contents of the
            // transaction. The interior nodes are hashes of the concentration of the two child hashes.
            //
            // This structure allows the creation of proof that a transaction was included into a block without having to
            // provide the full block contents. Instead, you can provide only a Merkle branch. For example to prove tx2 was
            // in a block you can just provide tx2, the hash(tx1) and B. Now the other party has everything they need to
            // derive the root, which can be checked against the block header. These proofs aren't used right now but
            // will be helpful later when we want to download partial block contents.
            //
            // Note that if the number of transactions is not even the last tx is repeated to make it so (see
            // tx3 above). A tree with 5 transactions would look like this:
            //
            //                root
            //                /  \
            //              1     \
            //            /  \     \
            //          2     3     4
            //         / \   / \   /  \
            //       t1 t2  t3 t4  t5 t5

            var tree = new List<byte[]>();

            // Start by adding all the hashes of the transactions as leaves of the tree.
            foreach (var item in hashList)
            {
                tree.Add(item);
            }

            var levelOffset = 0; // Offset in the list where the currently processed level starts.

            // Step through each level, stopping when we reach the root (levelSize == 1).
            for (var levelSize = hashList.Count; levelSize > 1; levelSize = (levelSize + 1) / 2)
            {
                // For each pair of nodes on that level:
                for (var left = 0; left < levelSize; left += 2)
                {   
                    // The right hand node can be the same as the left hand, in the case where we don't have enough transactions.
                    var right = Math.Min(left + 1, levelSize - 1);
                    var leftBytes = tree[levelOffset + left].ReverseBytes();
                    var rightBytes = tree[levelOffset + right].ReverseBytes();

                    tree.Add(Utils.DoubleDigestTwoBuffers(leftBytes, 0, 32, rightBytes, 0, 32).ReverseBytes());
                }
                // Move to the next level.
                levelOffset += levelSize;
            }
            return tree;
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
        private IList<byte[]> CalculateSteps(List<byte[]> hashList)
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
                        Ld.Add(this.MerkleJoin(L[i], L[i + 1]));
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
            foreach (var step in this.Steps)
            {
                first = first.Append(step).DoubleDigest();
            }

            return first;
        }
    }
}
