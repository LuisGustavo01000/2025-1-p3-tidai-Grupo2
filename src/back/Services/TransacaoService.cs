using YourProject.Models;
using MySql.Data.MySqlClient;

namespace YourProject.Services
{
    public class TransacaoService
    {
        private readonly string _connectionString;

        public TransacaoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("Connection string not found.");
        }

        public List<Transacao> GetAll()
        {
            var transacoes = new List<Transacao>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "SELECT * FROM Transacoes";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                transacoes.Add(new Transacao
                {
                    Id = reader.GetInt32("id"),
                    Descricao = reader.GetString("descricao"),
                    Valor = reader.GetDouble("valor"),
                    Tipo = reader.GetString("tipo"),
                    Data = reader.GetDateTime("data")
                });
            }

            return transacoes;
        }

        public Transacao Get(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "SELECT * FROM Transacoes WHERE id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Transacao
                {
                    Id = reader.GetInt32("id"),
                    Descricao = reader.GetString("descricao"),
                    Valor = reader.GetDouble("valor"),
                    Tipo = reader.GetString("tipo"),
                    Data = reader.GetDateTime("data")
                };
            }
            return null;
        }

        public Transacao Add(Transacao transacao)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = @"INSERT INTO Transacoes (descricao, valor, tipo, data) 
                         VALUES (@Descricao, @Valor, @Tipo, @Data)";
            using var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Descricao", transacao.Descricao);
            cmd.Parameters.AddWithValue("@Valor", transacao.Valor);
            cmd.Parameters.AddWithValue("@Tipo", transacao.Tipo);
            cmd.Parameters.AddWithValue("@Data", transacao.Data);

            cmd.ExecuteNonQuery();
            transacao.Id = (int)cmd.LastInsertedId;
            return transacao;
        }

        public void Update(int id, Transacao transacao)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = @"UPDATE Transacoes 
                         SET descricao = @Descricao, 
                             valor = @Valor, 
                             tipo = @Tipo, 
                             data = @Data 
                         WHERE id = @Id";
            using var cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Descricao", transacao.Descricao);
            cmd.Parameters.AddWithValue("@Valor", transacao.Valor);
            cmd.Parameters.AddWithValue("@Tipo", transacao.Tipo);
            cmd.Parameters.AddWithValue("@Data", transacao.Data);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public bool Delete(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "DELETE FROM Transacoes WHERE id = @Id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}