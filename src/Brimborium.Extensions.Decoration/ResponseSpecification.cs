using System;

namespace Brimborium.Extensions.Decoration {
    public class ResponseSpecification {
        private static ResponseOK? _InstanceOK;
        public static ResponseOK OK => _InstanceOK ??= new ResponseOK();

        private static ResponseFail? _InstanceFail;
        public static ResponseFail Fail => _InstanceFail ??= new ResponseFail();

        private static ResponseFaulted? _InstanceFaulted;
        public static ResponseFaulted Faulted => _InstanceFaulted ??= new ResponseFaulted(new InvalidOperationException());

        internal ResponseSpecification() {
        }

        public virtual bool IsSuccess => false;
        public virtual bool IsFail => false;
        public virtual bool IsFaulted => false;
        public virtual Exception GetException() => new Exception();
    }

    public class ResponseSuccess : ResponseSpecification {
        public ResponseSuccess() {
        }
        public override bool IsSuccess => true;
    }

    public class ResponseOK : ResponseSuccess {
        public ResponseOK() {
        }
    }

    public class ResponseFail : ResponseSpecification {
        public ResponseFail() {
        }
        public override bool IsFail => true;
    }

    public class ResponseNotFound : ResponseFail {
        public ResponseNotFound() {
        }
    }

    public class ResponseFaulted : ResponseSpecification {
        public ResponseFaulted(Exception exception) {
            this.Exception = exception;
        }
        public override bool IsFaulted => true;

        public T Rethrow<T>() {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(this.Exception).Throw();
            return default!;
        }

        public Exception Exception { get; }

        public override Exception GetException() => this.Exception;
    }

}
