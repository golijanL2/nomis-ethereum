using Nomis.Web.Client.Common.Routes;

namespace Nomis.Web.Client.Ethereum.Routes
{
    /// <summary>
    /// Ethereum endpoints.
    /// </summary>
    public class EthereumEndpoints :
        BaseEndpoints
    {
        /// <summary>
        /// Initialize <see cref="EthereumEndpoints"/>.
        /// </summary>
        /// <param name="baseUrl">Ethereum API base URL.</param>
        public EthereumEndpoints(string baseUrl) 
            : base(baseUrl)
        {
        }

        /// <inheritdoc/>
        public override string Blockchain => "ethereum";
    }
}
