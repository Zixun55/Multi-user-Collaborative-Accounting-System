using System.Data;
using System.Collections.Generic;

namespace project.Models.Interfaces
{
    public interface IDatabaseHelper
    {
        DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters);
        int ExecuteNonQuery(string sql, Dictionary<string, object> parameters);
        void ExecuteTransaction(List<(string sql, Dictionary<string, object> parameters)> commands);
        object ExecuteScalar(string sql, Dictionary<string, object> parameters);
    }
}
