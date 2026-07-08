using Microsoft.AspNetCore.Mvc;
using Nursery.Core.Infrastructure;
using Nursery.Core.Models;

namespace Nursery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlantsController : ControllerBase
{
    // GET /api/plants
    // Returns the full plant catalog
    [HttpGet]
    public ActionResult<IEnumerable<Plant>> GetAll()
    {
        var plants = PlantDatabaseServices.GetAllPlants();
        return Ok(plants);
    }

    // GET /api/plants/{name}
    // Returns a specific plant from the catalog by name
    [HttpGet("{name}")]
    public ActionResult<Plant> GetByName(string name)
    {
        var plant = PlantDatabaseServices.GetPlantByName(name);
        return plant is null ? NotFound() : Ok(plant);
    }

    // GET /api/plants/nursery/{userId}
    // Returns all plants belonging to a specific user
    [HttpGet("nursery/{userId}")]
    public ActionResult<IEnumerable<Plant>> GetByUser(string userId)
    {
        var plants = PlantDatabaseServices.GetPlantsForUser(userId);
        return Ok(plants);
    }

    // POST /api/plants/nursery/{userId}
    // Adds a plant to a user's nursery (find-or-create in catalog)
    [HttpPost("nursery/{userId}")]
    public ActionResult AddToNursery(string userId, [FromBody] Plant plant)
    {
        if (PlantDatabaseServices.UserHasPlantByName(userId, plant.Name))
            return Conflict($"{plant.Name} is already in this user's nursery.");

        Plant? existingPlant = PlantDatabaseServices.GetPlantByName(plant.Name);
        if (existingPlant != null)
        {
            PlantDatabaseServices.AddPlant(userId, existingPlant);
            return Ok($"{plant.Name} already existed in the catalog — added to nursery with existing data.");
        }

        PlantDatabaseServices.AddPlant(userId, plant);
        return Created($"/api/plants/{plant.Name}", plant);
    }

    // DELETE /api/plants/nursery/{userId}/{plantName}
    // Removes a plant from a user's nursery (plant stays in catalog)
    [HttpDelete("nursery/{userId}/{plantName}")]
    public ActionResult RemoveFromNursery(string userId, string plantName)
    {
        bool removed = PlantDatabaseServices.RemovePlant(userId, plantName);
        return removed ? NoContent() : NotFound($"{plantName} was not found in this user's nursery.");
    }
}