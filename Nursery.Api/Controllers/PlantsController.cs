using Microsoft.AspNetCore.Mvc;

namespace Nursery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlantsController : ControllerBase
{
    private static readonly List<Plant> Plants = new()
    {
        new Plant { Id = 1, Name = "Basil", Species = "Ocimum basilicum" },
        new Plant { Id = 2, Name = "Aloe", Species = "Aloe vera" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Plant>> GetAll()
    {
        return Ok(Plants);
    }

    [HttpGet("{id}")]
    public ActionResult<Plant> GetById(int id)
    {
        var plant = Plants.FirstOrDefault(p => p.Id == id);
        return plant is null ? NotFound() : Ok(plant);
    }
    
    [HttpPost]
    public ActionResult<Plant> Create(Plant newPlant)
    {
        newPlant.Id = Plants.Max(p => p.Id) + 1;
        Plants.Add(newPlant);
        return CreatedAtAction(nameof(GetById), new { id = newPlant.Id }, newPlant);
    }
}

public class Plant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
}