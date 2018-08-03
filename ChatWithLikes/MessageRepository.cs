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

        public MessageRepository(string conStr) 
            : this(new SqlConnection(conStr ?? throw new ArgumentNullException(nameof(conStr))))
        { }

        public MessageRepository(SqlConnection connection) =>
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        public void Dispose() => _connection.Dispose();


        public async Task<List<Message>> GetMessagesAsync()
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
                            replyMessageId: reader["ReplyMessageId"] as int? ?? default(int),
                            date: (DateTime)reader["Date"],
                            mark: reader["mark"] as int? ?? default(int)
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

        public async Task CreateMessageAsync(Message message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var query = $@"INSERT INTO {TableName} (Text, SenderId, ReplyMessageId, Date, Mark)
                           VALUES (@Text, @SenderId, @ReplyMessageId, @Date, @Mark)";

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

        public async Task DeleteMessageAsync(Message message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var query = $"DELETE FROM {TableName} WHERE MessageId = @DeletedId";
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

        public async Task UpdateMessageAsync(Message message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var query = $@"UPDATE {TableName} 
                        SET Text = @Text, SenderId = @SenderId, ReplyMessageId = @ReplyMessageId, Date = @Date, Mark = @Mark
                        WHERE MessageId = @UpdatedId";
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
