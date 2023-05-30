// ------------------------------------------------------------------------------------------------------
// <copyright file="TokenDataBalance.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;
using System.Text.Json.Serialization;

namespace Nomis.Blockchain.Abstractions.Contracts
{
    /// <summary>
    /// Token data balance.
    /// </summary>
    public class TokenDataBalance :
        TokenData
    {
        /// <summary>
        /// Balance.
        /// </summary>
        [JsonIgnore]
        public BigInteger Balance { get; set; }

        /// <summary>
        /// Balance amount.
        /// </summary>
        public decimal Amount
        {
            get
            {
                int.TryParse(Decimals, out int decimals);
                var realAmount = Balance;
                for (int i = 0; i < decimals; i++)
                {
                    realAmount /= 10;
                }

                if (realAmount <= new BigInteger(decimal.MaxValue))
                {
                    return (decimal)realAmount;
                }

                return 0;
            }
        }

        /// <summary>
        /// Price.
        /// </summary>
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// Last price date and time.
        /// </summary>
        public DateTime? LastPriceDateTime { get; set; }

        /// <summary>
        /// Confidence.
        /// </summary>
        public decimal Confidence { get; set; } = 0;

        /// <summary>
        /// Total token balance amount price.
        /// </summary>
        public decimal TotalAmountPrice => Price * Amount;
    }
}