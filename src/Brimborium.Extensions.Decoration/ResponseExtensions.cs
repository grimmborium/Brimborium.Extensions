namespace Brimborium.Extensions.Decoration {
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
