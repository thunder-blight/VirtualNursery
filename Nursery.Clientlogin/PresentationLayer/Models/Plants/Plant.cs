using Nursery.Clientlogin.Common;

namespace Nursery.Clientlogin.PresentationLayer.Models.Plants;

public class Plant
{
    public string Name { get; set; }
    public PlantType Type { get; set; }
    public LifeCycleType LifeCycle { get; set; }
    public bool FloweringStatus { get; set; }

    public Plant()
    {
    }
    
    public Plant(string name, PlantType plantType, LifeCycleType lifeCycle, bool floweringStatus)
    {
        Name = name;
        Type = plantType;
        LifeCycle = lifeCycle;
        FloweringStatus = floweringStatus;
    }

    public static Plant CreatePlant()
    {
        Console.WriteLine("Enter plant name: ");
        string name = Console.ReadLine().Trim();
        
        Console.WriteLine("What type is it? (tree, shrub, herb, climber, creeper): ");
        PlantType plantType = Enum.Parse<PlantType>(
            Console.ReadLine().Trim(),
            ignoreCase: true
            );
        
        Console.WriteLine("What is its life cycle? (annual, biennial, perennial): ");
        LifeCycleType lifeCycle = Enum.Parse<LifeCycleType>(
            Console.ReadLine().Trim(),
            ignoreCase: true
        );

        bool floweringStatus;
        while (true)
        {
            Console.WriteLine("Is it flowering? (yes/no): ");
            string input = (Console.ReadLine() ?? "").Trim().ToLower();

            if (input == "yes" || input == "y")
            {
                floweringStatus = true;
                break;
            }
            if (input == "no" || input == "n")
            {
                floweringStatus =  false;
                break;
            }
            Console.WriteLine("Please enter yes or no.");
        }
        return new Plant(name, plantType, lifeCycle, floweringStatus);
    }
}
