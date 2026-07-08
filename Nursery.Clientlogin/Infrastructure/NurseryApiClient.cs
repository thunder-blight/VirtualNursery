using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nursery.Core.Models;

namespace Nursery.Clientlogin.Infrastructure;

public static class NurseryApiClient
{
    private const string BaseUrl = "https://localhost:7288";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    private static readonly HttpClient HttpClient = new();

    public static List<Plant> GetPlantsForUser(string userId)
    {
        var response = HttpClient
            .GetAsync($"{BaseUrl}/api/plants/nursery/{userId}")
            .GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
            return new List<Plant>();

        return response.Content
            .ReadFromJsonAsync<List<Plant>>(JsonOptions)
            .GetAwaiter().GetResult() ?? new List<Plant>();
    }

    public static Plant? GetPlantByName(string name)
    {
        var response = HttpClient
            .GetAsync($"{BaseUrl}/api/plants/{name}")
            .GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
            return null;

        return response.Content
            .ReadFromJsonAsync<Plant>(JsonOptions)
            .GetAwaiter().GetResult();
    }

    public static bool AddPlant(string userId, Plant plant)
    {
        var response = HttpClient.PostAsJsonAsync(
            $"{BaseUrl}/api/plants/nursery/{userId}",
            plant,
            JsonOptions
        ).GetAwaiter().GetResult();

        return response.IsSuccessStatusCode;
    }
}