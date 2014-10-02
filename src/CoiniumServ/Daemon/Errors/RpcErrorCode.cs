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
namespace CoiniumServ.Daemon.Errors
{
    // defined in https://github.com/bitcoin/bitcoin/blob/master/src/rpcprotocol.h#L34

    public enum RpcErrorCode : int
    {
        // ReSharper disable InconsistentNaming

        // Standard JSON-RPC 2.0 errors
        RPC_INVALID_REQUEST = -32600,
        RPC_METHOD_NOT_FOUND = -32601,
        RPC_INVALID_PARAMS = -32602,
        RPC_INTERNAL_ERROR = -32603,
        RPC_PARSE_ERROR = -32700,

        // General application defined errors
        RPC_MISC_ERROR = -1,  // std::exception thrown in command handling
        RPC_FORBIDDEN_BY_SAFE_MODE = -2,  // Server is in safe mode, and command is not allowed in safe mode
        RPC_TYPE_ERROR = -3,  // Unexpected type was passed as parameter
        RPC_INVALID_ADDRESS_OR_KEY = -5,  // Invalid address or key
        RPC_OUT_OF_MEMORY = -7,  // Ran out of memory during operation
        RPC_INVALID_PARAMETER = -8,  // Invalid, missing or duplicate parameter
        RPC_DATABASE_ERROR = -20, // Database error
        RPC_DESERIALIZATION_ERROR = -22, // Error parsing or validating structure in raw format
        RPC_TRANSACTION_ERROR = -25, // General error during transaction submission
        RPC_TRANSACTION_REJECTED = -26, // Transaction was rejected by network rules
        RPC_TRANSACTION_ALREADY_IN_CHAIN = -27, // Transaction already in chain

        // P2P client errors
        RPC_CLIENT_NOT_CONNECTED = -9,  // Bitcoin is not connected
        RPC_CLIENT_IN_INITIAL_DOWNLOAD = -10, // Still downloading initial blocks
        RPC_CLIENT_NODE_ALREADY_ADDED = -23, // Node is already added
        RPC_CLIENT_NODE_NOT_ADDED = -24, // Node has not been added before

        // Wallet errors
        RPC_WALLET_ERROR = -4,  // Unspecified problem with wallet (key not found etc.)
        RPC_WALLET_INSUFFICIENT_FUNDS = -6,  // Not enough funds in wallet or account
        RPC_WALLET_INVALID_ACCOUNT_NAME = -11, // Invalid account name
        RPC_WALLET_KEYPOOL_RAN_OUT = -12, // Keypool ran out, call keypoolrefill first
        RPC_WALLET_UNLOCK_NEEDED = -13, // Enter the wallet passphrase with walletpassphrase first
        RPC_WALLET_PASSPHRASE_INCORRECT = -14, // The wallet passphrase entered was incorrect
        RPC_WALLET_WRONG_ENC_STATE = -15, // Command given in wrong wallet encryption state (encrypting an encrypted wallet etc.)
        RPC_WALLET_ENCRYPTION_FAILED = -16, // Failed to encrypt the wallet
        RPC_WALLET_ALREADY_UNLOCKED = -17, // Wallet is already unlocked

        // ReSharper restore InconsistentNaming
    }
}
