using System.Threading.Tasks;

namespace Brimborium.Extensions.RequestPipe {
    public static class Response {
        public static Response<TResponse> FromResultOk<TResponse>(TResponse result) => new Response<TResponse>(ResponseSpecification.OK, result);
        public static Task<Response<TResponse>> FromResultOkTask<TResponse>(TResponse result) => Task.FromResult(new Response<TResponse>(ResponseSpecification.OK, result));
    }

    public struct Response<TResponse> {
        public ResponseSpecification Specification { get;set;}
        public TResponse Result { get; set; }
        public Response(ResponseSpecification specification, TResponse result) {
            this.Specification = specification;
            this.Result = result;
        }
        public void Deconstruct(out ResponseSpecification specification, out TResponse result)
            => (specification, result) = (Specification, Result);
    }

    public static class ResponseExtensions {
        public static void Rethrow<TResponse>(Response<TResponse> response, bool failIfNoException) {
            if (response.Specification is ResponseFaulted responseFaulted) {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(responseFaulted.Exception).Throw();
            }
            if (failIfNoException) { 
                throw new System.InvalidOperationException("no Exception to Rethrow");            
            }
        }
    }
}