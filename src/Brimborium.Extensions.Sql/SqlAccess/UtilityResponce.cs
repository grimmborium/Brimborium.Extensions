namespace Brimborium.Extensions.SqlAccess {
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    [JsonObject]
    public sealed class UtilityResponce {
        /// <summary>
        /// Convert JSON to object.
        /// </summary>
        /// <param name="json">the source JSON</param>
        /// <returns>the object</returns>
        public static UtilityResponce FromJson(string json) {
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<UtilityResponce>(json, settings);
        }

        /// <summary>
        /// convert to JSON
        /// </summary>
        /// <param name="responce">object ot convert</param>
        /// <returns>json</returns>
        public static string ToJson(UtilityResponce responce) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(
                responce,
                new Newtonsoft.Json.JsonSerializerSettings() {
                    Formatting = Newtonsoft.Json.Formatting.None
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UtilityResponce"/> class.
        /// </summary>
        public UtilityResponce() {
        }

        /// <summary>
        /// Gets or sets the kind as  mimetype text/plain, text/html or application/json.
        /// </summary>
        [JsonProperty]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [JsonProperty]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the binary value
        /// </summary>
        [JsonProperty]
        public byte[] BinaryValue { get; set; }

        /// <summary>
        /// Gets or sets the nonce
        /// </summary>
        [JsonProperty]
        public string Nonce { get; set; }

#if later
        /// <summary>
        /// Gets or sets the post requests mode Execute or Insert.
        /// </summary>
        public string PostRequestsMode { get; set; }

        /// <summary>
        /// Gets or sets the post requests.
        /// </summary>
        public UtilityRequest[] PostRequests { get; set; }
#endif

        /// <summary>
        /// Get the value as HTML
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="cache">cache</param>
        /// <param name="isDefault">if Value contains a string - if isDefault is true the value will be returned.</param>
        /// <returns>the this[key] as string.</returns>
        public string GetValueNormalized(string key, ref Dictionary<string, object> cache, bool isDefault) {
            if (string.IsNullOrEmpty(this.Value)) {
                return null;
            }
            if (string.Equals(this.Kind, Consts.MimeTypeText, StringComparison.OrdinalIgnoreCase)) {
                if (isDefault) {
                    return System.Net.WebUtility.HtmlEncode(this.Value);
                } else {
                    return null;
                }
            } else if (string.Equals(this.Kind, Consts.MimeTypeHTML, StringComparison.OrdinalIgnoreCase)) {
                if (isDefault) {
                    return this.Value;
                } else {
                    return null;
                }
            } else if (string.Equals(this.Kind, Consts.MimeTypeJSON, StringComparison.OrdinalIgnoreCase)) {
                if (cache == null) {
                    var o = Newtonsoft.Json.JsonConvert.DeserializeObject(this.Value);
                    if (o is string) {
                        return o as string;
                    }
                    if (o is Newtonsoft.Json.Linq.JArray) {
                        var arr = ((Newtonsoft.Json.Linq.JArray)o);
                        o = arr.First;
                    }
                    if (o is Newtonsoft.Json.Linq.JArray) {
                        var arr = ((Newtonsoft.Json.Linq.JArray)o);
                        o = arr.First;
                    }
                    if (o is Newtonsoft.Json.Linq.JObject) {
                        var obj = ((Newtonsoft.Json.Linq.JObject)o);
                        cache = obj.ToObject<Dictionary<string, object>>();
                    }
                }
                if (cache == null) {
                    return this.Value;
                } else {
                    object result;
                    if (cache.TryGetValue(key, out result)) {
                        if (result == null) {
                            return null;
                        }
                        if (result is string) {
                            return result as string;
                        } else {
                            return result.ToString();
                        }
                    } else {
                        return string.Empty;
                    }
                }
            } else {
                // other types are not supported
                return string.Empty;
            }
        }
    }
}
