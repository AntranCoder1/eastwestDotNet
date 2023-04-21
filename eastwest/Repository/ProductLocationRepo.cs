using eastwest.Data;
using eastwest.Models;
using Microsoft.EntityFrameworkCore;

namespace eastwest.Repository
{
    public class ProductLocationRepo
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public ProductLocationRepo(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // public async Task<ProductLocationModel> createNewProductLocation(ProductLocationModel productLocation)
        // {
        //     var newProductLocation = new ProductLocationModel()
        //     {
        //         productId = productLocation.productId,
        //         locationId = productLocation.locationId,
        //         quantity = productLocation.quantity
        //     };

        //     _context.ProductLocations.Add(newProductLocation);
        //     await _context.SaveChangesAsync();

        //     return newProductLocation;
        // }

        // public async Task<ProductLocationModel> findBySkuAndLoc(string sku, string loc)
        // {
        //     return await _context.ProductLocations.Where(pL => pL.skuProduct == sku && pL.locBarcode == loc).FirstOrDefaultAsync();
        // }
    }
}
