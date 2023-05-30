﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="ITallySettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

namespace Nomis.Tally.Interfaces.Settings
{
    /// <summary>
    /// Tally settings.
    /// </summary>
    public interface ITallySettings
    {
        /// <summary>
        /// Supported blockchain ids.
        /// </summary>
        public IList<ulong> SupportedChainIds { get; set; }
    }
}