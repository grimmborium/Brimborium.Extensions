namespace Brimborium.Extensions.Http.Logging {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Http.Headers;
    using System.Text;

    internal class HttpHeadersLogValue : IReadOnlyList<KeyValuePair<string, object>> {
        private readonly Kind _Kind;

        private string _formatted;
        private List<KeyValuePair<string, object>> _values;

        public HttpHeadersLogValue(Kind kind, HttpHeaders headers, HttpHeaders contentHeaders) {
            this._Kind = kind;

            this.Headers = headers;
            this.ContentHeaders = contentHeaders;
        }

        public HttpHeaders Headers { get; }

        public HttpHeaders ContentHeaders { get; }

        private List<KeyValuePair<string, object>> Values {
            get {
                if (this._values == null) {
                    var values = new List<KeyValuePair<string, object>>();

                    foreach (var kvp in this.Headers) {
                        values.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value));
                    }

                    if (this.ContentHeaders != null) {
                        foreach (var kvp in this.ContentHeaders) {
                            values.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value));
                        }
                    }

                    this._values = values;
                }

                return this._values;
            }
        }

        public KeyValuePair<string, object> this[int index] {
            get {
                if (index < 0 || index >= this.Count) {
                    throw new IndexOutOfRangeException(nameof(index));
                }

                return this.Values[index];
            }
        }

        public int Count => this.Values.Count;

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return this.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.Values.GetEnumerator();
        }

        public override string ToString() {
            if (this._formatted == null) {
                var builder = new StringBuilder();
                builder.AppendLine(this._Kind == Kind.Request ? "Request Headers:" : "Response Headers:");

                for (var i = 0; i < this.Values.Count; i++) {
                    var kvp = this.Values[i];
                    builder.Append(kvp.Key);
                    builder.Append(": ");

                    foreach (var value in (IEnumerable<object>)kvp.Value) {
                        builder.Append(value);
                        builder.Append(", ");
                    }

                    // Remove the extra ', '
                    builder.Remove(builder.Length - 2, 2);
                    builder.AppendLine();
                }

                this._formatted = builder.ToString();
            }

            return this._formatted;
        }

        public enum Kind {
            Request,
            Response,
        }
    }
}
