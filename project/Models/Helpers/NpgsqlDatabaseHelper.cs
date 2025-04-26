using Npgsql;
using System.Data;
using project.Models.Interfaces;

namespace project.Models.Helpers
{
    public class NpgsqlDatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        public NpgsqlDatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters)
        {
            var dt = new DataTable();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(sql, conn);
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }

                var adapter = new NpgsqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(sql, conn);
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        public void ExecuteTransaction(List<(string sql, Dictionary<string, object> parameters)> commands)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            using (var cmd = new NpgsqlCommand(command.sql, conn, transaction))
                            {
                                foreach (var param in command.parameters)
                                {
                                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(sql, conn);
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
                return cmd.ExecuteScalar();
            }
        }
    }
}