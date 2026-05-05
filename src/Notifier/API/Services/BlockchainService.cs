// ============================================================
// Module 4: Blockchain Service — Ethereum Sepolia via Nethereum
// Owner: Shamraiz (02-131232-112)
//
// NuGet: dotnet add package Nethereum.Web3
//        dotnet add package Nethereum.Contracts
//
// Steps to deploy:
//   1. Get free Sepolia ETH from: https://sepoliafaucet.com/
//   2. Deploy AuditLog.sol via Remix IDE → copy contract address
//   3. Set AUDIT_CONTRACT_ADDRESS and ETH_PRIVATE_KEY in .env
//   4. Copy ABI from Remix into AUDIT_ABI below
// ============================================================
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;

public class BlockchainService
{
    private readonly Web3    _web3;
    private readonly string  _contractAddress;

    // ── Paste ABI from Remix IDE here after deploying AuditLog.sol ──
    private const string AUDIT_ABI = @"[
        {
            ""inputs"": [
                { ""internalType"": ""string"",  ""name"": ""taskId"",    ""type"": ""string""  },
                { ""internalType"": ""bytes32"",  ""name"": ""outputHash"",""type"": ""bytes32"" }
            ],
            ""name"": ""logTask"",
            ""outputs"": [],
            ""stateMutability"": ""nonpayable"",
            ""type"": ""function""
        }
    ]";

    public BlockchainService()
    {
        var rpcUrl     = Environment.GetEnvironmentVariable("ALCHEMY_RPC_URL")
            ?? throw new Exception("ALCHEMY_RPC_URL not set");
        var privateKey = Environment.GetEnvironmentVariable("ETH_PRIVATE_KEY")
            ?? throw new Exception("ETH_PRIVATE_KEY not set");
        _contractAddress = Environment.GetEnvironmentVariable("AUDIT_CONTRACT_ADDRESS")
            ?? throw new Exception("AUDIT_CONTRACT_ADDRESS not set");

        var account = new Account(privateKey, chainId: 11155111); // 11155111 = Sepolia
        _web3 = new Web3(account, rpcUrl);
    }

    /// <summary>
    /// Writes the SHA-256 hash of the task output to the Sepolia smart contract.
    /// Returns the transaction hash (0x...) for audit purposes.
    /// </summary>
    public async Task<string> LogTaskHashAsync(string taskId, string hexHash)
    {
        // Convert hex string to bytes32
        var hashBytes = hexHash.HexToByteArray();
        var bytes32   = new byte[32];
        Array.Copy(hashBytes, bytes32, Math.Min(hashBytes.Length, 32));

        var contract = _web3.Eth.GetContract(AUDIT_ABI, _contractAddress);
        var logTask  = contract.GetFunction("logTask");

        var txHash = await logTask.SendTransactionAsync(
            from: _web3.Eth.TransactionManager.Account.Address,
            gas:  new Nethereum.Hex.HexTypes.HexBigInteger(200000),
            value: null,
            functionInput: new object[] { taskId, bytes32 }
        );

        return txHash;
    }
}
