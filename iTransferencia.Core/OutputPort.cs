namespace iTransferencia.Core
{
    public class OutputPort<T> : IOutputPort<T>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public T Result { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Handle(T response)
        {
            Result = response;
        }

        public T GetResult()
        {
            return Result;
        }
    }
}
