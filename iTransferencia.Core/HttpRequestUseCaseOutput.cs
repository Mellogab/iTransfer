namespace iTransferencia.Core
{
    public class HttpRequestUseCaseOutput
    {

        public HttpRequestUseCaseOutput() { }
        public HttpRequestUseCaseOutput(bool successfully, string output, string? error) 
        { 
            this.Successfully = successfully;
            this.output = output;
            this.Error = error;
        }


        public bool Successfully { get; set; } = true;
        public string Error { get; set; }
        public string output { get; set; }
        public object entity { get; set; }
    }
}

