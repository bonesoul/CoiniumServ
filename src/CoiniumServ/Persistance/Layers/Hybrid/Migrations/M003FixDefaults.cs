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

using FluentMigrator;

namespace CoiniumServ.Persistance.Layers.Hybrid.Migrations
{    
    /// <summary>
    /// Fixes a bug in M002Payment.cs where default values for Block.Accounted and Payment.Completed was not set.
    /// </summary>
    [Migration(20141024)]
    public class M003FixDefaults : Migration
    {
        public override void Up()
        {
            Alter.Table("Block").AlterColumn("Accounted").AsBoolean().NotNullable().WithDefaultValue("0"); // let Block.Accounted have default value of 0.
            Alter.Table("Payment").AlterColumn("Completed").AsBoolean().NotNullable().WithDefaultValue("0"); // let Payment.Completed have default value of 0.
            Alter.Table("Block").AlterColumn("Reward").AsDecimal().NotNullable().WithDefaultValue("0"); // let Block.Reward have default value of 0
        }

        public override void Down()
        {
            throw new System.NotImplementedException();
        }
    }
}
