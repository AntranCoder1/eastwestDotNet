using eastwest.Data;
using eastwest.Models;
using eastwest.Repository;
using eastwest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using eastwest.ClassValue;
using System.Text;
using OfficeOpenXml;

namespace eastwest.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserController(ApplicationDBContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            var userRepo = new UserRepo(_context, _configuration);

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            Login user = JsonConvert.DeserializeObject<Login>(rawContent);

            int admin = 1;

            var checkEmail = await userRepo.findUserWithEmail(user.email);

            if (checkEmail != null)
            {
                return Ok("Email already exists");
            }

            var userContact = await userRepo.createNewUser(user, admin);

            return Ok(userContact);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin()
        {
            var userRepo = new UserRepo(_context, _configuration);

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            Login user = JsonConvert.DeserializeObject<Login>(rawContent);

            if (user != null && user.email != null && user.password != null)
            {
                var findEmail = await userRepo.findUserWithEmail(user.email);

                if (findEmail == null)
                {
                    return NotFound("Email Not Found");
                }

                if (BCrypt.Net.BCrypt.Verify(user.password, findEmail.password))
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", findEmail.Id.ToString()),
                        new Claim("isAdmin", findEmail.isAdmin.ToString()),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid crendentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("createNewWorker")]
        public async Task<IActionResult> createNewWorker([FromHeader(Name = "Authorization")] string token)
        {
            var userRepo = new UserRepo(_context, _configuration);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            var isAdmin = jwtToken.Claims.First(claim => claim.Type == "isAdmin").Value;

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            Login user = JsonConvert.DeserializeObject<Login>(rawContent);

            if (int.Parse(isAdmin) != 1)
            {
                return BadRequest("You not admin");
            }

            var checkEmail = await userRepo.findUserWithEmail(user.email);

            if (checkEmail != null)
            {
                return Ok("Email already exists");
            }

            int admin = 0;

            var createWorker = await userRepo.createNewUser(user, admin);

            return Ok(createWorker);
        }

        [Authorize]
        [HttpGet("/getList")]
        public async Task<IActionResult> getUsers([FromHeader(Name = "Authorization")] string token)
        {
            var userRepo = new UserRepo(_context, _configuration);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "isAdmin").Value;

            var users = await userRepo.findAll();

            if (users != null)
            {
                return Ok(users);
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [HttpGet("getById")]
        public async Task<IActionResult> findUserId([FromHeader(Name = "Authorization")] string token)
        {
            var userRepo = new UserRepo(_context, _configuration);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            var user = await userRepo.findById(int.Parse(userId));

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("User Not Found");
            }
        }

        [Authorize]
        [HttpPost("updateUser")]
        public async Task<IActionResult> updateInfoUser([FromHeader(Name = "Authorize")] string token)
        {
            var userRepo = new UserRepo(_context, _configuration);

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            Login user = JsonConvert.DeserializeObject<Login>(rawContent);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            var findUserId = await userRepo.findById(int.Parse(userId));

            if (findUserId == null)
            {
                return NotFound("User not found");
            }

            var updatedUser = new Login
            {
                name = user.name,
                email = user.email,
                phone = user.phone,
                profile_image = user.profile_image,
            };

            var result = await userRepo.editUser(int.Parse(userId), updatedUser);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpDelete("deleteWorker/{userId}")]
        public async Task<IActionResult> deleteUser([FromHeader(Name = "Authorization")] string token)
        {
            var userRepo = new UserRepo(_context, _configuration);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Replace("Bearer ", string.Empty));

            var userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            var isAdmin = jwtToken.Claims.First(claim => claim.Type == "isAdmin").Value;

            var findUser = await userRepo.findById(int.Parse(userId));

            if (findUser == null)
            {
                return NotFound("User not found");
            }

            if (findUser.isAdmin != 1)
            {
                return BadRequest("You not admin");
            }

            var removeUser = await userRepo.deleteUser(int.Parse(userId));

            return Ok("User has been deleted successfully");
        }

        [Authorize]
        [HttpPost("uploadImage")]
        public async Task<IActionResult> uploadProfileImage(IFormFile image)
        {
            var imageExtension = new ImageExtension();

            if (!imageExtension.IsImageExtension(image.FileName))
            {
                return BadRequest("Invalid image type");
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Upload/User");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            string imageUrl = Url.Content("~/Upload/User/" + uniqueFileName);

            return Ok(imageUrl);
        }

        [HttpGet("images/{filename}")]
        public IActionResult GetImage(string filename)
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Upload/User", filename);
            var image = System.IO.File.OpenRead(path);
            return File(image, "image/jpeg");
        }

        [HttpPost("importFile")]
        public async Task<IActionResult> importFileWorker(IFormFile file)
        {
            try
            {
                var userRepo = new UserRepo(_context, _configuration);

                string host = "workerManagerment/getFile/";
                string urls = host + file.FileName;
                string url = Path.Combine(Directory.GetCurrentDirectory(), "Upload/File", file.FileName);

                using (var stream = new FileStream(url, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var parsedData = new List<Login>();

                using (var package = new ExcelPackage(new FileInfo(url)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var user = new Login();

                        for (int col = 1; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value?.ToString();

                            if (cellValue != null)
                            {
                                switch (worksheet.Cells[1, col].Value?.ToString())
                                {
                                    case "Name":
                                        user.name = cellValue;
                                        break;
                                    case "Email":
                                        user.email = cellValue;
                                        break;
                                    case "Phone":
                                        user.phone = int.Parse(cellValue);
                                        break;
                                }
                            }
                        }

                        parsedData.Add(user);
                    }
                }

                if (parsedData.Count > 0)
                {
                    foreach (var i in parsedData)
                    {
                        var getUser = await userRepo.findUserWithEmail(i.email);

                        if (getUser == null)
                        {
                            int admin = 0;

                            var dataUser = new Login
                            {
                                name = i.name,
                                email = i.email,
                                phone = i.phone
                            };

                            var createNewUser = await userRepo.createNewUser(dataUser, admin);
                        }
                    }
                }

                return Ok(new { status = "success", message = "import file worker has been success" });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> forgotPassword()
        {
            var userRepo = new UserRepo(_context, _configuration);

            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            Login user = JsonConvert.DeserializeObject<Login>(rawContent);

            var findEmail = await userRepo.findUserWithEmail(user.email);

            if (findEmail == null)
            {
                return BadRequest(new { status = "failed", message = "Email not found" });
            }

            string tokenRP = BCrypt.Net.BCrypt.HashPassword("resetPassword");

            string token = tokenRP.Substring(21).Replace("/", "");

        }
    }
}
