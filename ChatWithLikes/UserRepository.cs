using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ChatWithLikes
{
    class UserRepository : IDisposable
    {
        private readonly SqlConnection _connection;
        private const string TableName = "Users";

        public UserRepository(string conStr) =>
            _connection = new SqlConnection(conStr ?? throw new ArgumentNullException(nameof(conStr)));

        public UserRepository(SqlConnection connection) =>
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        public void Dispose() => _connection.Dispose();


        public async Task<List<User>> GetUsersAsync()
        {
            var commandString = $"SELECT UserId, Username, Email FROM {TableName}";

            var command = new SqlCommand(commandString, _connection);
            try
            {
                await _connection.OpenAsync();

                var result = new List<User>();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new User
                        (
                            userId:   (int)reader["UserId"],
                            username: (string)reader["Username"],
                            email:    (string)reader["Email"]
                        ));
                    }
                }

                return result;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return new List<User>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<User>();
            }
            finally
            {
                _connection.Close();
            }

        }

        public async Task CreateUserAsync(User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            var query = $@"INSERT INTO {TableName} (UserId, Username, Email)
                           VALUES (@UserId, @Username, @Email)";

            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@UserId", user.UserId);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
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

        public async Task DeleteUserAsync(User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            var query = $"DELETE FROM {TableName} WHERE UserId = @DeletedId";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@DeletedId", user.UserId);
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

        public async Task UpdateRecordAsync(User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            var query = $@"UPDATE {TableName} 
                        SET Username = @Username, Email = @Email
                        WHERE UserId = @UpdatedId";
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@UpdatedId", user.UserId);
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
