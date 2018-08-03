using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ChatWithLikes
{
    class MessageRepository : IDisposable
    {
        private readonly SqlConnection _connection;
        private const string TableName = "Messages";

        public MessageRepository(string conStr) =>
            _connection = new SqlConnection(conStr ?? throw new ArgumentNullException(nameof(conStr)));

        public MessageRepository(SqlConnection connection) =>
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        public void Dispose() => _connection.Dispose();


        public async Task<List<Message>> GetMessages()
        {
            var commandString = $@"SELECT MessageId, Text, SenderId, ReplyMessageId, Date, Mark
                                   FROM {TableName}";

            var command = new SqlCommand(commandString, _connection);
            try
            {
                await _connection.OpenAsync();

                var result = new List<Message>();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new Message
                        (
                            messageId: (int)reader["MessageId"],
                            text: (string)reader["Text"],
                            senderId: (int)reader["SenderId"],
                            replyMessageId: reader["ReplyMessageId"] == DBNull.Value ? 0 : (int)reader["ReplyMessageId"],
                            date: (DateTime)reader["Date"],
                            mark: (int)reader["mark"]
                        ));
                    }
                }

                return result;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return new List<Message>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<Message>();
            }
            finally
            {
                _connection.Close();
            }

        }

        public async Task CreateMessage(Message message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var query = $@"INSERT INTO {TableName} (MessageId, Text, SenderId, ReplyMessageId, Date, Mark)
                           VALUES (@MessageId, @Text, @SenderId, @ReplyMessageId, @Date, @Mark)";

            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@MessageId", message.MessageId);
            command.Parameters.AddWithValue("@Text", message.Text);
            command.Parameters.AddWithValue("@SenderId", message.SenderId);
            command.Parameters.AddWithValue("@ReplyMessageId", message.ReplyMessageId);
            command.Parameters.AddWithValue("@Date", message.Date);
            command.Parameters.AddWithValue("@Mark", message.Mark);
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

        public async Task DeleteUser(Message message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var query = $"DELETE FROM {TableName} WHERE Id = @DeletedId";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@DeletedId", message.MessageId);
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

        public async Task UpdateRecord(Message message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var query = $@"UPDATE {TableName} 
                        SET Text = @Text, SenderId = @SenderId, ReplyMessageId = @ReplyMessageId, Date = @Date, Mark = @Mark
                        WHERE Id = @UpdatedId";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Text", message.Text);
            command.Parameters.AddWithValue("@SenderId", message.SenderId);
            command.Parameters.AddWithValue("@ReplyMessageId", message.ReplyMessageId);
            command.Parameters.AddWithValue("@Date", message.Date);
            command.Parameters.AddWithValue("@Mark", message.Mark);
            command.Parameters.AddWithValue("@UpdatedId", message.MessageId);

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
