/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
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

namespace Coinium.Core.Coin.Helpers
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
            get { return this._coins; }
            set
            {
                this._coins = value;
                this._millitBits = value*1000;
                this._microBits = this._millitBits*1000;
                this._satoshis = this._microBits * 100;
            }
        }

        /// <summary>
        /// Gets or sets the amount in millibits.
        /// </summary>
        public double MilliBits
        {
            get { return this._millitBits; }
            set
            {
                this._millitBits = value;
                this._coins = value/1000;
                this._microBits = this._millitBits * 1000;
                this._satoshis = this._microBits * 100;
            }
        }

        /// <summary>
        /// Gets or sets the amount in microbits.
        /// </summary>
        public double MicroBits
        {
            get { return this._microBits; }
            set
            {
                this._microBits = value;
                this._millitBits = this._microBits / 1000;
                this._coins = this._millitBits / 1000;
                this._satoshis = this._microBits * 100;
            }
        }

        /// <summary>
        /// Gets or sets the amount in satoshis.
        /// </summary>
        public double Satoshis
        {
            get { return this._satoshis; }
            set
            {
                this._satoshis = value;
                this._microBits = this._satoshis / 100;
                this._millitBits = this._microBits / 1000;
                this._coins = this._millitBits / 1000;                
            }
        }        
    }
}
