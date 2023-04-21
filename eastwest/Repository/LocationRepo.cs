using eastwest.Data;
using eastwest.Models;
using Microsoft.EntityFrameworkCore;

namespace eastwest.Repository
{
    public class LocationRepo
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public LocationRepo(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //     public async Task<LocationModel> createLocation(LocationModel location)
        //     {
        //         var newLocation = new LocationModel()
        //         {
        //             Loc_Barcodes = location.Loc_Barcodes
        //         };

        //         _context.Locations.Add(newLocation);
        //         await _context.SaveChangesAsync();

        //         return newLocation;
        //     }

        //     public async Task<LocationModel> updateLocation(int locationId, LocationModel updateLocation)
        //     {
        //         var findLocation = await _context.Locations.FindAsync(locationId);

        //         if (findLocation != null)
        //         {
        //             findLocation.Loc_Barcodes = updateLocation.Loc_Barcodes;

        //             _context.Locations.Update(findLocation);
        //             await _context.SaveChangesAsync();
        //         }

        //         return findLocation;
        //     }

        //     public async Task<LocationModel> deleteLocation(int locationId)
        //     {
        //         var findLocation = await _context.Locations.FindAsync(locationId);

        //         if (findLocation != null)
        //         {
        //             _context.Locations.Remove(findLocation);
        //             await _context.SaveChangesAsync();
        //         }

        //         return findLocation;
        //     }
        //     public async Task<LocationModel> findAll()
        //     {
        //         return await _context.Locations.FindAsync();
        //     }

        //     public async Task<LocationModel> findById(int locationId)
        //     {
        //         return await _context.Locations.Where(l => l.Id == locationId).FirstOrDefaultAsync();
        //     }
    }
}
