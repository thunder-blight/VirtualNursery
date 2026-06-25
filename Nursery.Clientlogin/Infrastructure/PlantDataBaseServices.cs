using Microsoft.Data.Sqlite;
using Nursery.Clientlogin.Common;
using Nursery.Clientlogin.PresentationLayer.Models.Plants;

namespace Nursery.Clientlogin.Infrastructure
{
    public static class PlantDatabaseServices
    {
        private static readonly string ConnectionString =
            $"Data Source={DataPaths.PlantsDbFile}";

        public static void Initialize()
        {
            if (!Directory.Exists(DataPaths.DataDirectory))
                Directory.CreateDirectory(DataPaths.DataDirectory);
            
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
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
                    FOREIGN KEY (PlantID) REFERENCES Plant(PlantID)
                );
            ";
            command.ExecuteNonQuery();
        }

        private static int GetOrCreatePlantId(Plant plant)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            var lookup = connection.CreateCommand();
            lookup.CommandText = @"SELECT PlantID FROM Plant WHERE Name = $name;";
            lookup.Parameters.AddWithValue("$name", plant.Name);
            
            var existingId = lookup.ExecuteScalar();
            if (existingId != null)
                return Convert.ToInt32(existingId);
            
            var insert = connection.CreateCommand();
            insert.CommandText = @"
                INSERT INTO Plant (Name, Type, LifeCycle, FloweringStatus)
                VALUES ($name, $type, $lifeCycle, $flowering);
                SELECT last_insert_rowid();
            ";
            insert.Parameters.AddWithValue("$name", plant.Name);
            insert.Parameters.AddWithValue("$type", plant.Type.ToString());
            insert.Parameters.AddWithValue("$lifeCycle", plant.LifeCycle.ToString());
            insert.Parameters.AddWithValue("$flowering", plant.FloweringStatus ? 1 : 0);

            return Convert.ToInt32(insert.ExecuteScalar());
        }

        public static bool AddPlant(string userId, Plant plant)
        {
            int plantId = GetOrCreatePlantId(plant);
            
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO UserNursery (UserID, PlantID)
                VALUES ($userId, $plantId);
            ";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$plantId", plantId);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public static List<Plant> GetPlantsForUser(string userId)
        {
            var plants = new List<Plant>();
            
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT p.Name, p.Type, p.LifeCycle, p.FloweringStatus
                FROM Plant p
                JOIN UserNursery un ON p.PlantID = un.PlantID
                WHERE un.UserID = $userId;
            ";
            command.Parameters.AddWithValue("$userId", userId);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string name = reader.GetString(0);
                var type = Enum.Parse<PlantType>(reader.GetString(1));
                var lifeCycle = Enum.Parse<LifeCycleType>(reader.GetString(2));
                bool flowering = reader.GetInt32(3) == 1;
                
                plants.Add(new Plant(name, type, lifeCycle, flowering));
            }

            return plants;
        }
    }
}