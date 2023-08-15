// ------------------------------------------------------------------------------------------------------
// <copyright file="BalanceCheckerSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.BalanceChecker.Contracts;
using Nomis.Utils.Contracts.Common;

namespace Nomis.BalanceChecker.Settings
{
    /// <summary>
    /// Balance checker settings.
    /// </summary>
    internal class BalanceCheckerSettings :
        ISettings
    {
        /// <summary>
        /// List of Balance checker data feed.
        /// </summary>
        public List<BalanceCheckerDataFeed> DataFeeds { get; init; } = new();
    }
}