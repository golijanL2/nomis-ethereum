using EthScanNet.Lib;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Wrapper;

namespace Nomis.Etherscan.Interfaces
{
    /// <summary>
    /// Etherscan service.
    /// </summary>
    public interface IEtherscanService :
        IInfrastructureService
    {
        /// <summary>
        /// Client for interacting with Etherscan API.
        /// </summary>
        public EScanClient Client { get; }

        /// <summary>
        /// Get ethereum wallet stats by address or ENS name.
        /// </summary>
        /// <param name="address">Ethereum wallet address.</param>
        /// <returns>Returns <see cref="EthereumWalletScore"/> result.</returns>
        public Task<Result<EthereumWalletScore>> GetWalletStatsAsync(string address);
    }
}