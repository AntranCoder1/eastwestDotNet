using eastwest.Data;
using eastwest.Models;
using eastwest.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using eastwest.ClassValue;
using System.Text;
using OfficeOpenXml;

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

        [HttpGet("getList")]
        public async Task<IActionResult> findAll()
        {
            var locationRepo = new LocationRepo(_context, _configuration);

            var findAllLocation = await locationRepo.findAll();

            if (findAllLocation != null)
            {
                return Ok(findAllLocation);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("createLocation")]
        public async Task<IActionResult> createNewLocation()
        {
            var locationRepo = new LocationRepo(_context, _configuration);

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            LocationValue location = JsonConvert.DeserializeObject<LocationValue>(rawContent);

            var findLoc = await locationRepo.findWithLoc(location.Loc_Barcodes);

            if (findLoc == null)
            {
                var newLocation = await locationRepo.createLocation(location);

                return Ok(new { status = "success", message = "create location successfully.", data = newLocation });
            }
            else
            {
                return BadRequest(new { status = "failed", message = "location has been exist" });
            }

        }

        [HttpPost("updateLocation")]
        public async Task<IActionResult> updateLocation(int locationId, LocationModel location)
        {
            var locationRepo = new LocationRepo(_context, _configuration);

            var findLocation = await locationRepo.findById(locationId);

            if (findLocation != null)
            {
                var dataLocationUpdate = new LocationModel
                {
                    Loc_Barcodes = location.Loc_Barcodes
                };

                var editLocation = await locationRepo.updateLocation(locationId, dataLocationUpdate);

                return Ok(editLocation);
            }
            else
            {
                return NotFound("Location not found");
            }
        }

        [HttpDelete("deleteLocation")]
        public async Task<IActionResult> deleteLocation(int locationId)
        {
            var locationRepo = new LocationRepo(_context, _configuration);

            var findLocation = await locationRepo.findById(locationId);

            if (findLocation == null)
            {
                return NotFound("Location not found");
            }

            var deleteLocation = await locationRepo.deleteLocation(locationId);

            return Ok(deleteLocation);
        }

        [HttpPost("importFile")]
        public async Task<IActionResult> importFileLocation(IFormFile file)
        {
            try
            {
                var locationRepo = new LocationRepo(_context, _configuration);

                string host = "locationManagerment/getFile/";
                string urls = host + file.FileName;
                string url = Path.Combine(Directory.GetCurrentDirectory(), "Upload/File", file.FileName);

                using (var stream = new FileStream(url, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var parsedData = new List<LocationValue>();

                using (var package = new ExcelPackage(new FileInfo(url)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var location = new LocationValue();

                        for (int col = 1; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value?.ToString();

                            if (cellValue != null)
                            {
                                switch (worksheet.Cells[1, col].Value?.ToString())
                                {
                                    case "location":
                                        location.Loc_Barcodes = cellValue;
                                        break;
                                }
                            }
                        }

                        parsedData.Add(location);
                    }

                    if (parsedData.Count > 0)
                    {
                        foreach (var i in parsedData)
                        {
                            var findLocation = await locationRepo.findWithLoc(i.Loc_Barcodes);

                            if (findLocation == null)
                            {
                                var dataLocation = new LocationValue
                                {
                                    Loc_Barcodes = i.Loc_Barcodes
                                };

                                var createNewLocation = await locationRepo.createLocation(dataLocation);
                            }
                        }
                    }

                    return Ok(new { status = "success", message = "import file location success" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }
        }
    }
}
