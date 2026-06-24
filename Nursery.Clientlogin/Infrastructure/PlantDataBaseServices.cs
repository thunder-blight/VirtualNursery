using Microsoft.Data.Sqlite;
using Nursery.Clientlogin.Common;
using Nursery.Clientlogin.PresentationLayer.Models.Plants;
using Nusery.Clientlogin.PresentationLayer.Models.Plants;

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
            ConnectionString.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Plants (
                   PlantID         INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID          TEXT    NOT NULL,
                    Name            TEXT    NOT NULL,
                    Type            TEXT    NOT NULL CHECK (Type IN ('Tree','Shrub','Herb','Climber','Creeper')),
                    LifeCycle       TEXT    NOT NULL CHECK (LifeCycle IN ('Annual','Biennial','Perennial')),
                    FloweringStatus INTEGER NOT NULL DEFAULT 0 CHECK (FloweringStatus IN (0,1)) 
                );
                CREATE INDEX IF NOT EXISTS idx_plants_userid ON Plants(UserID);
            ";
            command.ExecuterNonQuery();
        }

        public static void AddPlant(string userId, Plant plant)
        {
            using var connection = new SqliteConnection(ConnectionString);
            conection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Plants (UserID, Name, Type, LifeCycle, FloweringStatus)
                VALUES ($userId, $name, $type, $lifeCycle, $flowering);
            ";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$name", plant.Name);
            command.Parameters.AddWithValue("$type", plant.Type.ToString());
            command.Parameters.AddWithValue("$lifeCycle", plant.LifeCycle.ToString());
            command.Parameters.AddWithValue("$flowering", plant.FloweringStatus ? 1 : 0);

            command.ExecturNonQuery();
        }

        public static List<Plant> GetPlantsForUser(string userId)
        {
            var plants = new List<Plant>();
            
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Name, Type, LifeCycle, FloweringStatus
                FROM Plants
                WHERE UserID = $userId;
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