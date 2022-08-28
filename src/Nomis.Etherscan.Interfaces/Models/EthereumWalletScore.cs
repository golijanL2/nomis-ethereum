namespace Nomis.Etherscan.Interfaces.Models
{
    /// <summary>
    /// Ethereum wallet score.
    /// </summary>
    public class EthereumWalletScore
    {
        /// <summary>
        /// Nomis Score in range of [0; 1].
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Additional stat data used in score calculations.
        /// </summary>
        public EthereumWalletStats? Stats { get; set; }
    }
}