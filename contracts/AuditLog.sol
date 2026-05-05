// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

/**
 * @title IntelliFlowAuditLog
 * @notice Stores SHA-256 hashes of IntelliFlow task outputs on-chain.
 *         Deployed on Ethereum Sepolia testnet.
 *         Written by: Shamraiz (Module 4)
 */
contract IntelliFlowAuditLog {
    struct AuditEntry {
        bytes32 outputHash;   // SHA-256 hash of the report output
        uint256 timestamp;    // Block timestamp when recorded
        string  taskId;       // Internal task ID from the system
    }

    // taskId => AuditEntry
    mapping(string => AuditEntry) private entries;
    string[] private taskIds;

    address public owner;

    event TaskLogged(string indexed taskId, bytes32 outputHash, uint256 timestamp);

    modifier onlyOwner() {
        require(msg.sender == owner, "Not authorized");
        _;
    }

    constructor() {
        owner = msg.sender;
    }

    /**
     * @notice Log a completed task's output hash.
     * @param taskId    Internal task identifier.
     * @param outputHash SHA-256 hash of the final report (as bytes32).
     */
    function logTask(string calldata taskId, bytes32 outputHash) external onlyOwner {
        require(entries[taskId].timestamp == 0, "Task already logged");

        entries[taskId] = AuditEntry({
            outputHash: outputHash,
            timestamp:  block.timestamp,
            taskId:     taskId
        });
        taskIds.push(taskId);

        emit TaskLogged(taskId, outputHash, block.timestamp);
    }

    /**
     * @notice Retrieve audit record for a task.
     */
    function getEntry(string calldata taskId)
        external view
        returns (bytes32 outputHash, uint256 timestamp)
    {
        AuditEntry storage e = entries[taskId];
        require(e.timestamp != 0, "Task not found");
        return (e.outputHash, e.timestamp);
    }

    /**
     * @notice Returns total number of logged tasks.
     */
    function totalTasks() external view returns (uint256) {
        return taskIds.length;
    }
}
