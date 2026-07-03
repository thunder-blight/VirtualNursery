using Microsoft.Data.Sqlite;

namespace Nursery.Clientlogin.Infrastructure
{
    public static class DatabaseServices
    {
        private static readonly string ConnectionString =
            $"Data Source={DataPaths.NurseryDbFile}";

        public static void Initialize()
        {
            if (!Directory.Exists(DataPaths.DataDirectory))
                Directory.CreateDirectory(DataPaths.DataDirectory);
            
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID       TEXT PRIMARY KEY,
                    Username     TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Role         TEXT NOT NULL CHECK (Role IN ('Admin','Standard'))
                );

                CREATE TABLE IF NOT EXISTS Plant (
                    PlantID         INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name            TEXT    NOT NULL UNIQUE,
                    Type            TEXT    NOT NULL CHECK (Type IN ('Tree','Shrub','Herb','Climber','Creeper')),
                    LifeCycle       TEXT    NOT NULL CHECK (LifeCycle IN ('Annual','Biennial','Perennial')),
                    FloweringStatus INTEGER NOT NULL DEFAULT 0 CHECK (FloweringStatus IN (0,1))
                );

                CREATE TABLE IF NOT EXISTS UserNursery (
                    UserID  TEXT    NOT NULL,
                    PlantID INTEGER NOT NULL,
                    PRIMARY KEY (UserID, PlantID),
                    FOREIGN KEY (UserID)  REFERENCES Users(UserID),
                    FOREIGN KEY (PlantID) REFERENCES Plant(PlantID)
                );
            ";
            command.ExecuteNonQuery();
        }
    }
};

