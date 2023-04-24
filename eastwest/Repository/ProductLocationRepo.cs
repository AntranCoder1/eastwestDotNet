using eastwest.Data;
using eastwest.Models;
using Microsoft.EntityFrameworkCore;
using eastwest.ClassValue;

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

        public async Task<ProductLocationModel> createNewProductLocation(ProductLocationValue productLocation)
        {
            var newProductLocation = new ProductLocationModel()
            {
                productId = productLocation.productId,
                locationId = productLocation.locationId,
                quantity = productLocation.quantity,
                skuProduct = productLocation.skuProduct,
                locBarcode = productLocation.locBarcode
            };

            _context.ProductLocations.Add(newProductLocation);
            await _context.SaveChangesAsync();

            return newProductLocation;
        }

        public async Task<ProductLocationModel> findBySkuAndLoc(string sku, string loc)
        {
            return await _context.ProductLocations.Where(pL => pL.skuProduct == sku && pL.locBarcode == loc).FirstOrDefaultAsync();
        }

        public async Task<ProductLocationModel> updateQuantity(string sku, string loc, int? quantity)
        {
            var findProduct = await _context.ProductLocations.Where(pl => pl.skuProduct == sku && pl.locBarcode == loc).FirstOrDefaultAsync();

            if (findProduct != null)
            {
                findProduct.quantity = quantity;

                _context.ProductLocations.Update(findProduct);
                await _context.SaveChangesAsync();

                return findProduct;
            }
            else
            {
                return null;
            }
        }
    }
}
