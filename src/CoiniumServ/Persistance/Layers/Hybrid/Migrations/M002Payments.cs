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
            // Fix table & field names - use signular words & pascal case.
            Rename.Table("blocks").To("Block");
            Rename.Column("id").OnTable("Block").To("Id");
            Rename.Column("height").OnTable("Block").To("Height");
            Rename.Column("orphaned").OnTable("Block").To("Orphaned");
            Rename.Column("confirmed").OnTable("Block").To("Confirmed");
            Rename.Column("blockHash").OnTable("Block").To("BlockHash");
            Rename.Column("txHash").OnTable("Block").To("TxHash");
            Rename.Column("amount").OnTable("Block").To("Amount");
            Rename.Column("time").OnTable("Block").To("CreatedAt");

            // we'll be using block height as our foreign keys in payments tables, 
            // so we need to first set the new primary key as height column.
            Alter.Table("Block").AlterColumn("Id").AsInt32().NotNullable(); // remove primary key property from 'id' column.
            Delete.PrimaryKey("Id").FromTable("Block"); // remove primary key on 'id'.
            Create.PrimaryKey("Height").OnTable("Block").Column("Height"); // create new primary key on 'height' column.
            Delete.Column("Id").FromTable("Block"); // delete the 'id' column as we don't need it anymore.

            // add reward column to block table - need to use SQL here as we are adding the column after 'Amount'.
            Execute.Sql("ALTER TABLE Block ADD Reward DECIMAL NOT NULL AFTER Amount");

            // create the users table.
            Create.Table("User")                
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Username").AsString().NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            // create the awaitingPayments table.
            Create.Table("AwaitingPayment")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Block").AsInt32().ForeignKey("Block", "height")
                .WithColumn("User").AsInt32().ForeignKey("User", "Id")
                .WithColumn("Amount").AsDecimal().NotNullable()
                .WithColumn("OriginalCurrency").AsString(4).NotNullable()
                .WithColumn("PaymentCurrency").AsString(4).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            // create the completedPayments table.
            Create.Table("CompletedPayments")
                .WithColumn("Id").AsInt32().ForeignKey("AwaitingPayment", "Id").PrimaryKey()
                .WithColumn("User").AsInt32().ForeignKey("User", "Id")
                .WithColumn("OriginalAmmount").AsDecimal().NotNullable()
                .WithColumn("OriginalCurrency").AsString(4).NotNullable()
                .WithColumn("PaymentAmount").AsDecimal().NotNullable()
                .WithColumn("PaymentCurrency").AsString(4).NotNullable()
                .WithColumn("TxId").AsString(64).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            // revert back the changes on blocks height & id columns
            Alter.Table("Block").AlterColumn("Height").AsInt32().NotNullable();
            Delete.PrimaryKey("Height").FromTable("Blocks");
            Alter.Table("Blocks").AddColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity();

            // revert back table & field names
            Rename.Table("Block").To("blocks");
            Rename.Column("Id").OnTable("blocks").To("id");
            Rename.Column("Height").OnTable("blocks").To("height");
            Rename.Column("Orphaned").OnTable("blocks").To("orphaned");
            Rename.Column("Confirmed").OnTable("blocks").To("confirmed");
            Rename.Column("BlockHash").OnTable("blocks").To("blockHash");
            Rename.Column("TxHash").OnTable("blocks").To("txHash");
            Rename.Column("Amount").OnTable("blocks").To("amount");
            Rename.Column("CreatedAt").OnTable("blocks").To("time");

            // delete the reward column from block table
            Delete.Column("Reward").FromTable("Block");

            // delete the newly created tables.
            Delete.Table("User");
            Delete.Table("AwaitingPayment");
            Delete.Table("CompletedPayment");
        }
    }
}
