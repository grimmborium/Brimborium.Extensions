using System;

namespace Brimborium.Extensions.RequestPipe {
    public class ResponseSpecification {
        private static ResponseOK? _InstanceOK;
        public static ResponseOK OK => _InstanceOK ??= new ResponseOK();
    }

    public class ResponseOK : ResponseSpecification {
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
        public ResponseFaulted(Exception error) {
            this.Error = error;
        }

        public Exception Error { get; }
    }

}
