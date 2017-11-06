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

using System;
using JsonConfig;
using Serilog;

namespace CoiniumServ.Coin.Config
{
    public class CoinOptions:ICoinOptions
    {
        public bool IsProofOfStakeHybrid { get; set; }
        
        public bool BlockTemplateModeRequired { get; private set; }
        
        public bool UseDefaultAccount { get; private set; }

        public bool TxMessageSupported { get; private set; }

        public bool SubmitBlockSupported { get; set; }

        public bool Valid { get; private set; }

        public CoinOptions(dynamic config)
        {
            try
            {
                IsProofOfStakeHybrid = config.isProofOfStakeHybrid is NullExceptionPreventer ? false : config.isProofOfStakeHybrid;
                BlockTemplateModeRequired = config.blockTemplateModeRequired is NullExceptionPreventer ? false : config.blockTemplateModeRequired;
                UseDefaultAccount = config.useDefaultAccount is NullExceptionPreventer ? false : config.useDefaultAccount;
                TxMessageSupported = config.txMessageSupported is NullExceptionPreventer ? false : config.txMessageSupported;
                SubmitBlockSupported = true; // setting true by default, but will be actually checked in NetworkInfo.cs to see if the coin supports it.
                Valid = true;
            }
            catch (Exception e)
            {
                Valid = false;
                Log.Logger.ForContext<CoinOptions>().Error("Error loading coin options: {0:l}", e.Message);
            }
        }
    }
}
