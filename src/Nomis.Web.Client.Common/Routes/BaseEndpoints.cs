namespace Nomis.Web.Client.Common.Routes
{
    /// <summary>
    /// Base endpoints.
    /// </summary>
    public abstract class BaseEndpoints
    {
        private readonly string _baseUrl;

        /// <summary>
        /// Blockchain.
        /// </summary>
        public abstract string Blockchain { get; }

        /// <summary>
        /// Initialize class inherited from <see cref="BaseEndpoints"/>.
        /// </summary>
        /// <param name="baseUrl">Base URL for endpoint.</param>
        protected BaseEndpoints(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Get endpoint for Nomis score.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        public virtual string GetWalletScore(string address) => $"{_baseUrl}/api/v1/{Blockchain.ToLower()}/wallet/{address}/score";
    }
}