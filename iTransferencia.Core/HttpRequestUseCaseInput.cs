namespace iTransferencia.Core
{
    public class HttpRequestUseCaseInput
    {
        public HttpRequestUseCaseInput()
        {
            this.Headers = new Dictionary<string, string>();
        }

        public string Key { get; set; }
        public string Uri { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public Dictionary<string, string> Headers { get; set; }
        public object Content { get; set; }
    }
}