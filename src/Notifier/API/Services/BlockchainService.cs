using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Notifier.API.Services;

public class BlockchainService
{
    private readonly Web3 _web3;
    private readonly string _contractAddress;
    private const string AUDIT_ABI = @"[
        {
            ""inputs"": [
                { ""internalType"": ""string"", ""name"": ""taskId"", ""type"": ""string"" },
                { ""internalType"": ""string"", ""name"": ""outputHash"", ""type"": ""string"" }
            ],
            ""name"": ""logTask"",
            ""outputs"": [],
            ""stateMutability"": ""nonpayable"",
            ""type"": ""function""
        }
    ]";

    public BlockchainService()
    {
        var privateKey = Environment.GetEnvironmentVariable("ETH_PRIVATE_KEY")!;
        var rpcUrl = Environment.GetEnvironmentVariable("ALCHEMY_RPC_URL")!;
        _contractAddress = Environment.GetEnvironmentVariable("AUDIT_CONTRACT_ADDRESS")!;

        var account = new Account(privateKey, chainId: 11155111);
        _web3 = new Web3(account, rpcUrl);
    }

    public async Task<string> LogTaskHashAsync(string taskId, string outputHash)
    {
        var contract = _web3.Eth.GetContract(AUDIT_ABI, _contractAddress);
        var logFunction = contract.GetFunction("logTask");

        var txHash = await logFunction.SendTransactionAsync(
            _web3.TransactionManager.Account.Address,
            new Nethereum.Hex.HexTypes.HexBigInteger(200000),
            null,
            taskId,
            outputHash
        );

        return txHash;
    }
}