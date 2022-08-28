using System.Numerics;
using EthScanNet.Lib.Models.ApiResponses.Accounts.Models;

namespace Nomis.Etherscan.Extensions
{
    /// <summary>
    /// Extension methods for ethereum.
    /// </summary>
    public static class EthHelpers
    {
        /// <summary>
        /// Wei in one ETH.
        /// </summary>
        private const ulong WeiToEth = 1000000000000000000;

        /// <summary>
        /// Convert Wei value to Eth.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this BigInteger valueInWei)
        {
            return (decimal)valueInWei / WeiToEth;
        }

        /// <summary>
        /// Convert Wei value to Eth.
        /// </summary>
        /// <param name="valueInWei">Wei.</param>
        /// <returns>Returns total ETH.</returns>
        public static decimal ToEth(this decimal valueInWei)
        {
            return new BigInteger(valueInWei).ToEth();
        }

        /// <summary>
        /// Get token UID based on it ContractAddress and Id.
        /// </summary>
        /// <param name="token">Token info.</param>
        /// <returns>Returns token UID.</returns>
        public static string GetTokenUid(this EScanTokenTransferEvent token)
        {
            return token.ContractAddress + "_" + token.TokenId;
        }
    }
}