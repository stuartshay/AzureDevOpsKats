using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AzureDevOpsKats.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsKats.Data.Repository
{
    public class CatRepository : ICatRepository
    {
        private readonly SqliteConnection _dbConnection;

        private readonly string _connectionString; 


        private readonly ILogger<CatRepository> _logger;

        public CatRepository(string connection, ILogger<CatRepository> logger)
        {
            _logger = logger;
            _connectionString = connection;
            _dbConnection = GetSqliteConnection(connection);
        }

        public async Task<IEnumerable<Cat>> GetCats()
        {
            await Open();

            List<Cat> cats = new List<Cat>();
            await using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Description, Photo FROM Cats ORDER BY Name COLLATE NOCASE ASC;";
                var result = await command.ExecuteReaderAsync();
                while (result.Read())
                {
                    var cat = new Cat
                    {
                        Id = result.GetInt32(0),
                        Name = result.GetString(1) != null ? result.GetString(1) : null,
                        Description = result.GetString(2) != null ? result.GetString(2) : null,
                        Photo = result.GetString(3) != null ? result.GetString(3) : null,
                    };

                    cats.Add(cat);
                }
            }

            return cats;
        }

        public async Task<IEnumerable<Cat>> GetCats(int limit, int offset)
        {
            await Open();

            List<Cat> cats = new List<Cat>();
            await using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Description, Photo FROM Cats " +
                                      "ORDER BY Name COLLATE NOCASE ASC LIMIT @param1 OFFSET @param2;";

                command.Parameters.Add(new SqliteParameter("@param1", limit));
                command.Parameters.Add(new SqliteParameter("@param2", offset));

                var result = await command.ExecuteReaderAsync();
                while (result.Read())
                {
                    var cat = new Cat
                    {
                        Id = result.GetInt32(0),
                        Name = result.GetString(1) != null ? result.GetString(1) : null,
                        Description = result.GetString(2) != null ? result.GetString(2) : null,
                        Photo = result.GetString(3) != null ? result.GetString(3) : null,
                    };

                    cats.Add(cat);
                }
            }

            _logger.LogInformation($"Repository Total Cats:{{catsCount}}|Limit:{limit}|Offset{offset}", cats.Count, limit, offset);

            return cats;
        }

        public async Task<long> GetCount()
        {
            await Open();
            await using var command = _dbConnection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Cats";

            try
            {
                var result = await command.ExecuteScalarAsync();
                var item = result ?? 0;

                return (long)item;
            }
            catch (Exception ex)
            {
                _logger.LogError("GET COUNT FAILED", ex.Message);
                return -1;
            }
        }

        public async Task<Cat> GetCat(long id)
        {
            await Open();
            await using var command = _dbConnection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Description, Photo FROM Cats WHERE Id = @param1;";
            command.Parameters.Add(new SqliteParameter("@param1", id));

            var result = await command.ExecuteReaderAsync();
            if (!result.HasRows)
            {
                return null;
            }

            result.Read();
            var cat = new Cat
            {
                Id = result.GetInt32(0),
                Name = result.GetString(1),
                Description = result.GetString(2),
                Photo = result.GetString(3),
            };

            return cat;
        }

        public async Task EditCat(Cat cat)
        {
            using (_dbConnection)
            {
                await Open();
                await using var command = _dbConnection.CreateCommand();
                command.CommandText = "UPDATE Cats SET Name = @param1, Description= @param2 WHERE Id = @param3;";
                command.Parameters.Add(new SqliteParameter("@param1", cat.Name));
                command.Parameters.Add(new SqliteParameter("@param2", cat.Description));
                command.Parameters.Add(new SqliteParameter("@param3", cat.Id));

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<long> CreateCat(Cat cat)
        {
            await using (_dbConnection)
            {
                await Open();
                await using var command = _dbConnection.CreateCommand();
                command.CommandText = "INSERT INTO Cats(Name, Description, Photo) VALUES (@param1, @param2, @param3)";
                command.Parameters.Add(new SqliteParameter("@param1", cat.Name));
                command.Parameters.Add(new SqliteParameter("@param2", cat.Description));
                command.Parameters.Add(new SqliteParameter("@param3", cat.Photo));

                command.ExecuteScalar();
                command.CommandText = "SELECT last_insert_rowid()";
                var result = await command.ExecuteScalarAsync();

                return (long)result;
            }
        }

        public async Task DeleteCat(long id)
        {
            await using (_dbConnection)
            {
                await Open();
                await using var command = _dbConnection.CreateCommand();
                command.CommandText = "DELETE FROM Cats WHERE Id=@param1";
                command.Parameters.Add(new SqliteParameter("@param1", id));

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Open()
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                await _dbConnection.OpenAsync();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _dbConnection.CloseAsync();
            _dbConnection.Dispose();
        }

        private SqliteConnection GetSqliteConnection(string connectionString)
        {
            var connection = new SqliteConnection(connectionString);
            return connection;
        }
    }
}
