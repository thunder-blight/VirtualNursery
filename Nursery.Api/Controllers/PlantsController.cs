using Microsoft.AspNetCore.Mvc;
using Nursery.Core.Infrastructure;
using Nursery.Core.Models;

namespace Nursery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlantsController : ControllerBase
{
    // GET /api/plants
    [HttpGet]
    public ActionResult<IEnumerable<Plant>> GetAll()
    {
        var plants = PlantDatabaseServices.GetAllPlants();
        return Ok(plants);
    }

    // GET /api/plants/name/{name}
    [HttpGet("name/{name}")]
    public ActionResult<Plant> GetByName(string name)
    {
        var plant = PlantDatabaseServices.GetPlantByName(name);
        return plant is null ? NotFound() : Ok(plant);
    }

    // GET /api/plants/id/{plantId}
    [HttpGet("id/{plantId}")]
    public ActionResult<Plant> GetById(int plantId)
    {
        var plant = PlantDatabaseServices.GetPlantById(plantId);
        return plant is null ? NotFound() : Ok(plant);
    }

    // GET /api/plants/nursery/{userId}
    [HttpGet("nursery/{userId}")]
    public ActionResult<IEnumerable<Plant>> GetByUser(string userId)
    {
        var plants = PlantDatabaseServices.GetPlantsForUser(userId);
        return Ok(plants);
    }

    // POST /api/plants/nursery/{userId}
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
        return Created($"/api/plants/id/{plant.PlantID}", plant);
    }
    
    // PUT /api/plants/id/{plantId}
    [HttpPut("id/{plantId}")]
    public ActionResult UpdatePlant(int plantId, [FromBody] Plant plant)
    {
        var existing = PlantDatabaseServices.GetPlantById(plantId);
        if (existing is null)
            return NotFound($"Plant with ID {plantId} not found.");
        
        bool updated = PlantDatabaseServices.UpdatePlant(plantId, plant);
        return updated ? Ok(plant) : StatusCode(500, "Failed to update plant.");
    }

    // DELETE /api/plants/nursery/{userId}/{plantName}
    [HttpDelete("nursery/{userId}/{plantName}")]
    public ActionResult RemoveFromNursery(string userId, string plantName)
    {
        bool removed = PlantDatabaseServices.RemovePlant(userId, plantName);
        return removed ? NoContent() : NotFound($"{plantName} was not found in this user's nursery.");
    }
    
    // POST /api/plants/id/{plantId}
    [HttpPost]
    public ActionResult<Plant> CreatePlant([FromBody] Plant plant)
    {
        var created = PlantDatabaseServices.CreatePlant(plant);
        if (created is null)
            return Conflict($"Aplant named '{plant.Name}' already exists.");
        return Created($"/api/plants/id/{created.PlantID}", created);
    }
    
    // DELETE /api/plants/id/{plantId}
    [HttpDelete("id/{plantId}")]
    public ActionResult DeletePlant(int plantId)
    {
        var existing = PlantDatabaseServices.GetPlantById(plantId);
        if (existing is null)
            return NotFound($"Plant with ID {plantId} not found.");
        
        bool deleted = PlantDatabaseServices.DeletePlant(plantId);
        return deleted ? NoContent() : StatusCode(500, "Failed to delete plant.");
    }
}