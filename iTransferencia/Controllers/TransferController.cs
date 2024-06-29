using iTransferencia.Core;
using iTransferencia.Core.UseCases.Transfers.ExecuteTransfer;
using iTransferencia.Presenters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;

namespace iTransferencia.Controllers
{

    [ApiController]
    [Route("api/transfer")]
    public class TransferController : ControllerBase
    {
        private readonly ILogger<TransferController> _logger;
        public TransferController(ILogger<TransferController> logger) => _logger = logger;

        [HttpPost]
        [Route("transfers-async")]
        public async Task<ActionResult> ExeuteTransfersAsync(
            [FromServices] IExecuteTransferUseCase useCase,
            [FromServices] DefaultPresenter<UseCaseResponseMessage> presenter,
            [FromBody] ExecuteTransferInput input,
            CancellationToken cancellationToken
        )
        {
            try
            {
                input.CancellationToken = cancellationToken;
                await useCase.Handle(input, presenter);
                return presenter.GetJsonResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error On {MethodBase.GetCurrentMethod().Name}. Ex {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}