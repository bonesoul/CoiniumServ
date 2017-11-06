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

namespace CoiniumServ.Utils.Helpers
{
    /// <summary>
    /// Amount helper class.
    /// </summary>
    public class Amount
    {
        private double _coins;
        private double _millitBits;
        private double _microBits;
        private double _satoshis;

        /// <summary>
        /// Gets or sets the amount in coins.
        /// </summary>
        public double Coins
        {
            get { return _coins; }
            set
            {
                _coins = value;
                _millitBits = value*1000;
                _microBits = _millitBits*1000;
                _satoshis = _microBits * 100;
            }
        }

        /// <summary>
        /// Gets or sets the amount in millibits.
        /// </summary>
        public double MilliBits
        {
            get { return _millitBits; }
            set
            {
                _millitBits = value;
                _coins = value/1000;
                _microBits = _millitBits * 1000;
                _satoshis = _microBits * 100;
            }
        }

        /// <summary>
        /// Gets or sets the amount in microbits.
        /// </summary>
        public double MicroBits
        {
            get { return _microBits; }
            set
            {
                _microBits = value;
                _millitBits = _microBits / 1000;
                _coins = _millitBits / 1000;
                _satoshis = _microBits * 100;
            }
        }

        /// <summary>
        /// Gets or sets the amount in satoshis.
        /// </summary>
        public double Satoshis
        {
            get { return _satoshis; }
            set
            {
                _satoshis = value;
                _microBits = _satoshis / 100;
                _millitBits = _microBits / 1000;
                _coins = _millitBits / 1000;                
            }
        }        
    }
}
