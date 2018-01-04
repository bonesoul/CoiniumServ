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
