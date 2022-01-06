using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;

namespace FunctionAppWithSQLServer.Repository.Tests
{
    [TestClass()]
    public class SQLServerRowRepositoryTests
    {
        [TestMethod()]
        public void GetFirstValueTest()
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                RowRepository repo = SQLServerRowRepository.OfConnection(conn);
                Assert.AreEqual(repo.GetFirstValue("key"), "value");
            }
        }
    }
}