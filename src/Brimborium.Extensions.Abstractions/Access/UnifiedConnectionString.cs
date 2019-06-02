namespace Brimborium.Extensions.Access {
    using Brimborium.Extensions.Freezable;

    using Newtonsoft.Json; // TODO: remove dependency - any idea for an abstraction?? or remove the parse method??

    using System;

    /// <summary>
    /// A ConnectionString.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("UCS:{Name};{Url};{User}")]
    public class UnifiedConnectionString : FreezableObject, IUnifiedConnectionString {
        /// <summary>
        /// Parses the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>a new UnifiedConnectionString.</returns>
        public static UnifiedConnectionString Parse(string connectionString) {
            if (string.IsNullOrEmpty(connectionString)) {
                return null;
            }
            connectionString = connectionString.Trim();
            if (connectionString.StartsWith("{") && connectionString.EndsWith("}")) {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<UnifiedConnectionString>(connectionString);
            } else {
                return new UnifiedConnectionString() {
                    Url = connectionString
                };
            }
        }

        private string _Name;
        private string _AuthenticationMode;
        private string _Url;
        private string _Suffix;
        private string _SecretKey;
        private string _User;
        private string _Password;

        /// <summary>Initializes a new instance of the <see cref="UnifiedConnectionString"/> class.</summary>
        public UnifiedConnectionString() {
        }

        /// <summary>Initializes a new instance of the <see cref="UnifiedConnectionString"/> class.</summary>
        /// <param name="src">The source - can be null.</param>
        public UnifiedConnectionString(UnifiedConnectionString src) {
            if ((object)src == null) {
                //
            } else {
                this.Name = src.Name;
                this.AuthenticationMode = src.AuthenticationMode;
                this.Url = src.Url;
                this.Suffix = src.Suffix;
                this.SecretKey = src.SecretKey;
                this.User = src.User;
                this.Password = src.Password;
            }
        }

        /// <summary>
        /// Copies this and sets the specified name.
        /// </summary>
        /// <param name="name">The name to set in the copy.</param>
        /// <returns>a copy of this</returns>
        public virtual UnifiedConnectionString Copy(string name) {
            var result = new UnifiedConnectionString(this);
            if ((object)name != null) {
                result.Name = name;
            }
            return result;
        }

        /// <summary>a unique name - primary key.</summary>
        [JsonProperty(Order = 1)]
        public virtual string Name {
            get => this._Name; set => this.SetStringProperty(ref this._Name, value);
        }

        /// <summary>the authentication mode e.g. Windows, Basic, ...</summary>
        [JsonProperty(Order = 2)]
        public virtual string AuthenticationMode {
            get => this._AuthenticationMode; set => this.SetStringProperty(ref this._AuthenticationMode, value);
        }

        /// <summary>the url or sql connection string.</summary>
        [JsonProperty(Order = 3)]
        public virtual string Url {
            get => this._Url; set => this.SetStringProperty(ref this._Url, value);
        }

        /// <summary>the suffix for a http url e.g. api.</summary>
        [JsonProperty(Order = 4)]
        public virtual string Suffix {
            get => this._Suffix; set => this.SetStringProperty(ref this._Suffix, value);
        }

        /// <summary>Reference key to the secret.</summary>
        [JsonProperty(Order = 5)]
        public virtual string SecretKey {
            get => this._SecretKey; set => this.SetStringProperty(ref this._SecretKey, value);
        }

        /// <summary>Username.</summary>
        [JsonProperty(Order = 6)]
        public virtual string User {
            get => this._User; set => this.SetStringProperty(ref this._User, value);
        }

        /// <summary>Password.</summary>
        [JsonProperty(Order = 7)]
        [System.Diagnostics.DebuggerDisplay("***")]
        [Brimborium.Extensions.Destructurama.Attributed.LogMasked(PreserveLength = false, ShowFirst = 1, ShowLast = 1, Text = "...")]
        public virtual string Password {
            get => this._Password; set => this.SetStringProperty(ref this._Password, value);
        }

        private System.Tuple<string, string, string> _GetUrlNormalizedCache;

        /// <summary>Gets the URL normalized.</summary>
        /// <param name="appendSlash">if set to <c>true</c> [append slash].</param>
        /// <returns>the Url</returns>
        public virtual string GetUrlNormalized(bool appendSlash = false) {
            var cache = this._GetUrlNormalizedCache;
            System.Threading.Interlocked.MemoryBarrier();
            if (cache != null
                && string.Equals(cache.Item1, this.Url, StringComparison.Ordinal)
                && string.Equals(cache.Item2, this.Suffix, StringComparison.Ordinal)
                ) {
                var result = cache.Item3;
                if (appendSlash) { result += "/"; }
                return result;
            }
            if (string.IsNullOrEmpty(this.Suffix)) {
                var result = (this.Url ?? string.Empty).TrimEnd('/');
                this._GetUrlNormalizedCache = Tuple.Create(this.Url, this.Suffix, result);
                if (appendSlash) { result += "/"; }
                return result;
            } else {
                var result = (
                                (this.Url ?? string.Empty).TrimEnd('/') + "/" + (this.Suffix ?? string.Empty).Trim('/')
                            ).TrimEnd('/');
                this._GetUrlNormalizedCache = Tuple.Create(this.Url, this.Suffix, result);
                if (appendSlash) { result += "/"; }
                return result;
            }
        }

        /// <summary>Creates a copy the with suffix.</summary>
        /// <param name="suffix">The suffix to set.</param>
        /// <returns>a copy with the suffix.</returns>
        public virtual UnifiedConnectionString CreateWithSuffix(string suffix) {
            var result = new UnifiedConnectionString(this);
            result.Suffix = suffix;
            return result;
        }

        /// <summary>Overwrites/merges this with the specified overwrite.</summary>
        /// <param name="overwrite">The overwrite.</param>
        /// <returns>a copy.</returns>
        public virtual UnifiedConnectionString Overwrite(UnifiedConnectionString overwrite) {
            var result = new UnifiedConnectionString(this);
            if (overwrite != null) {
                // name??
                if (!string.IsNullOrEmpty(overwrite.AuthenticationMode)) {
                    result.AuthenticationMode = overwrite.AuthenticationMode;
                }
                if (!string.IsNullOrEmpty(overwrite.Url)) {
                    result.Url = overwrite.Url;
                }
                if (!string.IsNullOrEmpty(overwrite.Suffix)) {
                    result.Suffix = overwrite.Suffix;
                }
                if (!string.IsNullOrEmpty(overwrite.SecretKey)) {
                    result.SecretKey = overwrite.SecretKey;
                }
                if (!string.IsNullOrEmpty(overwrite.User)) {
                    result.User = overwrite.User;
                }
                if (!string.IsNullOrEmpty(overwrite.Password)) {
                    result.Password = overwrite.Password;
                }
            }
            return result;
        }


        /// <summary>
        /// Get this as json.
        /// </summary>
        /// <returns>json representation of this</returns>
        public string AsJson() {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            });
            return json;
        }
    }
}
