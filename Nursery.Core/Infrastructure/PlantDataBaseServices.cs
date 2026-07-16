using Microsoft.Data.Sqlite;
using Nursery.Core.Common;
using Nursery.Core.Models;

namespace Nursery.Core.Infrastructure;

public static class PlantDatabaseServices
{
    private static readonly string ConnectionString =
        $"Data Source={DataPaths.NurseryDbFile}";

    public static bool UserHasPlantByName(string userId, string plantName)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(1)
            FROM UserNursery un
            JOIN Plant p ON un.PlantID = p.PlantID
            WHERE un.UserID = $userId AND p.Name = $name;
        ";
        command.Parameters.AddWithValue("$userId", userId);
        command.Parameters.AddWithValue("$name", plantName);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public static Plant? GetPlantByName(string name)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT PlantID, Name, Type, LifeCycle, FloweringStatus
            FROM Plant
            WHERE Name = $name;
        ";
        command.Parameters.AddWithValue("$name", name);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return null;

        return new Plant(
            reader.GetString(1),
            Enum.Parse<PlantType>(reader.GetString(2)),
            Enum.Parse<LifeCycleType>(reader.GetString(3)),
            reader.GetInt32(4) == 1
        ) { PlantID = reader.GetInt32(0) };
    }

    public static Plant? GetPlantById(int plantId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT PlantID, Name, Type, LifeCycle, FloweringStatus
            FROM Plant
            WHERE PlantID = $plantId;
        ";
        command.Parameters.AddWithValue("$plantId", plantId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
            return null;

        return new Plant(
            reader.GetString(1),
            Enum.Parse<PlantType>(reader.GetString(2)),
            Enum.Parse<LifeCycleType>(reader.GetString(3)),
            reader.GetInt32(4) == 1
        ) { PlantID = reader.GetInt32(0) };
    }

    private static int GetOrCreatePlantId(Plant plant)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var lookup = connection.CreateCommand();
        lookup.CommandText = "SELECT PlantID FROM Plant WHERE Name = $name;";
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

    public static bool RemovePlant(string userId, string plantName)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM UserNursery
            WHERE UserID = $userId
            AND PlantID = (SELECT PlantID FROM Plant WHERE Name = $name);
        ";
        command.Parameters.AddWithValue("$userId", userId);
        command.Parameters.AddWithValue("$name", plantName);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public static List<Plant> GetAllPlants()
    {
        var plants = new List<Plant>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT PlantID, Name, Type, LifeCycle, FloweringStatus FROM Plant;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            plants.Add(new Plant(
                reader.GetString(1),
                Enum.Parse<PlantType>(reader.GetString(2)),
                Enum.Parse<LifeCycleType>(reader.GetString(3)),
                reader.GetInt32(4) == 1
            ) { PlantID = reader.GetInt32(0) });
        }

        return plants;
    }

    public static List<Plant> GetPlantsForUser(string userId)
    {
        var plants = new List<Plant>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT p.PlantID, p.Name, p.Type, p.LifeCycle, p.FloweringStatus
            FROM Plant p
            JOIN UserNursery un ON p.PlantID = un.PlantID
            WHERE un.UserID = $userId;
        ";
        command.Parameters.AddWithValue("$userId", userId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            plants.Add(new Plant(
                reader.GetString(1),
                Enum.Parse<PlantType>(reader.GetString(2)),
                Enum.Parse<LifeCycleType>(reader.GetString(3)),
                reader.GetInt32(4) == 1
            ) { PlantID = reader.GetInt32(0) });
        }

        return plants;
    }
}