using System;
using System.Data.SqlClient;

namespace FunctionAppWithSQLServer.Repository
{
    public interface RowRepository
    {
        public string GetFirstValue(string key);

    }

    public class SQLServerRowRepository : RowRepository
    {
        private readonly SqlConnection _conn;
        private readonly string _keyColumnName;
        private readonly string _valueColumnName;
        private readonly string _tableName;

        private SQLServerRowRepository(SqlConnection conn, string keyConlumnName, string valueColumnName, string tableName)
        {
            this._conn = conn;
            this._keyColumnName = keyConlumnName;
            this._valueColumnName = valueColumnName;
            this._tableName = tableName;
        }

        public static RowRepository OfConnection(SqlConnection conn)
        {
            return new SQLServerRowRepository(conn, "id", "value", "[dbo].[values]"); ;
        }

        private SqlCommand BuildCommand(string key)
        {
            SqlCommand command = new SqlCommand(null, this._conn);
            command.CommandText = $"select {this._valueColumnName} from {this._tableName} where {this._keyColumnName} = @key";

            SqlParameter keyParameter = new SqlParameter("@key", System.Data.SqlDbType.VarChar, 0);
            keyParameter.Value = key;
            command.Parameters.Add(keyParameter);

            return command;

        }

        public string GetFirstValue(string key)
        {
            SqlCommand command = this.BuildCommand(key);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read() == true)
                {
                    return (string)reader[this._valueColumnName];
                }
                else
                {
                    throw new Exception("some exception");
                }
            }

        }
    }
}
