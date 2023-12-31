﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="HapiExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Api.Common.Extensions;
using Nomis.Api.Hapi.Settings;
using Nomis.HapiExplorer.Interfaces;
using Nomis.ScoringService.Interfaces.Builder;

namespace Nomis.Api.Hapi.Extensions
{
    /// <summary>
    /// Hapi extension methods.
    /// </summary>
    public static class HapiExtensions
    {
        /// <summary>
        /// Add HAPI protocol.
        /// </summary>
        /// <typeparam name="TServiceRegistrar">The service registrar type.</typeparam>
        /// <param name="optionsBuilder"><see cref="IScoringOptionsBuilder"/>.</param>
        /// <returns>Returns <see cref="IScoringOptionsBuilder"/>.</returns>
        // ReSharper disable once InconsistentNaming
        public static IScoringOptionsBuilder WithHAPIProtocol<TServiceRegistrar>(
            this IScoringOptionsBuilder optionsBuilder)
            where TServiceRegistrar : IHapiServiceRegistrar, new()
        {
            return optionsBuilder
                .With<HapiAPISettings, TServiceRegistrar>();
        }
    }
}