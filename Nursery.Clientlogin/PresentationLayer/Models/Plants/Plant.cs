namespace Nursery.Clientlogin.PresentationLayer.Models.Plants;

public class Plant
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string LifeCycle { get; set; }
    public bool FloweringStatus { get; set; }

    public Plant(string name, string type, string lifeCycle, bool floweringStatus)
    {
        Name = name;
        Type = type;
        LifeCycle = lifeCycle;
        FloweringStatus = floweringStatus;
    }

    public static Plant CreatePlant()
    {
        Console.WriteLine("Enter plant name: ");
        string name = Console.ReadLine().Trim();
        
        Console.WriteLine("What type is it? (tree, shrub, herb, climber, creeper): ");
        string type = Console.ReadLine().Trim();
        
        Console.WriteLine("What is its life cycle? (annual, biennial, perennial): ");
        string lifeCycle = Console.ReadLine().Trim();
        
        Console.WriteLine("Is it flowering? (True/False): ");
        bool floweringStatus = bool.Parse(Console.ReadLine().Trim());
        
        return new Plant(name, type, lifeCycle, floweringStatus);
    }
}
