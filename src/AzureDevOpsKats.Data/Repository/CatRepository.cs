using System.Collections.Generic;
using System.Data;
using AzureDevOpsKats.Data.Entities;
using Microsoft.Data.Sqlite;

namespace AzureDevOpsKats.Data.Repository
{
    public class CatRepository : ICatRepository
    {
        private readonly SqliteConnection _dbConnection;

        public CatRepository(string connection)
        {
            _dbConnection = new SqliteConnection(connection);
        }

        public IEnumerable<Cat> GetCats()
        {
            Open();

            List<Cat> cats = new List<Cat>();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT Id,Name, Description,Photo FROM Cats ORDER BY Name;";
                var result = command.ExecuteReader();
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

        public Cat GetCat(int id)
        {
            Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "SELECT Id,Name,Description,Photo FROM Cats WHERE Id = @param1;";
                command.Parameters.Add(new SqliteParameter("@param1", id));

                var result = command.ExecuteReader();
                if (result == null)
                    return null;

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
        }

        public void EditCat(Cat cat)
        {
            using (_dbConnection)
            {
                Open();
                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = "UPDATE Cats SET Name = @param1, Description= @param2 WHERE Id = @param3;";
                    command.Parameters.Add(new SqliteParameter("@param1", cat.Name));
                    command.Parameters.Add(new SqliteParameter("@param2", cat.Description));
                    command.Parameters.Add(new SqliteParameter("@param3", cat.Id));

                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateCat(Cat cat)
        {
            using (_dbConnection)
            {
                Open();
                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Cats(Name, Description, Photo) VALUES (@param1, @param2, @param3)";
                    command.Parameters.Add(new SqliteParameter("@param1", cat.Name));
                    command.Parameters.Add(new SqliteParameter("@param2", cat.Description));
                    command.Parameters.Add(new SqliteParameter("@param3", cat.Photo));

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCat(int id)
        {
            using (_dbConnection)
            {
                Open();
                using (var command = _dbConnection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Cats WHERE Id=@param1";
                    command.Parameters.Add(new SqliteParameter("@param1", id));
                    command.ExecuteNonQuery();
                }
            };
        }

        public void Open()
        {
            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.OpenAsync();
        }

        public void Dispose()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }
    }
}
