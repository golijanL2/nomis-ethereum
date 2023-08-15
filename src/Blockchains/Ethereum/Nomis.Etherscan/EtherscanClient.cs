// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanClient.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net.Http.Json;

using Microsoft.Extensions.Logging;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Etherscan.Settings;
using Nomis.Utils.Attributes.Logging;
using Nomis.Utils.Contracts;
using Nomis.Utils.Exceptions;

namespace Nomis.Etherscan
{
    /// <inheritdoc cref="IEtherscanClient"/>
    internal sealed class EtherscanClient :
        IEtherscanClient
    {
        private const int ItemsFetchLimit = 10000;
        private readonly EtherscanSettings _etherscanSettings;
        private readonly HttpClient _client;
        private readonly string _apiKey;

        /// <summary>
        /// Initialize <see cref="EtherscanClient"/>.
        /// </summary>
        /// <param name="etherscanSettings"><see cref="EtherscanSettings"/>.</param>
        /// <param name="apiKeysPool"><see cref="IValuePool{TService,TValue}"/>.</param>
        /// <param name="client"><see cref="HttpClient"/>.</param>
        /// <param name="logger"><see cref="ILogger{TCategoryName}"/>.</param>
        public EtherscanClient(
            EtherscanSettings etherscanSettings,
            IValuePool<EtherscanService, string> apiKeysPool,
            HttpClient client,
            ILogger<EtherscanClient> logger)
        {
            _etherscanSettings = etherscanSettings;
            _apiKey = apiKeysPool.GetNextValue();
            var maskedAttribute = new LogMaskedAttribute(showFirst: 3, preserveLength: true);
            object? maskedApiKey = maskedAttribute.MaskValue(_apiKey);
            logger.LogDebug("Used {ApiKey} API key for {ChainId} chain ID.", maskedApiKey, 1);

            _client = client;
        }

        /// <inheritdoc/>
        public async Task<EtherscanAccount> GetBalanceAsync(string address)
        {
            await _etherscanSettings.WaitForRequestRateLimit().ConfigureAwait(false);
            var response = await _client.GetAsync($"/api?module=account&action=balance&address={address}&apiKey={_apiKey}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EtherscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account balance.");
        }

        /// <inheritdoc/>
        public async Task<EtherscanAccount> GetTokenBalanceAsync(string address, string contractAddress)
        {
            await _etherscanSettings.WaitForRequestRateLimit().ConfigureAwait(false);
            var response = await _client.GetAsync($"/api?module=account&action=tokenbalance&address={address}&contractaddress={contractAddress}&tag=latest&apiKey={_apiKey}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EtherscanAccount>().ConfigureAwait(false) ?? throw new CustomException("Can't get account token balance.");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TResultItem>> GetTransactionsAsync<TResult, TResultItem>(string address)
            where TResult : IEtherscanTransferList<TResultItem>
            where TResultItem : IEtherscanTransfer
        {
            var result = new List<TResultItem>();
            var transactionsData = await GetTransactionListAsync<TResult>(address).ConfigureAwait(false);
            result.AddRange(transactionsData.Data ?? new List<TResultItem>());
            while (transactionsData?.Data?.Count >= ItemsFetchLimit)
            {
                transactionsData = await GetTransactionListAsync<TResult>(address, transactionsData.Data.LastOrDefault()?.BlockNumber).ConfigureAwait(false);
                result.AddRange(transactionsData?.Data ?? new List<TResultItem>());
            }

            return result;
        }

        private async Task<TResult> GetTransactionListAsync<TResult>(
            string address,
            string? startBlock = null)
        {
            string request =
                $"/api?module=account&address={address}&sort=asc&apiKey={_apiKey}";

            if (typeof(TResult) == typeof(EtherscanAccountNormalTransactions))
            {
                request = $"{request}&action=txlist";
            }
            else if (typeof(TResult) == typeof(EtherscanAccountInternalTransactions))
            {
                request = $"{request}&action=txlistinternal";
            }
            else if (typeof(TResult) == typeof(EtherscanAccountERC20TokenEvents))
            {
                request = $"{request}&action=tokentx";
            }
            else if (typeof(TResult) == typeof(EtherscanAccountERC721TokenEvents))
            {
                request = $"{request}&action=tokennfttx";
            }
            else if (typeof(TResult) == typeof(EtherscanAccountERC1155TokenEvents))
            {
                request = $"{request}&action=token1155tx";
            }
            else
            {
                return default!;
            }

            if (!string.IsNullOrWhiteSpace(startBlock))
            {
                request = $"{request}&startblock={startBlock}";
            }

            await _etherscanSettings.WaitForRequestRateLimit().ConfigureAwait(false);
            var response = await _client.GetAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>().ConfigureAwait(false) ?? throw new CustomException("Can't get account transactions.");
        }
    }
}