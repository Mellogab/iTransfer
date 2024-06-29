namespace iTransferencia.Core
{
    public interface IUseCaseRequest
    {
        public interface IUseCaseRequest<out TUseCaseResponse> { }
    }
}
