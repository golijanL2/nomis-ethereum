// ------------------------------------------------------------------------------------------------------
// <copyright file="WalletStatsRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Nomis.Utils.Contracts.Properties;
using Nomis.Utils.Enums;

namespace Nomis.Utils.Contracts.Requests
{
    /// <summary>
    /// Request for getting the wallet stats.
    /// </summary>
    public class WalletStatsRequest :
        IHasAddress
    {
        private string _address = string.Empty;
        private string? _tokenAddress;

        /// <summary>
        /// Wallet address.
        /// </summary>
        [FromRoute(Name = "address")]
        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                _address = value.Trim();
            }
        }

        /// <summary>
        /// Contract nonce.
        /// </summary>
        /// <remarks>
        /// Should be get from the contract.
        /// </remarks>
        /// <example>0</example>
        [FromQuery(Name = "nonce")]
        public ulong Nonce { get; set; }

        /// <summary>
        /// Verifying deadline block timestamp.
        /// </summary>
        /// <example>133160867380732039</example>
        [FromQuery(Name = "deadline")]
        public ulong Deadline { get; set; }

        /// <summary>
        /// Scoring calculation model.
        /// </summary>
        /// <example>0</example>
        [FromQuery(Name = "calculationModel")]
        public ScoringCalculationModel CalculationModel { get; set; } = ScoringCalculationModel.CommonV1;

        /// <summary>
        /// Score type.
        /// </summary>
        /// <example>0</example>
        [FromQuery(Name = "scoreType")]
        public ScoreType ScoreType { get; set; } = ScoreType.Finance;

        /// <summary>
        /// Token contract address.
        /// </summary>
        /// <remarks>
        /// If `ScoreType = 1` and set, scoring calculate for this token.
        /// </remarks>
        /// <example>null</example>
        [FromQuery(Name = "tokenAddress")]
        public string? TokenAddress
        {
            get
            {
                return _tokenAddress;
            }

            set
            {
                _tokenAddress = value?.Trim();
            }
        }

        /// <summary>
        /// Prepare data to mint.
        /// </summary>
        /// <example>true</example>
        [FromQuery(Name = "prepareToMint")]
        public bool PrepareToMint { get; set; } = true;

        /// <summary>
        /// Blockchain in which the score will be minted.
        /// </summary>
        /// <example>0</example>
        [FromQuery]
        public MintChain MintChain { get; set; } = MintChain.Native;

        /// <summary>
        /// Blockchain type in which the score will be minted.
        /// </summary>
        /// <remarks>
        /// Uses only if `MintChain = 0 (Native)`.
        /// </remarks>
        /// <example>0</example>
        [FromQuery(Name = "mintBlockchainType")]
        public MintChainType MintBlockchainType { get; set; } = MintChainType.Mainnet;
    }
}