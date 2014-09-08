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
    [Migration(20140908)]
    public class M002Payments : Migration
    {
        public override void Up()
        {
            // we'll be using block height as our foreign keys in payments tables, 
            // so we need to first set the new primary key as height column.
            Alter.Table("blocks").AlterColumn("id").AsInt32().NotNullable(); // remove primary key property from 'id' column.
            Delete.PrimaryKey("id").FromTable("blocks"); // remove primary key on 'id'.
            Create.PrimaryKey("height").OnTable("blocks").Column("height"); // create new primary key on 'height' column.
            Delete.Column("id").FromTable("blocks"); // delete the 'id' column as we don't need it anymore.
        }

        public override void Down()
        {
            // revert back the changes on blocks height & id columns
            Alter.Table("blocks").AlterColumn("height").AsInt32().NotNullable();
            Delete.PrimaryKey("height").FromTable("blocks");
            Alter.Table("blocks").AddColumn("id").AsInt32().NotNullable().PrimaryKey().Identity();
        }
    }
}
