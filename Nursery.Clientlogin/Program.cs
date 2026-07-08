using Nursery.Core.Infrastructure;
using Nursery.Core.Models;
using Nursery.Clientlogin.Infrastructure;
using Nursery.Clientlogin.PresentationLayer.Menus;
using Nursery.Clientlogin.Services;

class Program
{
    static void Main()
    {
        DatabaseServices.Initialize();
        
        Console.WriteLine("Welcome to the Virtual Nursery!");
        Console.WriteLine();

        User? currentUser = null;

        while (currentUser == null)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.WriteLine("Choose an option: ");
            
            string loginChoice = Console.ReadLine()?.TrimEnd() ?? "";

            switch (loginChoice)
            {
                case "1":
                {
                    var user = LoginMenu.RegisterUser();
                    if (user != null)
                        currentUser = user;
                    break;
                }

                case "2":
                {
                    var user = LoginMenu.LoginUser();
                    if (user != null)
                        currentUser = user;
                    break;
                }

                case "3":
                {
                    Console.WriteLine("Goodbye!");
                    return;
                }

                default:
                {
                    Console.WriteLine("Invalid option.");
                    continue;
                }
            }
            
            Console.WriteLine();
            
            if (currentUser == null)
                {
                Console.WriteLine("Login failed. Returning to menu.");
                continue;
            }

            Console.WriteLine($"Logged in as: {currentUser.Username}");
            Console.WriteLine();

            List<Plant> plants = NurseryApiClient.GetPlantsForUser(currentUser.UserID);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1. Add a new plant");
                Console.WriteLine("2. View plants");
                Console.WriteLine("3. Logout");
                Console.Write("Choose an option: ");

                string plantChoice = Console.ReadLine()?.TrimEnd() ?? "";

                switch (plantChoice)
                {
                    case "1":
                    {
                        Console.Write("Enter plant name: ");
                        string name = (Console.ReadLine() ?? "").Trim();

                        if (plants.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine($"{name} is already in your nursery.");
                            break;
                        }

                        Plant? existingPlant = NurseryApiClient.GetPlantByName(name);
                        if (existingPlant != null)
                        {
                            Console.WriteLine($"{name} already exists in the database — adding it to your nursery with existing information.");
                            NurseryApiClient.AddPlant(currentUser.UserID, existingPlant);
                            plants.Add(existingPlant);
                            break;
                        }

                        Plant plant = PlantMenu.CreatePlant(name);
                        NurseryApiClient.AddPlant(currentUser.UserID, plant);
                        plants.Add(plant);
                        Console.WriteLine($"Added {plant.Name} to your nursery.");
                        break;
                    }
                    case "2":
                        if (plants.Count == 0)
                        {
                            Console.WriteLine("No plants in the nursery yet.");
                        }
                        else
                        {
                            Console.WriteLine("Plants in your nursery:");
                            foreach (var p in plants)
                            {
                                Console.WriteLine(
                                    $"- {p.Name} | {p.Type} | {p.LifeCycle} | {(p.FloweringStatus ? "Yes" : "No")}"
                                );
                            }
                        }
                        break;
                    case "3":
                        Console.WriteLine("Logging out...");
                        currentUser = null;
                        Main();
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        continue;
                }
            }
        }
    }
}