namespace Brimborium.Extensions.RequestPipe {
    public static class Response {
        public static Response<TResponse> Create<TResponse>(TResponse result) => new Response<TResponse>(ResponseSpecification.OK, result);
    }
    public struct Response<TResponse> {
        public ResponseSpecification Specification { get;set;}
        public TResponse Result { get; set; }
        public Response(ResponseSpecification specification, TResponse result) {
            this.Specification = specification;
            this.Result = result;
        }
    }
}