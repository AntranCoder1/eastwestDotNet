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
using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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
                return Ok(users);
            }
            else
            {
                return null;
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
                return BadRequest("You not admin");
            }

            var findProductSku = await productRepo.findWithSKU(product.SKU_product);

            if (findProductSku != null)
            {
                return BadRequest("SKU already exists");
            }

            var findProductUpc = await productRepo.findWithUPC(product.UPC);

            if (findProductUpc != null)
            {
                return BadRequest("UPC already exists");
            }

            var createProduct = await productRepo.createProduct(product);

            if (product.image != null && product.image.Count > 0)
            {
                for (var i = 0; i < product.image.Count; i++)
                {
                    var createImageProduct = await productImageRepo.createImage(product.image[i].image, createProduct.Id);
                }
            }

            return Ok(product);
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
                var fileExtension = new FileExtension();

                if (!fileExtension.IsFileExtension(file.FileName))
                {
                    return BadRequest("Invalid image type");
                }

                string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Upload/File");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                string imageUrl = Url.Content("~/Upload/File/" + uniqueFileName);

                var path = Path.Combine(_webHostEnvironment.ContentRootPath, imageUrl);


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    // you should use Write(stream) instead, or reset the stream position.
                    Request.Form.Files[0].CopyTo(stream);

                }

                {
                    //create the object for workbook part  
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                    StringBuilder excelResult = new StringBuilder();


                    //using for each loop to get the sheet from the sheetcollection  
                    foreach (Sheet thesheet in thesheetcollection)
                    {
                        excelResult.AppendLine("Excel Sheet Name : " + thesheet.Name);
                        excelResult.AppendLine("----------------------------------------------- ");
                        //statement to get the worksheet object by using the sheet id  
                        Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;

                        SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();
                        foreach (Row thecurrentrow in thesheetdata)
                        {
                            foreach (Cell thecurrentcell in thecurrentrow)
                            {
                                //statement to take the integer value  
                                string currentcellvalue = string.Empty;
                                if (thecurrentcell.DataType != null)
                                {
                                    if (thecurrentcell.DataType == CellValues.SharedString)
                                    {
                                        int id;
                                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                                        {
                                            SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                                            if (item.Text != null)
                                            {
                                                //code to take the string value  
                                                excelResult.Append(item.Text.Text + " ");
                                            }
                                            else if (item.InnerText != null)
                                            {
                                                currentcellvalue = item.InnerText;
                                            }
                                            else if (item.InnerXml != null)
                                            {
                                                currentcellvalue = item.InnerXml;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    excelResult.Append(Convert.ToInt16(thecurrentcell.InnerText) + " ");
                                }
                            }
                            excelResult.AppendLine();
                        }
                        excelResult.Append("");
                        Console.WriteLine(excelResult.ToString());
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
