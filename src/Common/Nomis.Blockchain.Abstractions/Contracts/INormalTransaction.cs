// ------------------------------------------------------------------------------------------------------
// <copyright file="INormalTransaction.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts.Properties;

namespace Nomis.Blockchain.Abstractions.Contracts
{
    /// <summary>
    /// Normal transaction.
    /// </summary>
    public interface INormalTransaction :
        IHasContractAddress,
        IHasHash,
        IHasTo
    {
    }
}