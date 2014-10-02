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

using FluentMigrator;

namespace CoiniumServ.Persistance.Layers.Hybrid.Migrations
{
    [Migration(20140901)]
    public class M001CreateBlocksTable : Migration
    {
        public override void Up()
        {
            // create the blocks table.
            Create.Table("blocks")
                .WithColumn("id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("height").AsInt32().NotNullable()
                .WithColumn("orphaned").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("confirmed").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("blockHash").AsString(65).NotNullable()
                .WithColumn("txHash").AsString(65).NotNullable()
                .WithColumn("amount").AsDecimal().NotNullable()
                .WithColumn("time").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("blocks");
        }
    }
}
