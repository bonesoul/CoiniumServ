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

using System;
using System.Collections.Generic;

namespace Coinium.Coin.Daemon.Responses
{
    public interface IBlockTemplate
    {
        string Bits { get; set; }

        UInt32 CurTime { get; set; }

        int Height { get; set; }

        string PreviousBlockHash { get; set; }

        int SigOpLimit { get; set; }

        int SizeLimit { get; set; }

        BlockTemplateTransaction[] Transactions { get; set; }

        UInt32 Version { get; set; }

        CoinBaseAux CoinBaseAux { get; set; }

        int CoinbaseTxt { get; set; }

        Int64 Coinbasevalue { get; set; }

        int WorkId { get; set; }

        string Target { get; set; }

        UInt32 MinTime { get; set; }

        List<string> Mutable { get; set; }

        string NonceRange { get; set; }
    }
}
