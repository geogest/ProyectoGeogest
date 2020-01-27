using System.Data.Entity;

namespace TryTestWeb.App_Start
{
    public class MySqlConfiguration : DbConfiguration
    {
        public MySqlConfiguration()
        {
            
            SetHistoryContext(
            "MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));

        }
    }
}