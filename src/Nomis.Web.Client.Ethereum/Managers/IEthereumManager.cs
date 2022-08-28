using Nomis.Etherscan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Nomis.Web.Client.Common.Managers;

namespace Nomis.Web.Client.Ethereum.Managers
{
    /// <summary>
    /// Ethereum manager.
    /// </summary>
    public interface IEthereumManager :
        IManager
    {
        /// <summary>
        /// Get ethereum wallet score.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <returns>Returns result of <see cref="EthereumWalletScore"/>.</returns>
        Task<IResult<EthereumWalletScore>> GetWalletScoreAsync(string address);
    }
}