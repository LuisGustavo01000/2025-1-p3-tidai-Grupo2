using YourProject.Models;
using MySql.Data.MySqlClient;

namespace YourProject.Services
{
    public class MetaFinanceiraService
    {
        private readonly string _connectionString;

        public MetaFinanceiraService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("Connection string not found.");
        }

        public List<MetaFinanceira> GetAll()
        {
            var metas = new List<MetaFinanceira>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "SELECT * FROM MetasFinanceiras";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                metas.Add(new MetaFinanceira
                {
                    Id = reader.GetInt32("id"),
                    Nome = reader.GetString("nome"),
                    Valor = reader.GetDouble("valor"),
                    Prazo = reader.GetDateTime("prazo"),
                    Status = reader.GetString("status")
                });
            }

            return metas;
        }

        public MetaFinanceira Get(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "SELECT * FROM MetasFinanceiras WHERE id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new MetaFinanceira
                {
                    Id = reader.GetInt32("id"),
                    Nome = reader.GetString("nome"),
                    Valor = reader.GetDouble("valor"),
                    Prazo = reader.GetDateTime("prazo"),
                    Status = reader.GetString("status")
                };
            }
            return null;
        }

        public MetaFinanceira Add(MetaFinanceira meta)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = @"INSERT INTO MetasFinanceiras (nome, valor, prazo, status, usuarioId) 
                         VALUES (@Nome, @Valor, @Prazo, @Status, @UsuarioId)";
            using var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Nome", meta.Nome);
            cmd.Parameters.AddWithValue("@Valor", meta.Valor);
            cmd.Parameters.AddWithValue("@Prazo", meta.Prazo);
            cmd.Parameters.AddWithValue("@Status", meta.Status);
            cmd.Parameters.AddWithValue("@UsuarioId", meta.Usuario.Id);

            cmd.ExecuteNonQuery();
            meta.Id = (int)cmd.LastInsertedId;
            return meta;
        }

        public void Update(int id, MetaFinanceira meta)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = @"UPDATE MetasFinanceiras 
                         SET nome = @Nome, 
                             valor = @Valor, 
                             prazo = @Prazo, 
                             status = @Status 
                         WHERE id = @Id";
            using var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Nome", meta.Nome);
            cmd.Parameters.AddWithValue("@Valor", meta.Valor);
            cmd.Parameters.AddWithValue("@Prazo", meta.Prazo);
            cmd.Parameters.AddWithValue("@Status", meta.Status);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public bool Delete(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "DELETE FROM MetasFinanceiras WHERE id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}