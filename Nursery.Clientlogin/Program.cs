using System;
using System.Collections.Generic;
using Nursery.Clientlogin.Infrastructure;
using Nursery.Clientlogin.PresentationLayer.Models.Plants;
using Nursery.Clientlogin.PresentationLayer.Menus;
using Nursery.Clientlogin.Models;

class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to the Virtual Nursery!");
        Console.WriteLine();
        
        User? currentUser = null;
        
        //Auth loop
        while (currentUser == null)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            
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
                    Console.WriteLine("Goodbye!");
                    return;
                
                default:
                    Console.WriteLine("Invalid option!");
                    continue;
            }
            
            Console.WriteLine();
            
            if (currentUser == null)
            {
                Console.WriteLine("Login failed. Returning to menu.");
                continue;
            }
            
            Console.WriteLine($"Logged in as: {currentUser.Username}");
            Console.WriteLine();
            
            //User nursery
            List<Plant> plants = UserPlantDataServices.LoadPlants(currentUser.UserID);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1. Add a new plant");
                Console.WriteLine("2. View plants");
                Console.WriteLine("3. Logout");
                Console.Write("Choose an option: ");
                
                string plantChoice =  Console.ReadLine()?.TrimEnd() ?? "";

                switch (plantChoice)
                {
                    case "1":
                    {
                        Plant plant = Plant.CreatePlant();
                        plants.Add(plant);
                        
                        UserPlantDataServices.SavePlants(currentUser.UserID, plants);
                        
                        Console.WriteLine($"Added plant: {plant.Name}");
                        break;
                    }
                    
                    case "2":
                        if (plants.Count == 0)
                        {
                            Console.WriteLine("No plants in the nursery yet.");
                        }
                        else
                        {
                            Console.WriteLine($"Plants in your nursery:");
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