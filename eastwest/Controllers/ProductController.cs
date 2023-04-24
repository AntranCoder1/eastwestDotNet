using eastwest.Data;
using eastwest.Models;
using eastwest.Repository;
using eastwest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using eastwest.ClassValue;
using System.Text;
using OfficeOpenXml;

namespace eastwest.Controllers
{
    [Authorize]
    [Route("api/product")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDBContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet("getList")]
        public async Task<IActionResult> getProducts([FromHeader(Name = "Authorization")] string token)
        {
            var productRepo = new ProductRepo(_context, _configuration);
            var userRepo = new UserRepo(_context, _configuration);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            var isAdmin = jwtToken.Claims.First(claim => claim.Type == "isAdmin").Value;

            var checkAdmin = await userRepo.findById(int.Parse(userId));

            if (checkAdmin.isAdmin != 1)
            {
                return BadRequest("You not admin");
            }

            var users = await productRepo.findAll();

            if (users != null)
            {
                return Ok(new { status = "success", data = users });
            }
            else
            {
                return NotFound(new { status = "failed", message = "Not Found" });
            }
        }

        [HttpPost("createProduct")]
        public async Task<ActionResult> createNewProduct([FromHeader(Name = "Authorization")] string token)
        {
            var productRepo = new ProductRepo(_context, _configuration);

            var userRepo = new UserRepo(_context, _configuration);

            var productImageRepo = new ProductImageRepo(_context, _configuration);

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            Product product = JsonConvert.DeserializeObject<Product>(rawContent);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            var isAdmin = jwtToken.Claims.First(claim => claim.Type == "isAdmin").Value;

            var checkAdmin = await userRepo.findById(int.Parse(userId));

            if (checkAdmin.isAdmin != 1)
            {
                return BadRequest(new { status = "failed", message = "You not admin" });
            }

            var findProductSku = await productRepo.findWithSKU(product.SKU_product);

            if (findProductSku != null)
            {
                return BadRequest(new { status = "failed", message = "SKU already exists" });
            }

            var findProductUpc = await productRepo.findWithUPC(product.UPC);

            if (findProductUpc != null)
            {
                return BadRequest(new { status = "failed", message = "SUPC already exists" });
            }

            var createProduct = await productRepo.createProduct(product);

            if (product.image != null && product.image.Count > 0)
            {
                for (var i = 0; i < product.image.Count; i++)
                {
                    var createImageProduct = await productImageRepo.createImage(product.image[i].image, createProduct.Id);
                }
            }

            return Ok(new { status = "success", message = "create product has been success", });
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> uploadProfileImage(IFormFile image, [FromHeader(Name = "Authorization")] string token)
        {
            var userRepo = new UserRepo(_context, _configuration);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            var checkAdmin = await userRepo.findById(int.Parse(userId));

            if (checkAdmin.isAdmin != 1)
            {
                return BadRequest("You not admin");
            }

            var imageExtension = new ImageExtension();

            Console.WriteLine("image.FileName" + image.FileName);
            Console.WriteLine("image" + image);

            if (!imageExtension.IsImageExtension(image.FileName))
            {
                return BadRequest("Invalid image type");
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Upload/Product");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            string imageUrl = Url.Content("~/Upload/Product/" + uniqueFileName);

            return Ok(imageUrl);
        }


        [HttpGet("images/{filename}")]
        public IActionResult GetImage(string filename)
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Upload/Product", filename);

            Console.WriteLine("path" + path);
            var image = System.IO.File.OpenRead(path);
            return File(image, "image/jpeg");
        }

        [HttpPost("updateProduct")]
        public async Task<IActionResult> editProduct(int productId)
        {
            var productRepo = new ProductRepo(_context, _configuration);

            var productImageRepo = new ProductImageRepo(_context, _configuration);

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            Product product = JsonConvert.DeserializeObject<Product>(rawContent);

            var findId = await productRepo.findById(productId);

            if (findId == null)
            {
                return NotFound("Product not found");
            }

            var updateProduct = await productRepo.updateProduct(productId, product);

            if (product.arrImageAdd != null && product.arrImageAdd.Count > 0)
            {
                for (var i = 0; i < product.arrImageAdd.Count; i++)
                {
                    var addImageProduct = await productImageRepo.createImage(product.arrImageAdd[i].image, productId);
                }
            }

            if (product.arrImageDel != null && product.arrImageDel.Count > 0)
            {
                for (var i = 0; i < product.arrImageDel.Count; i++)
                {
                    var deleteImageProduct = await productImageRepo.deleteImage(product.arrImageDel[i].Id);
                }
            }

            return Ok(updateProduct);
        }

        [HttpGet("getById/{productId}")]
        public async Task<IActionResult> getProductById(int productId)
        {
            var productRepo = new ProductRepo(_context, _configuration);
            var productImageRepo = new ProductImageRepo(_context, _configuration);

            var findProduct = await productRepo.findById(productId);

            var findProductImage = await productImageRepo.findImageProduct(productId);

            for (var i = 0; i < findProductImage.Count; i++)
            {

            }

            var dataResult = new LoadPorductId
            {
                SKU_product = findProduct.SKU_product,
                Product_Name = findProduct.Product_Name,
                UPC = findProduct.UPC,
                // image = findProductImage
            };

            if (findProduct != null)
            {
                return Ok(findProduct);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("importFile")]
        public async Task<IActionResult> importFileProduct(IFormFile file)
        {
            try
            {
                var productRepo = new ProductRepo(_context, _configuration);
                var productImageRepo = new ProductImageRepo(_context, _configuration);
                var productLocationRepo = new ProductLocationRepo(_context, _configuration);
                var locationRepo = new LocationRepo(_context, _configuration);

                string host = "productManagerment/getFile/";
                string urls = host + file.FileName;
                string url = Path.Combine(Directory.GetCurrentDirectory(), "Upload/File", file.FileName);

                using (var stream = new FileStream(url, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var parsedData = new List<ProductImportDto>();

                using (var package = new ExcelPackage(new FileInfo(url)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var product = new ProductImportDto();

                        for (int col = 1; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value?.ToString();

                            if (cellValue != null)
                            {
                                switch (worksheet.Cells[1, col].Value?.ToString())
                                {
                                    case "SKU":
                                        product.SKU = cellValue;
                                        break;
                                    case "Product Name":
                                        product.ProductName = cellValue;
                                        break;
                                    case "UPC":
                                        product.UPC = cellValue;
                                        break;
                                    case "Locations":
                                        product.Locations = cellValue.Split(',').ToList(); ;
                                        break;
                                    case "Quantity":
                                        product.Quantity = int.Parse(cellValue);
                                        break;
                                    case "Images":
                                        product.Images = cellValue;
                                        break;
                                }
                            }
                        }

                        parsedData.Add(product);
                    }
                }

                if (parsedData.Count > 0)
                {
                    foreach (var i in parsedData)
                    {
                        ProductModel addProduct;
                        // check product exists
                        var getProduct = await productRepo.findWithSKU(i.SKU);

                        if (getProduct != null)
                        {
                            addProduct = getProduct;
                        }
                        else
                        {
                            addProduct = await productRepo.createProductImport(i);

                            // create image product
                            var createImage = await productImageRepo.createImage(i.Images, addProduct.Id);
                        }

                        foreach (var j in i.Locations)
                        {
                            // check product location exists
                            var getProductLocation = await productLocationRepo.findBySkuAndLoc(i.SKU, j);

                            LocationModel location;

                            var getLocation = await locationRepo.findLocbarcode(j);

                            if (getLocation == null)
                            {
                                var dataLoc = new LocationValue
                                {
                                    Loc_Barcodes = j
                                };

                                location = await locationRepo.createLocation(dataLoc);
                            }
                            else
                            {
                                location = getLocation;
                            }

                            if (getProductLocation == null)
                            {
                                var newProductLocation = new ProductLocationValue
                                {
                                    productId = addProduct.Id,
                                    locationId = location.Id,
                                    quantity = i.Quantity,
                                    skuProduct = addProduct.SKU_product,
                                    locBarcode = location.Loc_Barcodes
                                };

                                var createNewProductLocation = await productLocationRepo.createNewProductLocation(newProductLocation);
                            }
                            else
                            {
                                // update quantity of product in location
                                var lastQuantity = getProductLocation.quantity + i.Quantity;

                                var updateQuantity = await productLocationRepo.updateQuantity(i.SKU, j, lastQuantity);
                            }
                        }

                    }
                }
                return Ok(new { status = "success", message = "Data imported successfully.", data = parsedData });
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
