using System;
using System.Data;
using NUnit.Framework;
using Projac.SqlClient;

namespace Projac.Sql.Tests.SqlClient
{
    [TestFixture]
    public class TSqlVarBinaryNullValueTests
    {
        private TSqlVarBinaryNullValue _sut;
        private TSqlVarBinarySize _size;

        [SetUp]
        public void SetUp()
        {
            _size = new TSqlVarBinarySize(100);
            _sut = new TSqlVarBinaryNullValue(_size);
        }

        [Test]
        public void IsSqlParameterValue()
        {
            Assert.That(_sut, Is.InstanceOf<IDbParameterValue>());
        }

        [Test]
        public void ToDbParameterReturnsExpectedInstance()
        {
            const string parameterName = "name";

            var result = _sut.ToDbParameter(parameterName);

            result.ExpectSqlParameter(parameterName, SqlDbType.VarBinary, DBNull.Value, true, 100);
        }

        [Test]
        public void ToSqlParameterReturnsExpectedInstance()
        {
            const string parameterName = "name";

            var result = _sut.ToSqlParameter(parameterName);

            result.ExpectSqlParameter(parameterName, SqlDbType.VarBinary, DBNull.Value, true, 100);
        }

        [Test]
        public void DoesEqualItself()
        {
            var self = _sut;
            Assert.That(_sut.Equals(self), Is.True);
        }

        [Test]
        public void DoesNotEqualOtherObjectType()
        {
            Assert.That(_sut.Equals(new object()), Is.False);
        }

        [Test]
        public void DoesNotEqualNull()
        {
            Assert.That(_sut.Equals(null), Is.False);
        }

        [Test]
        public void HasExpectedHashCode()
        {
            var result = _sut.GetHashCode();

            Assert.That(result, Is.EqualTo(100));
        }
    }
}