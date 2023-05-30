// ------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions;
using Nomis.Blockchain.Abstractions.Enums;
using Nomis.Utils.Contracts.Requests;

namespace Nomis.DexProviderService.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get the blockchain descriptor in which the score will be minted.
        /// </summary>
        /// <typeparam name="TWalletRequest">The wallet request type.</typeparam>
        /// <param name="service"><see cref="IDexProviderService"/>.</param>
        /// <param name="request"><see cref="WalletStatsRequest"/>.</param>
        /// <returns>Returns the blockchain descriptor in which the score will be minted.</returns>
        public static IBlockchainDescriptor? MintChain<TWalletRequest>(
            this IDexProviderService service,
            TWalletRequest request)
            where TWalletRequest : WalletStatsRequest
        {
            if (request.MintChain == Utils.Enums.MintChain.Native)
            {
                return null;
            }

            var supportedBlockchains = service.Blockchains(BlockchainType.EVM, true);
            var mintBlockchain = supportedBlockchains.Data.FirstOrDefault(b => b.ChainId == (ulong)request.MintChain);
            if (mintBlockchain == null)
            {
                throw new NotSupportedException($"{request.MintChain} blockchain does not supported or disabled.");
            }

            return mintBlockchain;
        }
    }
}