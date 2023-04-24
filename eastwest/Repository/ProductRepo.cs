using eastwest.Data;
using eastwest.Models;
using Microsoft.EntityFrameworkCore;
using eastwest.ClassValue;

namespace eastwest.Repository
{
    public class ProductRepo
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public ProductRepo(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ProductModel> findAll()
        {
            return await _context.Products.FindAsync();
        }

        public async Task<ProductModel> findWithSKU(string sku)
        {
            return await _context.Products.Where(p => p.SKU_product == sku).FirstOrDefaultAsync();
        }

        public async Task<ProductModel> findWithUPC(string upc)
        {
            return await _context.Products.Where(p => p.UPC == upc).FirstOrDefaultAsync();
        }

        public async Task<ProductModel> createProduct(Product product)
        {
            var newProduct = new ProductModel
            {
                SKU_product = product.SKU_product,
                Product_Name = product.Product_Name,
                UPC = product.UPC,
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return newProduct;
        }

        public async Task<ProductModel> findById(int productId)
        {
            return await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<ProductModel> updateProduct(int productId, Product product)
        {
            var findProduct = await _context.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();

            if (findProduct != null)
            {
                findProduct.SKU_product = product.SKU_product;
                findProduct.Product_Name = product.Product_Name;
                findProduct.UPC = product.UPC;

                _context.Products.Update(findProduct);
                await _context.SaveChangesAsync();

                return findProduct;
            }
            else
            {
                return null;
            }
        }

        public async Task<ProductModel> createProductImport(ProductImportDto product)
        {
            var newProduct = new ProductModel
            {
                SKU_product = product.SKU,
                Product_Name = product.ProductName,
                UPC = product.UPC
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return newProduct;
        }
    }
}
