namespace Brimborium.Extensions.SqlAccess {
    using Brimborium.Extensions.Entity;
    using Brimborium.Extensions.Freezable;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A base for SqlStoredProcedure.
    /// </summary>
    /// <typeparam name="TIn">The type of the in.</typeparam>
    public class SqlStoredProcedureBase<TIn>
        : FreezableObject
        , IEntityOperationWithParameter<TIn> {
        private readonly FreezableDictionary<string, SqlParameterDefinition> _ParameterDefinitions;

        /// <summary>
        /// The SQL name
        /// </summary>
        protected string _SqlName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStoredProcedureBase{TIn}"/> class.
        /// </summary>
        /// <param name="sqlName">Name of the SQL.</param>
        /// <exception cref="ArgumentNullException">sqlName</exception>
        protected SqlStoredProcedureBase(string sqlName) {
            if (string.IsNullOrEmpty(sqlName)) {
                throw new ArgumentNullException(nameof(sqlName));
            }

            this._SqlName = sqlName;
            this._ParameterDefinitions = new FreezableDictionary<string, SqlParameterDefinition>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the parameter definitions.
        /// </summary>
        /// <value>
        /// The parameter definitions.
        /// </value>
        public FreezableDictionary<string, SqlParameterDefinition> ParameterDefinitions => this._ParameterDefinitions;

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="sqlTransConnection">The SQL trans connection.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>a new SqlCommand</returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual SqlCommand CreateCommand(SqlTransConnection sqlTransConnection, TIn parameter) {
            var command = this.CreateCommand(sqlTransConnection);

            if (parameter is IEntity entity) {
                foreach (var sqlParameterDefinition in this.ParameterDefinitions.Values) {
                    var boundParameter = sqlParameterDefinition.AddParameter(command);

                    var parameterValue = entity.GetProperty(sqlParameterDefinition.Name, false, null);
                    boundParameter.SetValue(parameterValue);
                }
            } else {
                throw new NotSupportedException($"{parameter.GetType().FullName} is not a IEntity - implementation missing");
            }

            return command;
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="sqlTransConnection">The SQL trans connection.</param>
        /// <returns></returns>
        protected virtual SqlCommand CreateCommand(SqlTransConnection sqlTransConnection) => sqlTransConnection.SqlCommand(System.Data.CommandType.StoredProcedure, this._SqlName);

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameterDefinition">The parameter definition.</param>
        public void AddParameter(SqlParameterDefinition parameterDefinition) => this._ParameterDefinitions.Add(parameterDefinition.Name, parameterDefinition);

        /// <inheritdoc />
        public override bool Freeze() {
            var result = base.Freeze();
            if (result) {
                this._ParameterDefinitions.Freeze();
            }
            return result;
        }

        /// <inheritdoc />
        public override string ToString() => this._SqlName;

        //, ISqlMetaEntity
        // SqlObjectDefinition ISqlMetaEntity.EntitySqlObject => throw new NotImplementedException();
        //public IMetaProperty GetProperty(string name) {
        //    throw new NotImplementedException();
        //}

        //public IList<IMetaProperty> GetProperties() {
        //    throw new NotImplementedException();
        //}

        //public string Validate(IMetaProperty metaProperty, object value, bool validateOrThrow) {
        //    throw new NotImplementedException();
        //}
    }

    /// <summary>
    /// for Sql StoredProcedure ExecuteNonQuery.
    /// </summary>
    /// <typeparam name="TIn">The type of the in.</typeparam>
    public class SqlStoredProcedure<TIn> : SqlStoredProcedureBase<TIn> {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStoredProcedure{TIn}"/> class.
        /// </summary>
        /// <param name="sqlName">Name of the SQL.</param>
        protected SqlStoredProcedure(string sqlName) : base(sqlName) { }

        /// <summary>
        /// Executes the non query asynchronous.
        /// </summary>
        /// <param name="sqlTransConnection">The SQL trans connection.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(SqlTransConnection sqlTransConnection, TIn parameter, CancellationToken cancellationToken = default(CancellationToken)) {
            using (var command = this.CreateCommand(sqlTransConnection, parameter)) {
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }

    /// <summary>
    /// for Sql StoredProcedure ExecuteQuery.
    /// </summary>
    /// <typeparam name="TIn">The type of the in.</typeparam>
    /// <typeparam name="TOut">The type of the out.</typeparam>
    /// <seealso cref="Brimborium.Extensions.SqlAccess.SqlStoredProcedureBase{TIn}" />
    public abstract class SqlStoredProcedure<TIn, TOut> : SqlStoredProcedureBase<TIn> {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStoredProcedure{TIn, TOut}"/> class.
        /// </summary>
        /// <param name="sqlName">Name of the SQL.</param>
        protected SqlStoredProcedure(string sqlName) : base(sqlName) { }

        /// <summary>
        /// Executes the query asynchronous.
        /// </summary>
        /// <param name="sqlTransConnection">The SQL trans connection.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public TOut ExecuteQuery(SqlTransConnection sqlTransConnection, TIn parameter, CancellationToken cancellationToken = default(CancellationToken)) {
            using (var command = this.CreateCommand(sqlTransConnection, parameter)) {
                var result = SqlUtility.ExecuteReader(command, false, false);
                cancellationToken.ThrowIfCancellationRequested();
                return this.ConvertSqlReadResult(result);
            }
        }

        /// <summary>
        /// Executes the query asynchronous.
        /// </summary>
        /// <param name="sqlTransConnection">The SQL trans connection.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<TOut> ExecuteQueryAsync(SqlTransConnection sqlTransConnection, TIn parameter, CancellationToken cancellationToken = default(CancellationToken)) {
            using (var command = this.CreateCommand(sqlTransConnection, parameter)) {
                var result = await SqlUtility.ExecuteReaderAsync(command, false, false, cancellationToken);
                return this.ConvertSqlReadResult(result);
            }
        }
        //public virtual TOut ConvertSqlReadResult(List<SqlReadResult> lstSqlReadResult) => throw new NotImplementedException($"{this.GetType().FullName}");

        /// <summary>
        /// Converts the SQL read result.
        /// </summary>
        /// <param name="lstSqlReadResult">The list for read results.</param>
        /// <returns>the converted.</returns>
        public abstract TOut ConvertSqlReadResult(List<SqlReadResult> lstSqlReadResult);
    }
}
