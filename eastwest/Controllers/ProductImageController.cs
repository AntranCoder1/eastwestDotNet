using eastwest.Data;
using eastwest.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eastwest.Controllers
{
    [Authorize]
    [Route("api/productImage")]
    [ApiController]
    public class ProductImageController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public ProductImageController(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // [HttpPost("createImage")]
        // public async Task<IActionResult> createImage(int productId, [FromBody] string[] arrImage)
        // {
        //     Console.WriteLine(" file " + arrImage);

        //     var productImageRepo = new ProductImageRepo(_context, _configuration);

        //     if (arrImage.Length > 0)
        //     {
        //         for (var i = 0; i < arrImage.Length; i++)
        //         {
        //             var createImageProduct = await productImageRepo.createImage(i.ToString(), productId);
        //         }
        //     }

        //     return Ok(productImageRepo);
        // }

        //    [HttpPost("updateImageProduct")]
        //    public async Task<IActionResult> updateImageProduct(int productId, [FromBody] Image[] images)
        //    {
        //        var productImageRepo = new ProductImageRepo(_context, _configuration);

        //        if (images.Length > 0)
        //        {
        //            for (var i = 0; i < images.Length;i++)
        //            {
        //                if (i.status == true)
        //            }
        //        }

        //        //if (arrDelete.Length > 0)
        //        //{
        //        //    for (var i = 0; i < arrDelete.Length; i++)
        //        //    {
        //        //        var deleteImageProduct = await productImageRepo.deleteImage(i);
        //        //    }
        //        //}

        //        //if (arrAdd.Length > 0)
        //        //{
        //        //    for (var i = 0; i < arrAdd.Length; i++)
        //        //    {
        //        //        var addImageProduct = await productImageRepo.createImage(i.ToString(), productId);
        //        //    }
        //    }

        //        return Ok("update image success");
        //}
    }
}
