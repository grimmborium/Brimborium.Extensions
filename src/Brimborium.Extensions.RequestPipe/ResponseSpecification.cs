using System;

namespace Brimborium.Extensions.RequestPipe {
    public class ResponseSpecification {
        private static ResponseOK? _InstanceOK;
        public static ResponseOK OK => _InstanceOK ??= new ResponseOK();
    }

    public class ResponseSuccess : ResponseSpecification {
        public ResponseSuccess() {
        }
    }

    public class ResponseOK : ResponseSuccess {
        public ResponseOK() {
        }
    }

    public class ResponseFail : ResponseSpecification {
        public ResponseFail() {
        }
    }

    public class ResponseNotFound : ResponseFail {
        public ResponseNotFound() {
        }
    }

    public class ResponseFaulted : ResponseSpecification {
        public ResponseFaulted(Exception exception) {
            this.Exception = exception;
        }

        public T Rethrow<T>() {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(this.Exception).Throw();
            return default!;
        }

        public Exception Exception { get; }
    }

}
