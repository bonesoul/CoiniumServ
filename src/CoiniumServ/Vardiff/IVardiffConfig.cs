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

using CoiniumServ.Configuration;

namespace CoiniumServ.Vardiff
{
    public interface IVardiffConfig : IConfig
    {
        bool Enabled { get; }

        /// <summary>
        /// minimum difficulty that can be assigned to miners.
        /// </summary>
        float MinimumDifficulty { get; }

        /// <summary>
        /// maximum difficulty that can be assigned to miners.
        /// </summary>
        float MaximumDifficulty { get; }

        /// <summary>
        /// try to get a single share per this many seconds from miner.
        /// </summary>
        int TargetTime { get; }

        /// <summary>
        /// retarget a miners difficulty ever this many seconds.
        /// </summary>
        int RetargetTime { get; }

        /// <summary>
        /// allow difficulty for a miner to vary this percent without retargeting.
        /// </summary>
        int VariancePercent { get; }
    }
}
