using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InitialSetupMVC.Data
{
    public class DbConnection
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DbConnection(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? throw new System.InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
