// ------------------------------------------------------------------------------------------------------
// <copyright file="ScoringCalculationModel.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming

using System.ComponentModel;

namespace Nomis.Utils.Enums
{
    /// <summary>
    /// Scoring calculation model.
    /// </summary>
    public enum ScoringCalculationModel :
        ushort
    {
        /// <summary>
        /// Common V1.
        /// </summary>
        [Description("Common V1 scoring calculation model")]
        CommonV1 = 0,

        /// <summary>
        /// Symbiosis.
        /// </summary>
        [Description("Scoring calculation model for Symbosis Finance")]
        Symbiosis,

        /// <summary>
        /// XDEFI.
        /// </summary>
        [Description("Scoring calculation model for XDEFI wallet")]
        XDEFI,

        /// <summary>
        /// Halo.
        /// </summary>
        [Description("Scoring calculation model for Halo wallet")]
        Halo,

        /// <summary>
        /// Common V2.
        /// </summary>
        [Description("Common V2 scoring calculation model")]
        CommonV2
    }
}