using System.Text.Json;
using Nursery.Clientlogin.PresentationLayer.Models.Plants;

namespace Nursery.Clientlogin.Infrastructure
{
    public static class UserPlantDataServices
    {
        private static readonly string PlantFolder =
            Path.Combine(DataPaths.DataDirectory, "plants");

        public static void InitializeUserPlants(string userId)
        {
            if (!Directory.Exists(PlantFolder))
                Directory.CreateDirectory(PlantFolder);

            string path = GetUserPlantFile(userId);

            if (!File.Exists(path))
                File.WriteAllText(path, "[]");
        }

        public static List<Plant> LoadPlants(string userId)
        {
            string path = GetUserPlantFile(userId);
            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<List<Plant>>(json) ?? new();
        }

        public static void SavePlants(string userId, List<Plant> plants)
        {
            string path = GetUserPlantFile(userId);

            string json = JsonSerializer.Serialize(plants, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }

        private static string GetUserPlantFile(string userId)
        {
            return Path.Combine(PlantFolder, $"{userId}.json");
        }
    }
}
