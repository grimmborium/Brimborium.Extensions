namespace Brimborium.Extensions.SqlAccess {
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// SqlUtility
    /// </summary>
    public static class SqlUtility {
        /// <summary>
        /// Gets the value or DBNull.Value
        /// </summary>
        /// <param name="value">any value</param>
        /// <returns>The value or DBNull.Value</returns>
        public static object OrDBNull(object value) {
            if (value is null) { return DBNull.Value; }
            return value;
        }

        /// <summary>
        /// Replace the DBNULL.Value with null.
        /// </summary>
        /// <param name="values">the read values from sql</param>
        public static void ReplaceDBNullByNull(object[] values) {
            // DBNULL to null
            var cnt = values.Length;
            for (int idx = 0; idx < cnt; idx++) {
                if (values[idx] == DBNull.Value) {
                    values[idx] = null;
                }
            }
        }

        /// <summary>
        /// Create a [Brimborium].[TVP_KeyValue] parameter from a list
        /// </summary>
        /// <param name="name">the name of the parameter.</param>
        /// <param name="e">the list</param>
        /// <returns>the parameter</returns>
        public static SqlParameter CreateTVP_KeyValueParameter(string name, IEnumerable<KeyValuePair<string, string>> e) {
            var lst = new List<SqlDataRecord>();
            var sdrKey = new SqlMetaData("Key", SqlDbType.NVarChar, 255);
            var sdrValue = new SqlMetaData("Value", SqlDbType.NVarChar, SqlMetaData.Max);
            foreach (var kv in e) {
                if (kv.Value != null) {
                    var sdr = new SqlDataRecord(sdrKey, sdrValue);
                    sdr.SetString(0, kv.Key);
                    sdr.SetString(1, kv.Value);
                    lst.Add(sdr);
                }
            }
            var parameter = new SqlParameter(name, SqlDbType.Structured) {
                Direction = ParameterDirection.Input,
                TypeName = "[Brimborium].[TVP_KeyValue]",
                Value = ((lst.Count == 0) ? null : lst)
            };
            return parameter;
        }

        /// <summary>
        /// Create a [Brimborium].[TVP_Uid] parameter from a list
        /// </summary>
        /// <param name="name">the name of the parameter.</param>
        /// <param name="e">the list</param>
        /// <returns>the parameter</returns>
        public static SqlParameter CreateTVP_UidParameter(string name, IEnumerable<Guid> e) {
            var lst = new List<SqlDataRecord>();
            var sdrValue = new SqlMetaData("Item", SqlDbType.UniqueIdentifier);
            foreach (var item in e) {
                var sdr = new SqlDataRecord(sdrValue);
                sdr.SetGuid(0, item);
                lst.Add(sdr);
            }
            var parameter = new SqlParameter(name, SqlDbType.Structured) {
                Direction = ParameterDirection.Input,
                TypeName = "[Brimborium].[TVP_Uid]",
                Value = ((lst.Count == 0) ? null : lst)
            };
            return parameter;
        }

        /// <summary>
        /// Convert the sql results to a log string.
        /// </summary>
        /// <param name="sqlReadResults">the source</param>
        /// <param name="sb">the target.</param>
        public static void ConvertReadResultsToLogs(List<SqlReadResult> sqlReadResults, System.Text.StringBuilder sb) {
            if (sqlReadResults.Count > 0) {
                foreach (var sqlReadResult in sqlReadResults) {
                    foreach (var row in sqlReadResult.Rows) {
                        foreach (var cell in row) {
                            if (cell != null) {
                                sb.Append(cell);
                            }
                        }
                        sb.AppendLine();
                    }
                    if (sqlReadResult.MeassureError != null) {
                        sb.AppendLine(string.Join(System.Environment.NewLine, sqlReadResult.MeassureError));
                    }
                    if (sqlReadResult.MeassureMessage != null) {
                        sb.AppendLine(string.Join(System.Environment.NewLine, sqlReadResult.MeassureMessage));
                    }
                }
            }
        }

        /// <summary>
        /// Convert the sql results to UtilityResponce
        /// </summary>
        /// <param name="sqlReadResults">the source</param>
        /// <param name="result">the target.</param>
        public static void ConvertReadResultsToResponce(List<SqlReadResult> sqlReadResults, UtilityResponce result) {
            int kind = 0;
            var stringBuilder = new System.Text.StringBuilder();

            if (sqlReadResults.Count > 0) {
                var sqlReadResult = sqlReadResults[0];
                if ((sqlReadResult.FieldCount == 1)
                    && (sqlReadResult.Rows.Count > 0)
                    && ((string.Equals(sqlReadResult.FieldNames[0], "kind", StringComparison.OrdinalIgnoreCase))
                        || (string.Equals(sqlReadResult.FieldNames[0], "mimetype", StringComparison.OrdinalIgnoreCase)))) {
                    var cell00 = sqlReadResult.Rows[0][0] as string;
                    if ((string.Equals(cell00, "text", StringComparison.OrdinalIgnoreCase))
                        || (string.Equals(cell00, Consts.MimeTypeText, StringComparison.OrdinalIgnoreCase))
                        || (string.Equals(cell00, Consts.MimeTypeJavascript, StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = Consts.MimeTypeText;
                        kind = 1;
                        sqlReadResults.RemoveAt(0);
                    } else if ((string.Equals(cell00, Consts.MimeTypeHTML, StringComparison.OrdinalIgnoreCase))
                            || (string.Equals(cell00, "html", StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = Consts.MimeTypeHTML;
                        kind = 2;
                        sqlReadResults.RemoveAt(0);
                    } else if ((string.Equals(cell00, "json", StringComparison.OrdinalIgnoreCase))
                         || (string.Equals(cell00, Consts.MimeTypeJSON, StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = Consts.MimeTypeJSON;
                        kind = 3;
                        sqlReadResults.RemoveAt(0);
                    } else if ((string.Equals(cell00, Consts.MimeTypeXML, StringComparison.OrdinalIgnoreCase))
                        || (string.Equals(cell00, Consts.MimeTypeTextXML, StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = cell00;
                        kind = 4;
                        sqlReadResults.RemoveAt(0);
                    } else {
                        result.Kind = cell00;
                        kind = 1;
                    }
                } else if (sqlReadResult.FieldCount >= 1) {
                    var fieldName0 = sqlReadResult.FieldNames[0] as string;
                    if ((string.Equals(fieldName0, "text", StringComparison.OrdinalIgnoreCase))
                        || (string.Equals(fieldName0, Consts.MimeTypeText, StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = Consts.MimeTypeText;
                        kind = 1;
                    } else if ((string.Equals(fieldName0, Consts.MimeTypeHTML, StringComparison.OrdinalIgnoreCase))
                            || (string.Equals(fieldName0, "html", StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = Consts.MimeTypeHTML;
                        kind = 2;
                    } else if ((string.Equals(fieldName0, "json", StringComparison.OrdinalIgnoreCase))
                        || (string.Equals(fieldName0, Consts.MimeTypeJSON, StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = Consts.MimeTypeJSON;
                        kind = 3;
                    } else if ((string.Equals(fieldName0, Consts.MimeTypeXML, StringComparison.OrdinalIgnoreCase))
                        || (string.Equals(fieldName0, Consts.MimeTypeTextXML, StringComparison.OrdinalIgnoreCase))) {
                        result.Kind = fieldName0;
                        kind = 4;
                    } else {
                        result.Kind = "application/json";
                        kind = 3;
                    }
                }
            }
            if (kind == 1) {
                foreach (var sqlReadResult in sqlReadResults) {
                    foreach (var row in sqlReadResult.Rows) {
                        for (int fieldIndex = 0; fieldIndex < sqlReadResult.FieldCount; fieldIndex++) {
                            var value = row[fieldIndex];
                            if (value == null) { continue; }
                            stringBuilder.Append(value);
                        }
                    }
                }
            } else if (kind == 2) {
                foreach (var sqlReadResult in sqlReadResults) {
                    foreach (var row in sqlReadResult.Rows) {
                        for (int fieldIndex = 0; fieldIndex < sqlReadResult.FieldCount; fieldIndex++) {
                            var value = row[fieldIndex];
                            if (value == null) { continue; }
                            stringBuilder.Append(value);
                        }
                    }
                }
            } else if (kind == 3) {
                using (System.IO.StringWriter stringWriter = new System.IO.StringWriter(stringBuilder)) {
                    using (Newtonsoft.Json.JsonTextWriter jsonTextWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter)) {
                        jsonTextWriter.WriteStartArray();
                        foreach (var sqlReadResult in sqlReadResults) {
                            jsonTextWriter.WriteStartArray();
                            foreach (var row in sqlReadResult.Rows) {
                                jsonTextWriter.WriteStartObject();
                                for (int fieldIndex = 0; fieldIndex < sqlReadResult.FieldCount; fieldIndex++) {
                                    var fieldName = sqlReadResult.FieldNames[fieldIndex];
                                    var value = row[fieldIndex];
                                    jsonTextWriter.WritePropertyName(fieldName);
                                    jsonTextWriter.WriteValue(value);
                                }
                                jsonTextWriter.WriteEndObject();
                            }
                            jsonTextWriter.WriteEndArray();
                            if ((sqlReadResult.MeassureMessage.Count > 0) && (sqlReadResult.MeassureError.Count > 0)) {
                                if (sqlReadResult.MeassureMessage.Count > 0) {
                                    jsonTextWriter.WriteStartArray();
                                    foreach (var meassureMessage in sqlReadResult.MeassureMessage) {
                                        jsonTextWriter.WriteStartObject();
                                        jsonTextWriter.WritePropertyName("MeassureMessage");
                                        jsonTextWriter.WriteValue(meassureMessage);
                                        jsonTextWriter.WriteEndObject();
                                    }
                                    jsonTextWriter.WriteEndArray();
                                }
                                if (sqlReadResult.MeassureError.Count > 0) {
                                    jsonTextWriter.WriteStartArray();
                                    foreach (var meassureError in sqlReadResult.MeassureError) {
                                        jsonTextWriter.WriteStartObject();
                                        jsonTextWriter.WritePropertyName("MeassureError");
                                        jsonTextWriter.WriteValue(meassureError);
                                        jsonTextWriter.WriteEndObject();
                                    }
                                    jsonTextWriter.WriteEndArray();
                                }
                            }
                        }
                        jsonTextWriter.WriteEndArray();
                    }
                }
            } else if (kind == 4) {
                using (System.IO.StringWriter stringWriter = new System.IO.StringWriter(stringBuilder)) {
                    using (var xmlWriter = new System.Xml.XmlTextWriter(stringWriter)) {
                        xmlWriter.Formatting = System.Xml.Formatting.None;
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("data");
                        foreach (var sqlReadResult in sqlReadResults) {
                            xmlWriter.WriteStartElement("result");
                            foreach (var row in sqlReadResult.Rows) {
                                xmlWriter.WriteStartElement("row");
                                for (int fieldIndex = 0; fieldIndex < sqlReadResult.FieldCount; fieldIndex++) {
                                    var fieldName = sqlReadResult.FieldNames[fieldIndex];
                                    var value = row[fieldIndex];
                                    xmlWriter.WriteStartElement(fieldName);
                                    if (value == null) {
                                        // ignore
                                    } else if (value == DBNull.Value) {
                                        // ignore
                                    } else if (value is string) {
                                        xmlWriter.WriteValue((string)value);
                                    } else if (value is bool) {
                                        xmlWriter.WriteValue((bool)value);
                                    } else if (value is DateTime) {
                                        xmlWriter.WriteValue((DateTime)value);
                                    } else if (value is decimal) {
                                        xmlWriter.WriteValue((decimal)value);
                                    } else if (value is float) {
                                        xmlWriter.WriteValue((float)value);
                                    } else if (value is double) {
                                        xmlWriter.WriteValue((double)value);
                                    } else if (value is int) {
                                        xmlWriter.WriteValue((int)value);
                                    } else if (value is long) {
                                        xmlWriter.WriteValue((long)value);
                                    } else {
                                        xmlWriter.WriteElementString(fieldName, value.ToString());
                                    }
                                    xmlWriter.WriteEndElement(/*fieldName*/);
                                }
                                xmlWriter.WriteEndElement(/*row*/);
                            }
                            xmlWriter.WriteEndElement(/*result*/);
                        }
                        xmlWriter.WriteEndElement(/*data*/);
                        xmlWriter.WriteEndDocument();
                    }
                    using (Newtonsoft.Json.JsonTextWriter jsonTextWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter)) {
                        jsonTextWriter.WriteStartArray();
                        foreach (var sqlReadResult in sqlReadResults) {
                            jsonTextWriter.WriteStartArray();
                            foreach (var row in sqlReadResult.Rows) {
                                jsonTextWriter.WriteStartObject();
                                for (int fieldIndex = 0; fieldIndex < sqlReadResult.FieldCount; fieldIndex++) {
                                    var fieldName = sqlReadResult.FieldNames[fieldIndex];
                                    var value = row[fieldIndex];
                                    jsonTextWriter.WritePropertyName(fieldName);
                                    jsonTextWriter.WriteValue(value);
                                }
                                jsonTextWriter.WriteEndObject();
                            }
                            jsonTextWriter.WriteEndArray();
                        }
                        jsonTextWriter.WriteEndArray();
                    }
                }
            }
            result.Value = stringBuilder.ToString();
        }

        /// <summary>
        /// Split text in GO
        /// </summary>
        /// <param name="content">the sql text</param>
        /// <returns>the sql textes</returns>
        public static List<string> SplitInBatched(string content) {
            var batches = new List<string>();
            if (string.IsNullOrWhiteSpace(content)) { return batches; }
            var lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new System.Text.StringBuilder();
            foreach (var line in lines) {
                if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase)) {
                    if (sb.Length > 0) {
                        batches.Add(sb.ToString());
                        sb = new System.Text.StringBuilder();
                    }
                } else {
                    sb.AppendLine(line);
                }
            }
            batches.Add(sb.ToString());
            return batches;
        }


        public static List<SqlReadResult> ExecuteReader(SqlCommand command, bool meassure, bool log) {
            var result = new List<SqlReadResult>();
            if (meassure) {
                using (SqlCommand commandS = new SqlCommand()) {
                    commandS.Connection = command.Connection;
                    commandS.CommandText = "SET STATISTICS IO ON;";
                    if (command.Transaction != null) {
                        commandS.Transaction = command.Transaction;
                    }
                    commandS.ExecuteNonQuery();
                }
            }
            using (var m = (meassure || log) ? (new SqlMeassure(command.Connection)) : null) {
                using (var reader = command.ExecuteReader()) {
                    var nextResult = true;
                    for (int resultIndex = 0; (resultIndex == 0) || nextResult; resultIndex++) {
                        var readResult = new SqlReadResult() { ResultIndex = resultIndex };
                        result.Add(readResult);

                        int fieldCount = -1;
                        string[] fieldNames = null;
                        string[] fieldTypes = null;
                        for (int rowIndex = 0; reader.Read(); rowIndex++) {
                            if (rowIndex == 0) {
                                fieldCount = readResult.FieldCount = reader.FieldCount;
                                fieldNames = new string[fieldCount];
                                fieldTypes = new string[fieldCount];
                                for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++) {
                                    fieldNames[fieldIndex] = reader.GetName(fieldIndex);
                                    fieldTypes[fieldIndex] = reader.GetDataTypeName(fieldIndex);
                                }
                                readResult.FieldNames = fieldNames;
                                readResult.FieldTypes = fieldTypes;
                            }
                            var values = new object[fieldCount];
                            reader.GetValues(values);
                            ReplaceDBNullByNull(values);
                            readResult.Add(values);
                        }
                        if (readResult.FieldCount == 0) {
                            fieldCount = readResult.FieldCount = reader.FieldCount;
                            fieldNames = new string[fieldCount];
                            fieldTypes = new string[fieldCount];
                            for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++) {
                                fieldNames[fieldIndex] = reader.GetName(fieldIndex);
                                fieldTypes[fieldIndex] = reader.GetDataTypeName(fieldIndex);
                            }
                            readResult.FieldNames = fieldNames;
                            readResult.FieldTypes = fieldTypes;
                        }
                        nextResult = (reader.NextResult());
                        if (m != null) {
                            readResult.MeassureMessage.AddRange(m.Messages);
                            readResult.MeassureError.AddRange(m.Errors);
                            m.Messages.Clear();
                            m.Errors.Clear();
                        }
                    }
                }
            }
            return result;
        }

        public static async Task<List<SqlReadResult>> ExecuteReaderAsync(SqlCommand command, bool meassure, bool log, CancellationToken cancellationToken = default(CancellationToken)) {
            var result = new List<SqlReadResult>();
            if (meassure) {
                using (SqlCommand commandS = new SqlCommand()) {
                    commandS.Connection = command.Connection;
                    commandS.CommandText = "SET STATISTICS IO ON;";
                    if (command.Transaction != null) {
                        commandS.Transaction = command.Transaction;
                    }
                    await commandS.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            using (var m = (meassure || log) ? (new SqlMeassure(command.Connection)) : null) {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false)) {
                    var nextResult = true;
                    for (int resultIndex = 0; (resultIndex == 0) || nextResult; resultIndex++) {
                        var readResult = new SqlReadResult() { ResultIndex = resultIndex };
                        result.Add(readResult);

                        int fieldCount = -1;
                        string[] fieldNames = null;
                        string[] fieldTypes = null;

                        //for (int rowIndex = 0; await reader.ReadAsync().ConfigureAwait(false); rowIndex++) {
                        for (int rowIndex = 0; reader.Read(); rowIndex++) {
                            if (rowIndex == 0) {
                                fieldCount = readResult.FieldCount = reader.FieldCount;
                                fieldNames = new string[fieldCount];
                                fieldTypes = new string[fieldCount];
                                for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++) {
                                    fieldNames[fieldIndex] = reader.GetName(fieldIndex);
                                    fieldTypes[fieldIndex] = reader.GetDataTypeName(fieldIndex);
                                }
                                readResult.FieldNames = fieldNames;
                                readResult.FieldTypes = fieldTypes;
                            }
                            var values = new object[fieldCount];
                            reader.GetValues(values);
                            ReplaceDBNullByNull(values);
                            readResult.Add(values);

                            if (cancellationToken.IsCancellationRequested) { cancellationToken.ThrowIfCancellationRequested(); }
                        }
                        if (readResult.FieldCount == 0) {
                            fieldCount = readResult.FieldCount = reader.FieldCount;
                            fieldNames = new string[fieldCount];
                            fieldTypes = new string[fieldCount];
                            for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++) {
                                fieldNames[fieldIndex] = reader.GetName(fieldIndex);
                                fieldTypes[fieldIndex] = reader.GetDataTypeName(fieldIndex);
                            }
                            readResult.FieldNames = fieldNames;
                            readResult.FieldTypes = fieldTypes;
                        }
                        nextResult = (await reader.NextResultAsync().ConfigureAwait(false));
                        if (m != null) {
                            readResult.MeassureMessage.AddRange(m.Messages);
                            readResult.MeassureError.AddRange(m.Errors);
                            m.Messages.Clear();
                            m.Errors.Clear();
                        }
                    }
                }
            }
            return result;
        }

        public static async Task ExecuteNonQueryAsync(SqlCommand command, bool meassure, bool log) {

            if (meassure) {
                using (SqlCommand commandS = new SqlCommand()) {
                    commandS.Connection = command.Connection;
                    commandS.CommandText = "SET STATISTICS IO ON;";
                    if (command.Transaction != null) {
                        commandS.Transaction = command.Transaction;
                    }
                    await commandS.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            using (var m = (meassure || log) ? (new SqlMeassure(command.Connection)) : null) {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false)) {
                    var nextResult = true;
                    for (int resultIndex = 0; (resultIndex == 0) || nextResult; resultIndex++) {
                        var readResult = new SqlReadResult() { ResultIndex = resultIndex };
                        int fieldCount = -1;
                        string[] fieldNames = null;
                        string[] fieldTypes = null;
                        //for (int rowIndex = 0; reader.Read(); rowIndex++) {
                        for (int rowIndex = 0; await reader.ReadAsync().ConfigureAwait(false); rowIndex++) {
                            if (rowIndex == 0) {
                                fieldCount = readResult.FieldCount = reader.FieldCount;
                                fieldNames = new string[fieldCount];
                                fieldTypes = new string[fieldCount];
                                for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++) {
                                    fieldNames[fieldIndex] = reader.GetName(fieldIndex);
                                    fieldTypes[fieldIndex] = reader.GetDataTypeName(fieldIndex);
                                }
                                readResult.FieldNames = fieldNames;
                                readResult.FieldTypes = fieldTypes;
                            }
                            var values = new object[fieldCount];
                            reader.GetValues(values);
                            ReplaceDBNullByNull(values);
                            readResult.Add(values);
                        }
                        if (readResult.FieldCount == 0) {
                            fieldCount = readResult.FieldCount = reader.FieldCount;
                            fieldNames = new string[fieldCount];
                            fieldTypes = new string[fieldCount];
                            for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++) {
                                fieldNames[fieldIndex] = reader.GetName(fieldIndex);
                                fieldTypes[fieldIndex] = reader.GetDataTypeName(fieldIndex);
                            }
                            readResult.FieldNames = fieldNames;
                            readResult.FieldTypes = fieldTypes;
                        }
                        nextResult = (await reader.NextResultAsync().ConfigureAwait(false));
                        if (m != null) {
                            readResult.MeassureMessage.AddRange(m.Messages);
                            readResult.MeassureError.AddRange(m.Errors);
                            m.Messages.Clear();
                            m.Errors.Clear();
                        }
                    }
                }
            }
        }
    }
}
