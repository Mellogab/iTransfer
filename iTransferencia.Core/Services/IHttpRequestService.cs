using iTransferencia.Core.Entities;

namespace iTransferencia.Core.Services
{
    public interface IHttpRequestService
    {
        Task<HttpRequestUseCaseOutput> MakeRequest<TEntity>(HttpRequestUseCaseInput input);
    }
}
