using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Projac.Sql;

namespace Projac.SqlClient
{
    /// <summary>
    /// Represents the T-SQL MONEY parameter value.
    /// </summary>
    public class TSqlMoneyValue : IDbParameterValue
    {
        private readonly decimal _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TSqlMoneyValue" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public TSqlMoneyValue(decimal value)
        {
            _value = value;
        }

        /// <summary>
        ///     Creates a <see cref="DbParameter" /> instance based on this instance.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>
        ///     A <see cref="DbParameter" />.
        /// </returns>
        public DbParameter ToDbParameter(string parameterName)
        {
            return ToSqlParameter(parameterName);
        }

        /// <summary>
        ///     Creates a <see cref="SqlParameter" /> instance based on this instance.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>
        ///     A <see cref="SqlParameter" />.
        /// </returns>
        public SqlParameter ToSqlParameter(string parameterName)
        {
#if NET46 || NET452
            return new SqlParameter(
                parameterName,
                SqlDbType.Money,
                8,
                ParameterDirection.Input,
                false,
                0,
                4,
                "",
                DataRowVersion.Default,
                _value);
#elif NETSTANDARD2_0
            return new SqlParameter 
                {
                    ParameterName = parameterName,
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Money,
                    Size = 8,
                    Value = _value,
                    SourceColumn = "",
                    IsNullable = false,
                    Precision = 0,
                    Scale = 4,
                    SourceVersion = DataRowVersion.Default
                };
#endif
        }

        private bool Equals(TSqlMoneyValue other)
        {
            return _value == other._value;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TSqlMoneyValue)obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}