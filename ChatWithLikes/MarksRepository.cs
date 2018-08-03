using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ChatWithLikes
{
    class MarksRepository : IDisposable
    {
        private readonly SqlConnection _connection;
        private const string TableName = "Likes";

        public MarksRepository(string conStr) 
            : this(new SqlConnection(conStr ?? throw new ArgumentNullException(nameof(conStr))))
        { }

        public MarksRepository(SqlConnection connection) =>
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        public void Dispose() => _connection.Dispose();


        public async Task<List<Mark>> GetMarksAsync()
        {
            var commandString = $@"SELECT MessageId, UserId, Mark
                                   FROM {TableName}";

            var command = new SqlCommand(commandString, _connection);
            try
            {
                await _connection.OpenAsync();

                var result = new List<Mark>();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new Mark
                        (
                            messageId: (int)reader["MessageId"],
                            userId: (int)reader["UserId"],
                            value: (int)reader["Mark"]
                        ));
                    }
                }

                return result;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return new List<Mark>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<Mark>();
            }
            finally
            {
                _connection.Close();
            }

        }

        public async Task CreateMarkAsync(Mark mark)
        {
            if (mark is null) throw new ArgumentNullException(nameof(mark));

            var query = $@"INSERT INTO {TableName} (MessageId, UserId, Mark)
                           VALUES (@MessageId, @UserId, @Mark)";

            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@MessageId", mark.MessageId);
            command.Parameters.AddWithValue("@UserId", mark.UserId);
            command.Parameters.AddWithValue("@Mark", mark.Value);
            try
            {
                await _connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task DeleteMarkAsync(Mark mark)
        {
            if (mark is null) throw new ArgumentNullException(nameof(mark));

            var query = $"DELETE FROM {TableName} WHERE MessageId = @MessageId AND UserId = @UserId";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@MessageId", mark.MessageId);
            command.Parameters.AddWithValue("@UserId", mark.UserId);
            try
            {
                await _connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task UpdateMarkAsync(Mark mark)
        {
            if (mark is null) throw new ArgumentNullException(nameof(mark));

            var query = $@"UPDATE {TableName} 
                        SET Mark = @Mark
                        WHERE MessageId = @MessageId AND UserId = @UserId";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Mark", mark.Value);
            command.Parameters.AddWithValue("@MessageId", mark.MessageId);
            command.Parameters.AddWithValue("@UserId", mark.UserId);

            try
            {
                await _connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
