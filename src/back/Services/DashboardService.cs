using YourProject.Models;
using MySql.Data.MySqlClient;

namespace YourProject.Services
{
    public class DashboardService
    {
        private readonly string _connectionString;

        public DashboardService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("Connection string not found.");
        }

        public Dashboard GetByUserId(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "SELECT * FROM Dashboards WHERE usuarioId = @UserId";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Dashboard
                {
                    Id = reader.GetInt32("id"),
                    SaldoTotal = reader.GetDouble("saldoTotal"),
                    InvestimentoTotal = reader.GetDouble("investimentoTotal")
                };
            }
            return null;
        }

        public Dashboard Add(Dashboard dashboard)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = @"INSERT INTO Dashboards (usuarioId, saldoTotal, investimentoTotal) 
                         VALUES (@UsuarioId, @SaldoTotal, @InvestimentoTotal)";
            using var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@UsuarioId", dashboard.Usuario.Id);
            cmd.Parameters.AddWithValue("@SaldoTotal", dashboard.SaldoTotal);
            cmd.Parameters.AddWithValue("@InvestimentoTotal", dashboard.InvestimentoTotal);

            cmd.ExecuteNonQuery();
            dashboard.Id = (int)cmd.LastInsertedId;
            return dashboard;
        }

        public void Update(int id, Dashboard dashboard)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = @"UPDATE Dashboards 
                         SET saldoTotal = @SaldoTotal, 
                             investimentoTotal = @InvestimentoTotal 
                         WHERE id = @Id";
            using var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@SaldoTotal", dashboard.SaldoTotal);
            cmd.Parameters.AddWithValue("@InvestimentoTotal", dashboard.InvestimentoTotal);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }
    }
}