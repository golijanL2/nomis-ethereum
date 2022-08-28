using Nomis.Blockchain.Abstractions.Enums;
using Nomis.Utils.Contracts.Common;

namespace Nomis.Etherscan.Interfaces.Settings
{
    /// <summary>
    /// Etherscan settings.
    /// </summary>
    public class EtherscanSettings :
        ISettings
    {
        /// <summary>
        /// API key for etherscan.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Blockchain network.
        /// </summary>
        public BlockchainNetwork Network { get; set; } = BlockchainNetwork.Ethereum;

        /// <summary>
        /// Blockchain provider URL.
        /// </summary>
        public string? BlockchainProviderUrl { get; set; }
    }
}