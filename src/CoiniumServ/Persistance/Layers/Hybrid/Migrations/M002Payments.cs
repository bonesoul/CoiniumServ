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

            // new payment processor uses block height as the foreign key regularly.
            // so we need to first drop the 'Id' column and set the new primary key as 'Height' column.
            Alter.Table("Block").AlterColumn("Id").AsInt32().NotNullable(); // remove primary key property from 'Id' column.
            Delete.PrimaryKey("Id").FromTable("Block"); // remove primary key on 'Id'.
            Create.PrimaryKey("Height").OnTable("Block").Column("Height"); // create new primary key on 'Height' column.
            Delete.Column("Id").FromTable("Block"); // delete the 'Id' column as we don't need it anymore.

            // add reward column to block table.
            Execute.Sql("ALTER TABLE Block ADD Reward DECIMAL(19,5) NOT NULL AFTER Amount");

            // add accounted column to block table
            Execute.Sql("ALTER TABLE Block ADD Accounted Boolean NOT NULL AFTER Confirmed");

            // create the users table.
            Create.Table("Account")                
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Username").AsString().NotNullable().Unique()
                .WithColumn("Address").AsString(34).NotNullable().Unique()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            // create the payout table.
            Create.Table("Payment")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Block").AsInt32().ForeignKey("Block", "Height")
                .WithColumn("AccountId").AsInt32().ForeignKey("Account", "Id")
                .WithColumn("Amount").AsDecimal().NotNullable()
                .WithColumn("Completed").AsBoolean().NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();

            // create the transaction table.
            Create.Table("Transaction")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("AccountId").AsInt32().ForeignKey("Account", "Id")
                .WithColumn("PaymentId").AsInt32().ForeignKey("Payment", "Id")
                .WithColumn("Amount").AsDecimal().NotNullable()
                .WithColumn("Currency").AsString(4).NotNullable()
                .WithColumn("TxHash").AsString(64).NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            // revert back the changes on blocks height & id columns
            Alter.Table("Block").AlterColumn("Height").AsInt32().NotNullable(); // remove the private key property from 'Height' column.
            Delete.PrimaryKey("Height").FromTable("Block"); // remove the primary key on 'Height'.
            Alter.Table("Block").AddColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity(); // create the 'Id' column again and set is as primary key.            

            // delete the new columns.
            Delete.Column("Reward").FromTable("Block");
            Delete.Column("Accounted").FromTable("Block");

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

            // delete the newly created tables.
            Delete.Table("Account");
            Delete.Table("Payment");
            Delete.Table("Transaction");
        }
    }
}
