using eastwest.Data;
using eastwest.Models;
using eastwest.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eastwest.Controllers
{
    [Authorize]
    [Route("api/location")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public LocationController(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // [HttpGet("getList")]
        // public async Task<IActionResult> findAll()
        // {
        //     var locationRepo = new LocationRepo(_context, _configuration);

        //     var findAllLocation = await locationRepo.findAll();

        //     if (findAllLocation != null)
        //     {
        //         return Ok(findAllLocation);
        //     }
        //     else
        //     {
        //         return NotFound();
        //     }
        // }

        // [HttpPost("createLocation")]
        // public async Task<IActionResult> createNewLocation(LocationModel location)
        // {
        //     var locationRepo = new LocationRepo(_context, _configuration);

        //     // var newLocation = await locationRepo.createLocation(location);

        //     return Ok(newLocation);
        // }

        // [HttpPost("updateLocation")]
        // public async Task<IActionResult> updateLocation(int locationId, LocationModel location)
        // {
        //     var locationRepo = new LocationRepo(_context, _configuration);

        //     var findLocation = await locationRepo.findById(locationId);

        //     if (findLocation != null)
        //     {
        //         var dataLocationUpdate = new LocationModel
        //         {
        //             Loc_Barcodes = location.Loc_Barcodes
        //         };

        //         var editLocation = await locationRepo.updateLocation(locationId, dataLocationUpdate);

        //         return Ok(editLocation);
        //     }
        //     else
        //     {
        //         return NotFound("Location not found");
        //     }
        // }

        // [HttpDelete("deleteLocation")]
        // public async Task<IActionResult> deleteLocation(int locationId)
        // {
        //     var locationRepo = new LocationRepo(_context, _configuration);

        //     var findLocation = await locationRepo.findById(locationId);

        //     if (findLocation == null)
        //     {
        //         return NotFound("Location not found");
        //     }

        //     var deleteLocation = await locationRepo.deleteLocation(locationId);

        //     return Ok(deleteLocation);
        // }
    }
}
