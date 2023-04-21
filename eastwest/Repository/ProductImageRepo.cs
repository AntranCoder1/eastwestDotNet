using eastwest.Data;
using eastwest.Models;
using Microsoft.EntityFrameworkCore;

namespace eastwest.Repository
{
    public class ProductImageRepo
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public ProductImageRepo(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ProductImageModel> createImage(string image, int productId)
        {
            var newImageProduct = new ProductImageModel
            {
                productId = productId,
                image = image
            };

            _context.ProductImages.Add(newImageProduct);
            await _context.SaveChangesAsync();

            return newImageProduct;
        }

        public async Task<ProductImageRepo> deleteImage(int productImageId)
        {
            var findImage = await _context.ProductImages.Where(p => p.Id == productImageId).FirstOrDefaultAsync();

            if (findImage != null)
            {
                _context.ProductImages.Remove(findImage);

                await _context.SaveChangesAsync();
            }

            return null;
        }

        public async Task<List<ProductImageModel>> findImageProduct(int productId)
        {
            return await _context.ProductImages.Where(image => image.productId == productId).ToListAsync();
        }
    }
}
