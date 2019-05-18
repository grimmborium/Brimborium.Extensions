namespace Brimborium.Extensions.SqlAccess {
    using System;
    using System.Data;
    using System.Reflection;

    using Brimborium.Extensions.Freezable;

    /// <summary>
    /// Sql Column Definition
    /// </summary>
    public sealed class SqlColumnDefinition : FreezableObject {
        private string _Name;
        private SqlDbType _SqlDbType;
        private int _Size;
        private byte _Precision;
        private byte _Scale;
        private bool _IsNullable;
        private Type _PropertyType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlColumnDefinition"/> class.
        /// </summary>
        public SqlColumnDefinition() {
            this._IsNullable = true;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get => this._Name; set => this.SetStringProperty(ref this._Name, value); }

        /// <summary>
        /// Gets or sets the type of the SQL database.
        /// </summary>
        /// <value>
        /// The type of the SQL database.
        /// </value>
        public SqlDbType SqlDbType { get => this._SqlDbType; set => this.SetValueProperty(ref this._SqlDbType, value); }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size { get => this._Size; set => this.SetValueProperty(ref this._Size, value); }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public byte Precision { get => this._Precision; set => this.SetValueProperty(ref this._Precision, value); }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public byte Scale { get => this._Scale; set => this.SetValueProperty(ref this._Scale, value); }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable { get => this._IsNullable; set => this.SetValueProperty(ref this._IsNullable, value); }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public Type PropertyType { get => this._PropertyType; set => this.SetRefProperty(ref this._PropertyType, value); }
    }

    /// <summary>
    /// Attribute
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SqlColumnAttribute : Attribute {
        private SqlColumnDefinition _SqlColumnDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlColumnAttribute"/> class.
        /// </summary>
        public SqlColumnAttribute() {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get => this.GetSqlColumnDefinition().Name; set => this.GetSqlColumnDefinition().Name = value; }

        /// <summary>
        /// Gets or sets the type of the SQL database.
        /// </summary>
        /// <value>
        /// The type of the SQL database.
        /// </value>
        public SqlDbType SqlDbType { get => this.GetSqlColumnDefinition().SqlDbType; set => this.GetSqlColumnDefinition().SqlDbType = value; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size { get => this.GetSqlColumnDefinition().Size; set => this.GetSqlColumnDefinition().Size = value; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public byte Precision { get => this.GetSqlColumnDefinition().Precision; set => this.GetSqlColumnDefinition().Precision = value; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public byte Scale { get => this.GetSqlColumnDefinition().Scale; set => this.GetSqlColumnDefinition().Scale = value; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable { get => this.GetSqlColumnDefinition().IsNullable; set => this.GetSqlColumnDefinition().IsNullable = value; }

        /// <summary>
        /// Gets or sets the SQL column definition.
        /// </summary>
        /// <value>
        /// The SQL column definition.
        /// </value>
        public SqlColumnDefinition SqlColumnDefinition { get => this._SqlColumnDefinition; set => this._SqlColumnDefinition = value; }

        /// <summary>
        /// Gets the SQL column definition.
        /// </summary>
        /// <returns></returns>
        public SqlColumnDefinition GetSqlColumnDefinition() {
            if (this._SqlColumnDefinition is null) {
                return this._SqlColumnDefinition = new SqlColumnDefinition();
            } else {
                return this._SqlColumnDefinition;
            }
        }

        /// <summary>
        /// Gets the SQL column definition.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public SqlColumnDefinition GetSqlColumnDefinition(PropertyInfo property) {
            var result = this.GetSqlColumnDefinition();
            if (string.IsNullOrEmpty(result.Name)) {
                result.Name = property.Name;
            }
            if (result.PropertyType is null) {
                result.PropertyType = property.PropertyType;
            }
            return result;
        }
    }
}
