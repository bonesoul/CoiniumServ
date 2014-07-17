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
namespace CoiniumServ.Coin.Helpers
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
