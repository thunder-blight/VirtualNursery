using Nursery.Core.Common;
using Nursery.Core.Models;

namespace Nursery.Clientlogin.PresentationLayer.Menus;

public static class PlantMenu
{
    public static Plant CreatePlant(string name)
    {
        PlantType plantType;
        while (true)
        {
            Console.WriteLine("What type is it? (tree, shrub, herb, climber, creeper): ");
            if (Enum.TryParse<PlantType>(Console.ReadLine()?.Trim(), ignoreCase: true, out plantType))
                break;
            Console.WriteLine("Invalid type. Please enter: tree, shrub, herb, climber, or creeper.");
        }

        LifeCycleType lifeCycle;
        while (true)
        {
            Console.WriteLine("What is its life cycle? (annual, biennial, perennial): ");
            if (Enum.TryParse<LifeCycleType>(Console.ReadLine()?.Trim(), ignoreCase: true, out lifeCycle))
                break;
            Console.WriteLine("Invalid life cycle. Please enter: annual, biennial, or perennial.");
        }

        bool floweringStatus;
        while (true)
        {
            Console.WriteLine("Is it flowering? (yes/no): ");
            string input = (Console.ReadLine() ?? "").Trim().ToLower();

            if (input == "yes" || input == "y") { floweringStatus = true; break; }
            if (input == "no" || input == "n") { floweringStatus = false; break; }
            Console.WriteLine("Please enter yes or no.");
        }

        return new Plant(name, plantType, lifeCycle, floweringStatus);
    }
}