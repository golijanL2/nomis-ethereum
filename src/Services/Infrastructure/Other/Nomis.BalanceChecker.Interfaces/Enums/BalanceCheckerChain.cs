// ------------------------------------------------------------------------------------------------------
// <copyright file="BalanceCheckerChain.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace Nomis.BalanceChecker.Interfaces.Enums
{
    /// <summary>
    /// Balance checker blockchain.
    /// </summary>
    public enum BalanceCheckerChain
    {
        /// <summary>
        /// Cronos Mainnet Beta.
        /// </summary>
        Cronos = 25,

        /// <summary>
        /// Binance Smart Chain Mainnet.
        /// </summary>
        BSC = 56,

        /// <summary>
        /// Gnosis.
        /// </summary>
        Gnosis = 100,

        /// <summary>
        /// Polygon Mainnet.
        /// </summary>
        Polygon = 137,

        /// <summary>
        /// Astar.
        /// </summary>
        Astar = 592,

        /// <summary>
        /// Metis Andromeda Mainnet.
        /// </summary>
        Metis = 1088,

        /// <summary>
        /// Moonbeam.
        /// </summary>
        Moonbeam = 1284,

        /// <summary>
        /// Moonriver.
        /// </summary>
        Moonriver = 1285,

        /// <summary>
        /// Arbitrum One.
        /// </summary>
        ArbitrumOne = 42161,

        /// <summary>
        /// Arbitrum Nova.
        /// </summary>
        ArbitrumNova = 42170,

        /// <summary>
        /// Celo Mainnet.
        /// </summary>
        Celo = 42220,

        /// <summary>
        /// Avalanche C-Chain.
        /// </summary>
        Avalanche = 43114
    }
}