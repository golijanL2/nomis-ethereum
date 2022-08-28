using Microsoft.Extensions.Options;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Nomis.Web.Client.Common.Extensions;
using Nomis.Web.Client.Common.Settings;
using Nomis.Web.Client.Ethereum.Routes;

namespace Nomis.Web.Client.Ethereum.Managers
{
    /// <inheritdoc cref="IEthereumManager" />
    public class EthereumManager :
        IEthereumManager
    {
        private readonly HttpClient _httpClient;
        private readonly EthereumEndpoints _endpoints;

        /// <summary>
        /// Initialize <see cref="EthereumManager"/>.
        /// </summary>
        /// <param name="webApiSettings"><see cref="WebApiSettings"/>.</param>
        public EthereumManager(
            IOptions<WebApiSettings> webApiSettings)
        {
            _httpClient = new()
            {
                BaseAddress = new(webApiSettings.Value?.ApiBaseUrl ?? throw new ArgumentNullException(nameof(webApiSettings.Value.ApiBaseUrl)))
            };
            _endpoints = new(webApiSettings.Value?.ApiBaseUrl ?? throw new ArgumentNullException(nameof(webApiSettings.Value.ApiBaseUrl)));
        }

        /// <inheritdoc />
        public async Task<IResult<EthereumWalletScore>> GetWalletScoreAsync(string address)
        {
            var response = await _httpClient.GetAsync(_endpoints.GetWalletScore(address));
            return await response.ToResultAsync<EthereumWalletScore>();
        }
    }
}