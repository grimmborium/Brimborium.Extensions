using System;

namespace Brimborium.Extensions.LogInside {
    public class InsideLog {
        private InsideLog _InsideLog;
        private string _Message;

        public InsideLog() {
        }

        public InsideLog(string message) {
            this._Message = message;
        }

        public InsideLog(InsideLog previous, string message) {
            this._InsideLog = previous;
            this._Message = message;
        }

        //public bool Condition(bool value, string message) {
        //    return value;
        //}
        //public T Value<T>(T value, string message) {
        //    return value;
        //}

        //public bool Condition<T>(T value, Func<T, bool> func, string message) {
        //    throw new NotImplementedException();
        //}

        //public T Value<T, R>(T value, Func<T, R> func, string message) {
        //    throw new NotImplementedException();
        //}
        public InsideLog Log(string message) {
            return new InsideLog(this, message) ;
        }

        public override string ToString() {
            return this._Message;
        }
    }
}
