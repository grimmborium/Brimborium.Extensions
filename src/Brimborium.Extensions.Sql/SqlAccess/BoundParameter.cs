namespace Brimborium.Extensions.SqlAccess {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Paramter bounded to SqlStoredProcedure
    /// </summary>
    public sealed class BoundParameter {
        /// <summary>
        /// The real parameter
        /// </summary>
        public readonly SqlParameter Parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundParameter"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentNullException">parameter</exception>
        public BoundParameter(SqlParameter parameter) {
            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value {
            get => this.Parameter.Value;
            set => this.SetValue(value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(object value) {
            var p = this.Parameter;
            if (string.IsNullOrEmpty(p.TypeName)) {
                if (value is null) {
                    p.Value = DBNull.Value;
                } else {
                    p.Value = value;
                }
            } else {
                if (value is null) {
                    p.Value = new List<Microsoft.SqlServer.Server.SqlDataRecord>();
                } else {
                    p.Value = value;
                }
            }
        }
    }

    /// <summary>
    /// Bound structured parameter.
    /// </summary>
    /// <typeparam name="TIn">The type of the in.</typeparam>
    public class BoundStructuredParameter<TIn> {
        /// <summary>
        /// The parameter
        /// </summary>
        public readonly SqlParameter Parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundStructuredParameter{TIn}"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentNullException">parameter</exception>
        public BoundStructuredParameter(SqlParameter parameter) {
            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="items">The items.</param>
        public void SetValue(IEnumerable<TIn> items) {
            this.Parameter.Value = this.Convert(items);
        }

        /// <summary>
        /// Converts the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public List<Microsoft.SqlServer.Server.SqlDataRecord> Convert(IEnumerable<TIn> items) {
            var result = new List<Microsoft.SqlServer.Server.SqlDataRecord>();
            if (!(items is null)) {
                var lstMetaDatas = new List<Microsoft.SqlServer.Server.SqlMetaData>();
                lstMetaDatas.Add(new Microsoft.SqlServer.Server.SqlMetaData("Column3", SqlDbType.DateTime));
                var arrMetaDatas = lstMetaDatas.ToArray();

                foreach (var item in items) {
                    var sqlDataRecord = new Microsoft.SqlServer.Server.SqlDataRecord(arrMetaDatas);
                    //sqlDataRecord.SetValues();
                }
            }
            return result;
        }
    }
}
