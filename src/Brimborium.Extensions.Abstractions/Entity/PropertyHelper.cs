#pragma warning disable CS1591
namespace Brimborium.Extensions.Entity {
    using System;

    public static class PropertyHelper {
        /// <summary>
        /// Get the value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value -or- ifNotExistsValue</returns>
        public static object GetProperty(this IEntity that, string name, bool ignoreIfNotExists = true, object ifNotExistsValue = null) {
            if (that is null) { throw new ArgumentNullException(nameof(that)); }
            if (name is null) { throw new ArgumentNullException(nameof(name)); }
            var property = that.MetaData.GetProperty(name);
            if (property is null) {
                if (ignoreIfNotExists) {
                    return ifNotExistsValue;
                } else {
                    throw new InvalidOperationException(string.Format("Property {0} not found.", name));
                }
            }
            return property.GetAccessor(that).Value;
        }

        /// <summary>
        /// Get the string value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value -or- ifNotExistsValue</returns>
        public static string GetPropertyAsString(this IEntity that, string name, bool ignoreIfNotExists = true, string ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            return (string)value;
        }

        /// <summary>
        /// Get the TimeSpan value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value -or- ifNotExistsValue</returns>
        public static System.TimeSpan? GetPropertyAsTimeSpanQ(this IEntity that, string name, bool ignoreIfNotExists = true, string ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is null) { return null; }
            if (value is decimal dec) {
                return TimeSpan.FromMinutes((double)dec);
            }
            throw new NotImplementedException("GetPropertyAsTimeSpanQ");
        }

        /// <summary>
        /// Get the Double value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value -or- ifNotExistsValue</returns>
        public static double? GetPropertyAsDoubleQ(this IEntity that, string name, bool ignoreIfNotExists = true, string ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is null) { return null; }
            if (value is double) {
                return ((double)value);
            }
            throw new NotImplementedException("GetPropertyAsTimeSpanQ");
        }

        /// <summary>
        /// Get the Enum value of the property named name of that entity .
        /// </summary>
        /// <typeparam name="TEnum">a enum</typeparam>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value -or- ifNotExistsValue</returns>
        public static TEnum? GetPropertyAsEnumQ<TEnum>(this IEntity that, string name, bool ignoreIfNotExists = true, string ifNotExistsValue = null)
            where TEnum : struct {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is null) { return null; }
            if (value is string) {
                if (Enum.TryParse<TEnum>((string)value, out var e)) {
                    return e;
                } else {
                    return null;
                }
            }
            if (value is int) {
                return (TEnum)(value);
            }
            throw new NotImplementedException("GetPropertyAsTimeSpanQ");
        }

        /// <summary>
        /// Get the Guid value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static Guid GetPropertyAsGuid(this IEntity that, string name, bool ignoreIfNotExists = true, Guid? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is Guid guidValue) {
                return guidValue;
            }
            if (value is string txtValue) {
                return Guid.Parse(txtValue);
            }
            return (Guid)value;
        }

        /// <summary>
        /// Get the bool value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static bool GetPropertyAsBool(this IEntity that, string name, bool ignoreIfNotExists = true, bool? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            return (bool)value;
        }

        /// <summary>
        /// Get the bool? value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static bool? GetPropertyAsBoolQ(this IEntity that, string name, bool ignoreIfNotExists = true, bool? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            //return (bool?)value;
            if (value is null) { return null; }
            if (value is bool boolValue) {
                return boolValue;
            }
            return bool.Parse(value.ToString());
        }

        /// <summary>
        /// Get the Guid? value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static Guid? GetPropertyAsGuidQ(this IEntity that, string name, bool ignoreIfNotExists = true, Guid? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            //return (Guid?)value;
            if (value is null) { return null; }
            if (value is Guid) {
                return (Guid)value;
            }
            return Guid.Parse(value.ToString());
        }

        /// <summary>
        /// Get the Byte value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static byte GetPropertyAsByte(this IEntity that, string name, bool ignoreIfNotExists = true, byte ifNotExistsValue = 0) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            try {
                if (value is byte byteValue) {
                    return byteValue;
                }
                return System.Convert.ToByte(value);
            } catch {
                return System.Convert.ToByte(value);
            }
        }

        /// <summary>
        /// Get the Byte? value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static byte? GetPropertyAsByteQ(this IEntity that, string name, bool ignoreIfNotExists = true, byte? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is null) {
                return null;
            }
            if (value is byte byteValue) {
                return byteValue;
            }
            return System.Convert.ToByte(value);
        }

        /// <summary>
        /// Get the short value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static short GetPropertyAsShort(this IEntity that, string name, bool ignoreIfNotExists = true, short ifNotExistsValue = 0) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is short shortValue) {
                return shortValue;
            } else if (value is byte byteValue) {
                return (short)byteValue;
            }
            return System.Convert.ToInt16(value);
        }

        /// <summary>
        /// Get the short? value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static short? GetPropertyAsShortQ(this IEntity that, string name, bool ignoreIfNotExists = true, short? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is null) { return null; }
            if (value is short shortValue) {
                return shortValue;
            } else if (value is byte byteValue) {
                return (short)byteValue;
            }
            return System.Convert.ToInt16(value);
        }

        /// <summary>
        /// Get the int value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static int GetPropertyAsInt(this IEntity that, string name, bool ignoreIfNotExists = true, int ifNotExistsValue = 0) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is int intValue) {
                return intValue;
            } else if (value is byte byteValue) {
                return (int)byteValue;
            } else if (value is short shortValue) {
                return (int)shortValue;
            }
            return System.Convert.ToInt32(value);
        }

        /// <summary>
        /// Get the int? value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static int? GetPropertyAsIntQ(this IEntity that, string name, bool ignoreIfNotExists = true, int? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is null) {
                return null;
            }
            if (value is int intValue) {
                return intValue;
            } else if (value is byte byteValue) {
                return (int)byteValue;
            } else if (value is short shortValue) {
                return (int)shortValue;
            }
            return System.Convert.ToInt32(value);
        }

        /// <summary>
        /// Get the long value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static long GetPropertyAsLong(this IEntity that, string name, bool ignoreIfNotExists = true, long ifNotExistsValue = 0) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is long longValue) {
                return longValue;
            } else if (value is int intValue) {
                return intValue;
            } else if (value is byte byteValue) {
                return (int)byteValue;
            } else if (value is short shortValue) {
                return (int)shortValue;
            }
            return System.Convert.ToInt64(value);
        }

        /// <summary>
        /// Get the long? value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static long? GetPropertyAsLongQ(this IEntity that, string name, bool ignoreIfNotExists = true, long? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            if (value is long longValue) {
                return longValue;
            } else if (value is int intValue) {
                return intValue;
            } else if (value is byte byteValue) {
                return (int)byteValue;
            } else if (value is short shortValue) {
                return (int)shortValue;
            }
            return System.Convert.ToInt64(value);
        }

        /// <summary>
        /// Get the DateTime value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static DateTime GetPropertyAsDateTime(this IEntity that, string name, bool ignoreIfNotExists = true, DateTime? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            return (DateTime)value;
        }

        /// <summary>
        /// Get the DateTime? value of the property named name of that entity .
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        /// <param name="ifNotExistsValue">if not exists and ignored return this value.</param>
        /// <returns>The value</returns>
        public static DateTime? GetPropertyAsDateTimeQ(this IEntity that, string name, bool ignoreIfNotExists = true, DateTime? ifNotExistsValue = null) {
            object value = GetProperty(that, name, ignoreIfNotExists, ifNotExistsValue);
            return (DateTime?)value;
        }

        /// <summary>
        /// Set the value of property named named of that entity to value.
        /// </summary>
        /// <param name="that">the owner entity</param>
        /// <param name="name">the name of the property.</param>
        /// <param name="value">the value.</param>
        /// <param name="ignoreIfNotExists">throw an error or ignore</param>
        public static void SetProperty(this IEntity that, string name, object value, bool ignoreIfNotExists = true) {
            if (that is null) { throw new ArgumentNullException(nameof(that)); }
            if (name is null) { throw new ArgumentNullException(nameof(name)); }
            var property = that.MetaData.GetProperty(name);
            if (property is null) {
                if (ignoreIfNotExists) {
                    return;
                } else {
                    throw new InvalidOperationException(string.Format("Property {0} not found.", name));
                }
            }
            property.GetAccessor(that).Value = value;
        }
    }
}