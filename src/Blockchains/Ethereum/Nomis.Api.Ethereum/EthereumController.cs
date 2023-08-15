// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumController.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nomis.Api.Common.Swagger.Examples;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Etherscan.Interfaces.Requests;
using Nomis.Utils.Enums;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Ethereum
{
    /// <summary>
    /// A controller to aggregate all Ethereum-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Ethereum blockchain.")]
    public sealed class EthereumController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/ethereum";

        /// <summary>
        /// Common tag for Ethereum actions.
        /// </summary>
        internal const string EthereumTag = "Ethereum";

        private readonly ILogger<EthereumController> _logger;
        private readonly IEthereumScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="EthereumController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IEthereumScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public EthereumController(
            IEthereumScoringService scoringService,
            ILogger<EthereumController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="request">Request for getting the wallet stats.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/ethereum/wallet/0xF696AB3E4F9d52482B8350fFae67D21fda78e601/score?scoreType=0&amp;nonce=0&amp;deadline=1790647549
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetEthereumWalletScore")]
        [SwaggerOperation(
            OperationId = "GetEthereumWalletScore",
            Tags = new[] { EthereumTag })]
        [ProducesResponseType(typeof(Result<EthereumWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEthereumWalletScoreAsync(
            [Required(ErrorMessage = "Request should be set")] EthereumWalletStatsRequest request,
            CancellationToken cancellationToken = default)
        {
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    return Ok(await _scoringService.GetWalletStatsAsync<EthereumWalletStatsRequest, EthereumWalletScore, EthereumWalletStats, EthereumTransactionIntervalData>(request, cancellationToken));
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get Nomis Score for given wallets addresses.
        /// </summary>
        /// <param name="requests">The list of requests for getting the wallets stats.</param>
        /// <param name="concurrentRequestCount">Concurrent request count.</param>
        /// <param name="delayInMilliseconds">Delay in milliseconds between calls.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>An Nomis Score values and corresponding statistical data.</returns>
        /// <response code="200">Returns Nomis Scores and stats.</response>
        /// <response code="400">Addresses not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpPost("wallets/score", Name = "GetEthereumWalletsScore")]
        [SwaggerOperation(
            OperationId = "GetEthereumWalletsScore",
            Tags = new[] { EthereumTag })]
        [ProducesResponseType(typeof(Result<List<EthereumWalletScore>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEthereumWalletsScoreAsync(
            [Required(ErrorMessage = "Request should be set"), FromBody] IList<EthereumWalletStatsRequest> requests,
            int concurrentRequestCount = 1,
            int delayInMilliseconds = 500,
            CancellationToken cancellationToken = default)
        {
            if (requests.Any(r => r.ScoreType == ScoreType.Token))
            {
                throw new NotImplementedException();
            }

            return Ok(await _scoringService.GetWalletsStatsAsync<EthereumWalletStatsRequest, EthereumWalletScore, EthereumWalletStats, EthereumTransactionIntervalData>(requests, concurrentRequestCount, delayInMilliseconds, cancellationToken));
        }

        /// <summary>
        /// Get Nomis Score for given wallets addresses by file.
        /// </summary>
        /// <param name="request">Requests for getting the wallets stats parameters.</param>
        /// <param name="file">File with wallets addresses separated line by line.</param>
        /// <param name="concurrentRequestCount">Concurrent request count.</param>
        /// <param name="delayInMilliseconds">Delay in milliseconds between calls.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>An Nomis Score values and corresponding statistical data.</returns>
        /// <response code="200">Returns Nomis Scores and stats.</response>
        /// <response code="400">Addresses not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpPost("wallets/score-by-file", Name = "GetEthereumWalletsScoreByFile")]
        [SwaggerOperation(
            OperationId = "GetEthereumWalletsScoreByFile",
            Tags = new[] { EthereumTag })]
        [ProducesResponseType(typeof(Result<List<EthereumWalletScore>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateLimitResult), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEthereumWalletsScoreByFileAsync(
            [Required(ErrorMessage = "Request should be set")] EthereumWalletStatsRequest request,
            IFormFile file,
            int concurrentRequestCount = 1,
            int delayInMilliseconds = 500,
            CancellationToken cancellationToken = default)
        {
            switch (request.ScoreType)
            {
                case ScoreType.Finance:
                    var wallets = new List<string?>();
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        while (reader.Peek() >= 0)
                        {
                            wallets.Add(await reader.ReadLineAsync(cancellationToken));
                        }
                    }

                    return Ok(await _scoringService.GetWalletsStatsAsync<EthereumWalletStatsRequest, EthereumWalletScore, EthereumWalletStats, EthereumTransactionIntervalData>(
                        wallets.Where(x => x != null).Cast<string>().Select(wallet => new EthereumWalletStatsRequest
                        {
                            Address = wallet,
                            UseTokenLists = request.UseTokenLists,
                            CalculationModel = request.CalculationModel,
                            TokenAddress = request.TokenAddress,
                            Deadline = request.Deadline,
                            FirstSwapPairs = request.FirstSwapPairs,
                            GetAaveProtocolData = request.GetAaveProtocolData,
                            GetChainanalysisData = request.GetChainanalysisData,
                            GetCyberConnectProtocolData = request.GetCyberConnectProtocolData,
                            GetGreysafeData = request.GetGreysafeData,
                            GetHapiProtocolData = request.GetHapiProtocolData,
                            GetHoldTokensBalances = request.GetHoldTokensBalances,
                            GetSnapshotProtocolData = request.GetSnapshotProtocolData,
                            GetTokensSwapPairs = request.GetTokensSwapPairs,
                            IncludeUniversalTokenLists = request.IncludeUniversalTokenLists,
                            MintBlockchainType = request.MintBlockchainType,
                            Nonce = request.Nonce,
                            ScoreType = request.ScoreType,
                            SearchWidthInHours = request.SearchWidthInHours,
                            Skip = request.Skip
                        }).ToList(),
                        concurrentRequestCount,
                        delayInMilliseconds,
                        cancellationToken));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}