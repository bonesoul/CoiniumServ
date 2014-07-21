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

using System;
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Daemon.Config;
using CoiniumServ.Daemon.Requests;
using CoiniumServ.Daemon.Responses;

/* This file is based on https://github.com/BitKoot/BitcoinRpcSharp */

/* Possible alternative implementations:
 *      https://en.bitcoin.it/wiki/API_reference_(JSON-RPC)#.NET_.28C.23.29 
 *      https://github.com/GeorgeKimionis/BitcoinLib
 *      https://code.google.com/p/bitcoinsharp/     
 *      https://sourceforge.net/projects/bitnet 
 */

// Original bitcoin api call list: https://en.bitcoin.it/wiki/Original_Bitcoin_client/API_Calls_list
// Rpc error codes: https://github.com/bitcoin/bitcoin/blob/master/src/rpcprotocol.h#L34

namespace CoiniumServ.Daemon
{
    /// <summary>
    /// RPC client for coind's.
    /// </summary>
    public class DaemonClient : DaemonBase, IDaemonClient
    {
        public static readonly object[] EmptyString = {}; // used as empty parameter.

        public DaemonClient(string pool, IDaemonConfig daemonConfig)
            : base(pool, daemonConfig)
        { }

        /// <summary>
        /// Version 0.8: Attempts add or remove node from the addnode list or try a connection to node once.
        /// </summary>
        /// <param name="nRquired">Number of required signatures to sign a transaction.</param>
        /// <param name="publicKeys">Public keys associated with the multi signature address.</param>
        /// <param name="account">The account to associate with the multi signature address.</param>
        /// <returns>The created multi signature address.</returns>
        public string AddMultiSigAddress(int nRquired, List<string> publicKeys, string account)
        {
            return MakeRequest<string>("addmultisigaddress", nRquired, publicKeys, account);
        }

        /// <summary>
        /// Version 0.8: Attempts add or remove node from the addnode list or try a connection to node once.
        /// </summary>
        /// <param name="node">Node URL to add.</param>
        /// <param name="mode">One of the following: add / remove / onetry.</param>
        public void AddNode(string node, string mode)
        {
            MakeRequest<string>("addnode", node, mode);
        }

        /// <summary>
        /// Safely copies wallet.dat to destination, which can be a directory or a path with a filename. 
        /// </summary>
        /// <param name="walletPath">
        /// The location to copy the wallet.dat to. Can be a directory or a path with a filename.
        /// </param>
        public void BackupWallet(string walletPath)
        {
            MakeRequest<string>("backupwallet", walletPath);
        }

        /// <summary>
        /// Creates a multi-signature address.
        /// </summary>
        /// <param name="nRquired">Number of required signatures to sign a transaction.</param>
        /// <param name="publicKeys">Public keys associated with the multi signature address.</param>
        /// <returns></returns>
        public MultisigAddress CreateMultiSig(int nRquired, List<string> publicKeys)
        {
            return MakeRequest<MultisigAddress>("createmultisig", nRquired, publicKeys);
        }

        /// <summary>
        /// Version 0.7: Creates a raw transaction spending given inputs.
        /// </summary>
        /// <param name="rawTransaction">The raw transaction details.</param>
        /// <returns>The raw transaction hex. The transaction is not signed yet.</returns>
        public string CreateRawTransaction(CreateRawTransaction rawTransaction)
        {
            return MakeRequest<string>("createrawtransaction", rawTransaction.Inputs, rawTransaction.Outputs);
        }

        /// <summary>
        /// Version 0.7: Produces a human-readable JSON object for a raw transaction.
        /// </summary>
        /// <param name="rawTransactionHex">The hex of the raw transaction.</param>
        /// <returns>The decoded raw transaction details.</returns>
        public DecodedRawTransaction DecodeRawTransaction(string rawTransactionHex)
        {
            return MakeRequest<DecodedRawTransaction>("decoderawtransaction", rawTransactionHex);
        }

        /// <summary>
        /// Reveals the private key corresponding to bitcoinaddress.
        /// </summary>
        /// <param name="bitcoinAddress">The bitcoin address to dump the private key of.</param>
        /// <returns>The private key corresponding to the given bitcoin address.</returns>
        public string DumpPrivateKey(string bitcoinAddress)
        {
            return MakeRequest<string>("dumpprivkey", bitcoinAddress);
        }

        /// <summary>
        /// Encrypts the wallet with a passphrase.
        /// </summary>
        /// <param name="passPhrase">The passphrase to encrypt the wallet with.</param>
        /// <returns>A message detailing the result of the encryption of the wallet.</returns>
        public string EncryptWallet(string passPhrase)
        {
            return MakeRequest<string>("encryptwallet", passPhrase);
        }

        /// <summary>
        /// Returns the account associated with the given address.
        /// </summary>
        /// <param name="bitcoinAddress">The address to get the account for.</param>
        /// <returns>The account the given address belongs to.</returns>
        public string GetAccount(string bitcoinAddress)
        {
            return MakeRequest<string>("getaccount", bitcoinAddress);
        }

        /// <summary>
        /// Returns the current bitcoin address for receiving payments to this account.
        /// </summary>
        /// <param name="account">The account to get the address for.</param>
        /// <returns>The address which belongs to the given account.</returns>
        public string GetAccountAddress(string account)
        {
            return MakeRequest<string>("getaccountaddress", account);
        }

        /// <summary>
        /// Version 0.8: Returns information about the given added node, or all 
        /// added nodes (note that onetry addnodes are not listed here). If dns 
        /// is false, only a list of added nodes will be provided, otherwise 
        /// connected information will also be available.
        /// </summary>
        /// <param name="dns">
        /// If false, only a list of added nodes will be provided, otherwise 
        /// connected information will also be available.
        /// </param>
        /// <param name="node">The node URL.</param>
        /// <returns></returns>
        public AddedNodeInfo GetAddedNodeInfo(bool dns, string node = "")
        {
            return MakeRequest<AddedNodeInfo>("getaddednodeinfo", dns, node);
        }

        /// <summary>
        /// Returns the list of addresses for the given account.
        /// </summary>
        /// <param name="account">The account to return a list of addresses for.</param>
        /// <returns>A list of address associated with the account.</returns>
        public List<string> GetAddressesByAccount(string account)
        {
            return MakeRequest<List<string>>("getaddressesbyaccount", account);
        }

        /// <summary>
        /// If [account] is not specified, returns the server's total available balance.
        /// If [account] is specified, returns the balance in the account. 
        /// </summary>
        /// <param name="account">The account to get the balance for.</param>
        /// <returns>The balance of the account or the total wallet.</returns>
        public decimal GetBalance(string account = "")
        {
            return string.IsNullOrEmpty(account)
                ? MakeRequest<decimal>("getbalance", EmptyString)
                : MakeRequest<decimal>("getbalance", account);
        }

        /// <summary>
        /// Returns the hash of the best (tip) block in the longest block chain.
        /// </summary>
        /// <returns></returns>
        public string GetBestBlockHash()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns information about the block with the given hash. 
        /// </summary>
        /// <param name="hash">The hash of the block to get information about.</param>
        /// <returns>Information about the block.</returns>
        public Block GetBlock(string hash)
        {
            return MakeRequest<Block>("getblock", hash);
        }

        /// <summary>
        /// Returns the number of blocks in the longest block chain. 
        /// </summary>
        /// <returns>The number of blocks in the longest block chain.</returns>
        public long GetBlockCount()
        {
            return MakeRequest<long>("getblockcount", null);
        }

        /// <summary>
        /// Returns hash of block in best-block-chain at index; index 0 is the genesis block.
        /// </summary>
        /// <param name="index">The index of the block.</param>
        /// <returns>The hash of the block at the given index.</returns>
        public string GetBlockHash(long index)
        {
            return MakeRequest<string>("getblockhash", index);
        }

        [Obsolete("Deprecated. Removed in version 0.7. Use getblockcount.")]
        public long GetBlockNumber()
        {
            return GetBlockCount();
        }

        /// <summary>
        /// https://github.com/bitcoin/bips/blob/master/bip-0022.mediawiki
        /// https://en.bitcoin.it/wiki/Getblocktemplate
        /// </summary>
        public BlockTemplate GetBlockTemplate()
        {
            var capabilities = new Dictionary<string, object>
            {
                {"capabilities", new List<string> {"coinbasetxn", "workid", "coinbase/append"}}
            };

            return MakeRequest<BlockTemplate>("getblocktemplate", capabilities);
        }

        /// <summary>
        /// Submits a block.
        /// </summary>
        /// <param name="blockHex"></param>
        /// <returns></returns>
        public BlockTemplate GetBlockTemplate(string blockHex)
        {
            var submission = new Dictionary<string, object>
            {
                {"mode", "submit"},
                {"data", blockHex}
            };

            return MakeRequest<BlockTemplate>("getblocktemplate", submission);
        }

        /// <summary>
        /// Attempts to submit new block to network.
        /// </summary>
        public string SubmitBlock(string blockHex)
        {
            return MakeRequest<string>("submitblock", blockHex);
        }

        /// <summary>
        /// Returns the number of connections to other nodes. 
        /// </summary>
        /// <returns>The number of connections to other nodes.</returns>
        public long GetConnectionCount()
        {
            return MakeRequest<long>("getconnectioncount", null);
        }

        /// <summary>
        /// Returns the proof-of-work difficulty as a multiple of the minimum difficulty. 
        /// </summary>
        /// <returns>The proof of work difficulty as a multiple of the minimum difficulty.</returns>
        public decimal GetDifficulty()
        {
            return MakeRequest<decimal>("getdifficulty", null);
        }

        /// <summary>
        /// Returns true or false whether bitcoind is currently generating hashes. 
        /// </summary>
        /// <returns>True if bitcoind is currently generating hashes.</returns>
        public bool GetGenerate()
        {
            return MakeRequest<bool>("getgenerate", null);
        }

        /// <summary>
        /// Returns a recent hashes per second performance measurement while generating.
        /// </summary>
        /// <returns>A recent hashes per second performance measurement while generating.</returns>
        public decimal GetHashesPerSec()
        {
            return MakeRequest<decimal>("gethashespersec", null);
        }

        /// <summary>
        /// Returns an object containing various state info.
        /// </summary>
        /// <returns>An object containing some general information.</returns>
        public Info GetInfo()
        {
            return MakeRequest<Info>("getinfo", null);
        }

        [Obsolete("Replaced in v0.7.0 with getblocktemplate, submitblock, getrawmempool")]
        public void GetMemoryPool()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an object containing mining-related information: blocks, currentblocksize, currentblocktx, difficulty, errors, generate, genproclimit, hashespersec, pooledtx, testnet 
        /// </summary>
        /// <returns>An object containing mining information.</returns>
        public MiningInfo GetMiningInfo()
        {
            return MakeRequest<MiningInfo>("getmininginfo", null);
        }

        /// <summary>
        /// Returns a new bitcoin address for receiving payments. If [account] is specified (recommended), it is added to the address book so payments received with the address will be credited to [account]. 
        /// </summary>
        /// <param name="account">The account to add the new address to.</param>
        /// <returns>The new address.</returns>
        public string GetNewAddress(string account = "")
        {
            return MakeRequest<string>("getnewaddress", account);
        }

        /// <summary>
        /// Version 0.7: Returns data about each connected node. 
        /// </summary>
        /// <returns>A list of objects containing information about connected nodes.</returns>
        public IList<PeerInfo> GetPeerInfo()
        {
            return MakeRequest<IList<PeerInfo>>("getpeerinfo", null);
        }

        /// <summary>
        /// recent git checkouts only Returns a new Bitcoin address, for receiving change. This is for use with raw transactions, NOT normal use.
        /// </summary>
        public void GetRawChangeAddress()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Version 0.7: Returns all transaction ids in memory pool.
        /// </summary>
        /// <returns>A list of transaction ids in the memory pool.</returns>
        public List<string> GetRawMemPool()
        {
            return MakeRequest<List<string>>("getrawmempool", null);
        }

        /// <summary>
        /// Version 0.7: Returns raw transaction representation for given transaction id. 
        /// </summary>
        /// <param name="txId">The transaction id.</param>
        /// <param name="verbose">
        /// The verbosity level. If it is higher than 0, 
        /// a lot more information will be returned.
        /// </param>
        /// <returns>The raw transaction hex.</returns>
        public DecodedRawTransaction GetRawTransaction(string txId, int verbose = 0)
        {
            if (verbose == 0)
            {
                var hex = MakeRequest<string>("getrawtransaction", txId, verbose);
                return new DecodedRawTransaction { Hex = hex };
            }

            return MakeRequest<DecodedRawTransaction>("getrawtransaction", txId, verbose);
        }

        /// <summary>
        /// Returns the total amount received by addresses with [account] in transactions 
        /// with at least [minconf] confirmations. If [account] not provided return will 
        /// include all transactions to all accounts. (version 0.3.24)  
        /// </summary>
        /// <param name="account">The account to get the balance for.</param>
        /// <param name="minconf">Minimum amount of confirmations before a transaction is included.</param>
        /// <returns>The total amount received by the address with the given account.</returns>
        public decimal GetReceivedByAccount(string account = "", int minconf = 1)
        {
            return MakeRequest<decimal>("getreceivedbyaccount ", account, minconf);
        }

        /// <summary>
        /// Returns the total amount received by bitcoinaddress in transactions with at 
        /// least [minconf] confirmations. While some might consider this obvious, value 
        /// reported by this only considers *receiving* transactions. It does not check 
        /// payments that have been made *from* this address. In other words, this is 
        /// not "getaddressbalance". Works only for addresses in the local wallet, 
        /// external addresses will always show 0. 
        /// </summary>
        /// <param name="bitcoinAddress">The address to get the balance for.</param>
        /// <param name="minconf">Minimum amount of confirmations before a transaction is included.</param>
        /// <returns>The total amount received by the bitcoinaddress.</returns>
        public decimal GetReceivedByAddress(string bitcoinAddress, int minconf = 1)
        {
            return MakeRequest<decimal>("getreceivedbyaddress", bitcoinAddress, minconf);
        }

        /// <summary>
        /// Returns an object about the given transaction containing: 
        /// amount : total amount of the transaction 
        /// confirmations : number of confirmations of the transaction 
        /// txid : the transaction ID 
        /// time : time associated with the transaction[1]. 
        /// details - An array of objects containing: 
        ///  - account 
        ///  - address 
        ///  - category 
        ///  - amount 
        ///  - fee
        /// </summary>
        /// <param name="txId">The transaction id.</param>
        /// <returns>An object containg transaction information.</returns>
        public Transaction GetTransaction(string txId)
        {
            return MakeRequest<Transaction>("gettransaction", txId);
        }

        /// <summary>
        /// Returns details about an unspent transaction output (UTXO).
        /// </summary>
        /// <param name="txId">The transaction id.</param>
        /// <param name="n">The 'n' value of the out transaction we want to see.</param>
        /// <param name="includeMemPool">Include unspent transaction in the memory pool.</param>
        /// <returns>An object containg transaction information.</returns>
        public Transaction GetTxOut(string txId, long n, bool includeMemPool = true)
        {
            return MakeRequest<Transaction>("gettxout", txId, n, includeMemPool);
        }

        /// <summary>
        /// Returns statistics about the unspent transaction output (UTXO) set. 
        /// </summary>
        /// <returns>An object containing information about the unspent tx set.</returns>
        public TxOutSetInfo GetTxOutSetInfo()
        {
            return MakeRequest<TxOutSetInfo>("gettxoutsetinfo ", null);
        }

        /// <summary>
        /// If [data] is not specified, returns formatted hash data to work on:
        /// "midstate" : precomputed hash state after hashing the first half of the data
        /// "data" : block data
        /// "hash1" : formatted hash buffer for second hash
        /// "target" : little endian hash target
        /// If [data] is specified, tries to solve the block and returns true if it was successful.
        /// </summary>
        public Getwork Getwork()
        {
            return MakeRequest<Getwork>("getwork", null);   
        }

        /// <summary>
        /// The Getwork Completion response from the Bitcoin daemon is very simply a true or false value. If the value is true, then the hash was accepted and a new block has been created! If the value is false then the hash was rejected. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Getwork(string data)
        {
            return MakeRequest<bool>("getwork", data);
        }

        /// <summary>
        /// Adds a private key (as returned by dumpprivkey) to your wallet. This 
        /// may take a while, as a rescan is done, looking for existing transactions. 
        /// Optional [rescan] parameter added in 0.8.0.
        /// </summary>
        /// <param name="bitcoinPrivKey">Private key to import.</param>
        /// <param name="label"></param>
        /// <param name="rescan">Should a rescan be performed?</param>
        public void ImportPrivKey(string bitcoinPrivKey, string label = "", bool rescan = true)
        {
            MakeRequest<string>("importprivkey", bitcoinPrivKey, label, rescan);
        }

        /// <summary>
        /// Fills the keypool, requires wallet passphrase to be set.
        /// </summary>
        public void KeyPoolRefill()
        {
            MakeRequest<string>("keypoolrefill", null);
        }

        /// <summary>
        /// Returns Object that has account names as keys, account balances as values. 
        /// </summary>
        /// <returns>A dictionary with account names as keys and account balances as values.</returns>
        public Dictionary<string, decimal> ListAccounts()
        {
            return MakeRequest<Dictionary<string, decimal>>("listaccounts", null);
        }

        /// <summary>
        /// Version 0.7: Returns all addresses in the wallet and info used for coincontrol.
        /// </summary>
        /// <returns>A list of address groupings. Each grouping contains an address with its balance.</returns>
        public List<List<List<string>>> ListAddressGroupings()
        {
            return MakeRequest<List<List<List<string>>>>("listaddressgroupings", null);
        }

        /// <summary>
        /// Returns an array of objects containing: 
        /// account : the account of the receiving addresses 
        /// amount : total amount received by addresses with this account 
        /// confirmations : number of confirmations of the most recent transaction included
        /// </summary>
        /// <param name="minconf">Minimum number of confirmations before the transaction is included in the result.</param>
        /// <param name="includeEmpty">Include empty accounts?</param>
        /// <returns>A list of balances per account.</returns>
        public List<ListReceivedByAccountTransaction> ListReceivedByAccount(int minconf = 1, bool includeEmpty = false)
        {
            return MakeRequest<List<ListReceivedByAccountTransaction>>("listreceivedbyaccount", minconf, includeEmpty);
        }

        /// <summary>
        /// Returns an array of objects containing: 
        /// address : receiving address 
        /// account : the account of the receiving address 
        /// amount : total amount received by the address 
        /// confirmations : number of confirmations of the most recent transaction included 
        /// To get a list of accounts on the system, execute bitcoind listreceivedbyaddress 0 true
        /// </summary>
        /// <param name="minconf">Minimum number of confirmations before the transaction is included in the result.</param>
        /// <param name="includeEmpty">Include empty address.</param>
        /// <returns>A list of balances per address.</returns>
        public List<ListReceivedByAddressTransaction> ListReceivedByAddress(int minconf = 1, bool includeEmpty = false)
        {
            return MakeRequest<List<ListReceivedByAddressTransaction>>("listreceivedbyaddress", minconf, includeEmpty);
        }

        /// <summary>
        /// Get all transactions in blocks since block [blockhash], or all transactions if omitted.
        /// </summary>
        /// <param name="blockHash">The block hash.</param>
        /// <param name="targetConfirmations">Minimum amount of confirmations, 1 or higher is required as a value.</param>
        /// <returns>An object containing the transactions since the given block.</returns>
        public TransactionsSinceBlock ListSinceBlock(string blockHash, int targetConfirmations)
        {
            return MakeRequest<TransactionsSinceBlock>("listsinceblock", blockHash, targetConfirmations);
        }

        /// <summary>
        /// Returns up to [count] most recent transactions skipping the first [from] 
        /// transactions for account [account]. If [account] not provided will return 
        /// recent transaction from all accounts.
        /// </summary>
        /// <param name="account">The account to list transactions for.</param>
        /// <param name="count">Number of transactions to list.</param>
        /// <param name="from">Starting point to list transactions from.</param>
        /// <returns>A list of transactions.</returns>
        public List<ListTransaction> ListTransactions(string account = "", int count = 10, int from = 0)
        {
            return MakeRequest<List<ListTransaction>>("listtransactions", account, count, from);
        }

        /// <summary>
        /// Version 0.7: Returns array of unspent transaction inputs in the wallet.
        /// </summary>
        /// <param name="minConf">Minimum amount of confirmations needed before transaction is included.</param>
        /// <param name="maxConf">Maximum amount of confirmations allowed to include transaction.</param>
        /// <returns>A list of unspent transactions.</returns>
        public List<UnspentTransaction> ListUnspent(int minConf = 1, int maxConf = 999999)
        {
            return MakeRequest<List<UnspentTransaction>>("listunspent", minConf, maxConf);
        }

        /// <summary>
        /// version 0.8 Returns list of temporarily unspendable outputs
        /// </summary>
        public void ListLockUnspent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// version 0.8 Updates list of temporarily unspendable outputs
        /// </summary>
        public void LockUnspent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move from one account in your wallet to another.
        /// </summary>
        /// <param name="fromAccount">From account.</param>
        /// <param name="toAccount">To account.</param>
        /// <param name="amount">Amount to move.</param>
        /// <param name="minConf"></param>
        /// <param name="comment">Comment to go with the transaction.</param>
        /// <returns>True if succesfull.</returns>
        public bool Move(string fromAccount, string toAccount, decimal amount, int minConf = 1, string comment = "")
        {
            return MakeRequest<bool>("move", fromAccount, toAccount, amount, minConf, comment);
        }

        /// <summary>
        /// Amount is a real and is rounded to 8 decimal places. Will send the given amount to 
        /// the given address, ensuring the account has a valid balance using [minconf] 
        /// confirmations. Returns the transaction ID if successful (not in JSON object).
        /// </summary>
        /// <param name="fromAccount">From account.</param>
        /// <param name="toBitcoinAddress">Bitcoin address to move bitcoins to.</param>
        /// <param name="amount">Amount to move.</param>
        /// <param name="minConf"></param>
        /// <param name="comment">Comment to go with the transaction.</param>
        /// <param name="commentTo">Comment in the 'to' field.</param>
        /// <returns>The transaction ID if succesful.</returns>
        public string SendFrom(string fromAccount, string toBitcoinAddress, decimal amount, int minConf = 1, string comment = "", string commentTo = "")
        {
            return MakeRequest<string>("sendfrom", fromAccount, toBitcoinAddress, amount, minConf, comment, commentTo);
        }

        /// <summary>
        /// Send bitcoins to multiple addresses in one transaction. 
        /// Amounts are double-precision floating point numbers.
        /// </summary>
        /// <param name="fromAccount">The account to send the bitcoins from.</param>
        /// <param name="toBitcoinAddress">A dictionary of address to send bitcoins to. The key is the address, the value is the amount of bitcoins to send.</param>
        /// <param name="minConf"></param>
        /// <param name="comment">A comment to provide with the transaction.</param>
        /// <returns>The transaction ID if succesful.</returns>
        public string SendMany(string fromAccount, Dictionary<string, decimal> toBitcoinAddress, int minConf = 1, string comment = "")
        {
            return MakeRequest<string>("sendmany", fromAccount, toBitcoinAddress, minConf, comment);
        }

        /// <summary>
        /// Version 0.7: Submits raw transaction (serialized, hex-encoded) to local node and network.
        /// </summary>
        /// <returns></returns>
        public string SentRawTransaction(string hexString)
        {
            return MakeRequest<string>("sendrawtransaction", hexString);
        }

        /// <summary>
        /// Amount is a real and is rounded to 8 decimal places. Returns the transaction ID txid if successful.
        /// </summary>
        /// <param name="bitcoinAddress">Bitcoin address to sent to.</param>
        /// <param name="amount">Amount to send.</param>
        /// <param name="comment">Comment to go with the transaction.</param>
        /// <param name="commentTo">Comment for the 'to' field.</param>
        /// <returns>The transaction ID if succesful.</returns>
        public string SendToAddress(string bitcoinAddress, decimal amount, string comment, string commentTo)
        {
            return MakeRequest<string>("sendtoaddress", bitcoinAddress, amount, comment, commentTo);
        }

        /// <summary>
        /// Sets the account associated with the given address. Assigning 
        /// address that is already assigned to the same account will 
        /// create a new address associated with that account.
        /// </summary>
        /// <param name="bitcoinAddress">The bitcoin address to assocaite.</param>
        /// <param name="account">The account to associate with.</param>
        public void SetAccount(string bitcoinAddress, string account)
        {
            MakeRequest<string>("setaccount", bitcoinAddress, account);
        }

        /// <summary>
        /// Generate is true or false to turn generation on or off. Generation is 
        /// limited to [genproclimit] processors, -1 is unlimited.
        /// </summary>
        /// <param name="generate">True to turn generation on.</param>
        /// <param name="genProcLimit">Number of processors to use, -1 is unlimited.</param>
        public void SetGenerate(bool generate, int genProcLimit = -1)
        {
            MakeRequest<string>("setgenerate", generate, genProcLimit);
        }

        /// <summary>
        /// Amount is a real and is rounded to the nearest 0.00000001.
        /// </summary>
        /// <param name="amount">Amount of transaction fee.</param>
        /// <returns>True if succesfull.</returns>
        public bool SetTxFee(decimal amount)
        {
            return MakeRequest<bool>("settxfee", amount);
        }

        /// <summary>
        /// Sign a message with the private key of an address.
        /// </summary>
        /// <param name="bitcoinAddress">Address who's private key is used to sign the message.</param>
        /// <param name="message">The message to sign.</param>
        /// <returns>The signed message.</returns>
        public string SignMessage(string bitcoinAddress, string message)
        {
            return MakeRequest<string>("signmessage", bitcoinAddress, message);
        }

        /// <summary>
        /// Version 0.7: Adds signatures to a raw transaction and returns the resulting raw transaction. 
        /// </summary>
        /// <param name="rawTransaction">The raw transaction to sign.</param>
        /// <returns>The signed transaction.</returns>
        public SignedRawTransaction SignRawTransaction(SignRawTransaction rawTransaction)
        {
            var hex = rawTransaction.RawTransactionHex;
            var inputs = rawTransaction.Inputs.Any() ? rawTransaction.Inputs : null;
            var privateKeys = rawTransaction.PrivateKeys.Any() ? rawTransaction.PrivateKeys : null;

            return MakeRequest<SignedRawTransaction>("signrawtransaction", hex, inputs, privateKeys);
        }

        /// <summary>
        /// Stop bitcoin server.
        /// </summary>
        /// <returns>A message detailing the bitcoin server is stopping.</returns>
        public string Stop()
        {
            return MakeRequest<string>("stop", null);
        }
       

        /// <summary>
        /// Return information about the bitcoin address. 
        /// </summary>
        /// <param name="walletAddress">The bitcoin address.</param>
        /// <returns></returns>
        public ValidateAddress ValidateAddress(string walletAddress)
        {
            return MakeRequest<ValidateAddress>("validateaddress", walletAddress);
        }

        /// <summary>
        /// Verify a signed message.
        /// </summary>
        /// <param name="bitcoinAddress">The bitcoin address who's public key is used to verify the message.</param>
        /// <param name="signature">The provided signature.</param>
        /// <param name="message">The signed message.</param>
        /// <returns>True if the message has been signed by the bitcoinaddress.</returns>
        public bool VerifyMessage(string bitcoinAddress, string signature, string message)
        {
            return MakeRequest<bool>("verifymessage", bitcoinAddress, signature, message);
        }

        /// <summary>
        /// Removes the wallet encryption key from memory, locking the wallet. 
        /// After calling this method, you will need to call walletpassphrase 
        /// again before being able to call any methods which require the wallet 
        /// to be unlocked.
        /// </summary>
        public void WalletLock()
        {
            MakeRequest<string>("walletlock", null);
        }

        /// <summary>
        /// Stores the wallet decryption key in memory for timeout seconds. This essentialy
        /// unlocks the wallet for the given time. During this time more commands are 
        /// allowed to be invoked (like transferring bitcoins).
        /// </summary>
        /// <param name="passphrase">Pass phrase to use.</param>
        /// <param name="timeout">The timeout in seconds.</param>
        public void WalletPassPhrase(string passphrase, int timeout)
        {
            MakeRequest<string>("walletpassphrase", passphrase, timeout);
        }

        /// <summary>
        /// Changes the wallet passphrase from oldpassphrase to newpassphrase.
        /// </summary>
        /// <param name="oldpassphrase">Old pass phrase.</param>
        /// <param name="newpassphrase">The new pass phrase.</param>
        public void WalletPassPhraseChange(string oldpassphrase, string newpassphrase)
        {
            MakeRequest<string>("walletpassphrasechange", oldpassphrase, newpassphrase);
        }
    }
}
