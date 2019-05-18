#pragma warning disable CS1591

namespace Brimborium.Extensions.SqlAccess {
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Brimborium.Extensions.Freezable;

    public sealed class SqlParameterDefinition : FreezableObject {
        private const int MAX_PARAMETER_NAME_LENGTH = 128;
        private string _Name;
        private SqlDbType _SqlDbType;
        private int _Size;
        private byte _Precision;
        private byte _Scale;
        private ParameterDirection _Direction;
        private bool _IsNullable;
        private String _TypeName;
        private String _UdtTypeName;
        private Type _PropertyType;

        public SqlParameterDefinition() {
        }

        public SqlParameterDefinition(
            string parameterName,
            SqlDbType dbType, int size,
            ParameterDirection direction, bool isNullable,
            byte precision, byte scale
            ) {
            this.Name = parameterName;
            this.SqlDbType = dbType;
            this.Size = size;
            this.Direction = direction;
            this.IsNullable = isNullable;
            this.Precision = precision;
            this.Scale = scale;
        }
        public SqlParameterDefinition(
            string parameterName,
            SqlDbType dbType, int size,
            ParameterDirection direction,
            byte precision, byte scale) {
            this.Name = parameterName;
            this.SqlDbType = dbType;
            this.Size = size;
            this.Direction = direction;
            this.Precision = precision;
            this.Scale = scale;
        }
        public SqlParameterDefinition(
            string parameterName,
            SqlDbType dbType) {
            this.Name = parameterName;
            this.SqlDbType = dbType;
        }

        public SqlParameterDefinition(
            string parameterName,
            SqlDbType dbType,
            int size) {
            this.Name = parameterName;
            this.SqlDbType = dbType;
            this.Size = size;
        }

        public SqlParameterDefinition(
            string parameterName,
            string typeName) {
            this.Name = parameterName;
            this.SqlDbType = SqlDbType.Structured;
            this.TypeName = typeName;
        }

        public void Enrich(SqlColumnDefinition sqlColumnDefinition) {
            if (sqlColumnDefinition is null) { return; }
            if (this.Name is null) { this.Name = sqlColumnDefinition.Name; }
            if (sqlColumnDefinition.SqlDbType != SqlDbType.BigInt) { this.SqlDbType = sqlColumnDefinition.SqlDbType; }
            if (sqlColumnDefinition.Size != 0) { this.Size = sqlColumnDefinition.Size; }
            if (sqlColumnDefinition.Precision != 0) { this.Precision = sqlColumnDefinition.Precision; }
            if (sqlColumnDefinition.Scale != 0) { this.Scale = sqlColumnDefinition.Scale; }
            if (sqlColumnDefinition.IsNullable != false) { this.IsNullable = sqlColumnDefinition.IsNullable; }
        }

        public string Name { get => this._Name; set => this.SetStringProperty(ref this._Name, value); }
        public SqlDbType SqlDbType { get => this._SqlDbType; set => this.SetValueProperty(ref this._SqlDbType, value); }
        public int Size { get => this._Size; set => this.SetValueProperty(ref this._Size, value); }
        public byte Precision { get => this._Precision; set => this.SetValueProperty(ref this._Precision, value); }
        public byte Scale { get => this._Scale; set => this.SetValueProperty(ref this._Scale, value); }
        public ParameterDirection Direction { get => this._Direction; set => this.SetValueProperty(ref this._Direction, value); }
        public bool IsNullable { get => this._IsNullable; set => this.SetValueProperty(ref this._IsNullable, value); }
        public String TypeName { get => this._TypeName; set => this.SetStringProperty(ref this._TypeName, value); }
        public String UdtTypeName { get => this._UdtTypeName; set => this.SetStringProperty(ref this._UdtTypeName, value); }

        public Type PropertyType { get => this._PropertyType; set => this.SetRefProperty(ref this._PropertyType, value); }

        public SqlParameter SqlParameter {
            get {
                var result = new SqlParameter();
                result.ParameterName = this.Name;
                result.SqlDbType = this.SqlDbType;
                result.Direction = this.Direction;
                result.IsNullable = this.IsNullable;
                if (this.TypeName != null) { result.TypeName = this.TypeName; }
                if (this.UdtTypeName != null) { result.UdtTypeName = this.UdtTypeName; }
                if (this.Size != 0) { result.Size = this.Size; }
                if (this.Precision != 0) { result.Precision = this.Precision; }
                if (this.Scale != 0) { result.Scale = this.Scale; }

                return result;
            }
        }

        public BoundParameter AddParameter(SqlCommand sqlCommand) {
            var parameter = this.SqlParameter;
            sqlCommand.Parameters.Add(parameter);
            return new BoundParameter(parameter);
        }

        public BoundStructuredParameter<TIn> AddStructuredParameter<TIn>(SqlCommand sqlCommand) {
            var parameter = this.SqlParameter;
            sqlCommand.Parameters.Add(parameter);
            return new BoundStructuredParameter<TIn>(parameter);
        }

    }
}
