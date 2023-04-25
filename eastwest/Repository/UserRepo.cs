using eastwest.Data;
using eastwest.Models;
using Microsoft.EntityFrameworkCore;
using eastwest.ClassValue;

namespace eastwest.Repository
{
    public class UserRepo
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _configuration;
        public UserRepo(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserModel> findUserWithEmail(string email)
        {
            var user = await _context.Users
                .Where(u => u.email == email)
                .Select(u => new UserModel { Id = u.Id, name = u.name, email = u.email, phone = u.phone, password = u.password, profile_image = u.profile_image, isAdmin = u.isAdmin })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserModel> createNewUser(Login user, int admin)
        {
            string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);

            if (admin == 1)
            {
                var users = new UserModel
                {
                    name = user.name,
                    email = user.email,
                    phone = user.phone,
                    password = encryptedPassword,
                    isAdmin = 1
                };

                await _context.Users.AddAsync(users);
                await _context.SaveChangesAsync();

                return users;
            }
            else
            {
                var users = new UserModel
                {
                    name = user.name,
                    email = user.email,
                    phone = user.phone,
                    password = encryptedPassword,
                    isAdmin = 0
                };

                await _context.Users.AddAsync(users);
                await _context.SaveChangesAsync();

                return users;
            }
        }

        public async Task<UserModel> findById(int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserModel { Id = u.Id, name = u.name, email = u.email, phone = u.phone, profile_image = u.profile_image, created_at = u.created_at, updated_at = u.updated_at, isAdmin = u.isAdmin })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<UserModel> editUser(int userId, Login user)
        {
            var findUser = await _context.Users.FindAsync(userId);

            if (findUser != null)
            {
                findUser.name = user.name;
                findUser.email = user.email;
                findUser.phone = user.phone;
                findUser.profile_image = user.profile_image;

                _context.Users.Update(findUser);
                await _context.SaveChangesAsync();

                return findUser;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserModel> deleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<UserModel> updateResetToken(string email, string token)
        {
            var findUser = await _context.Users.Where(u => u.email == email).FirstOrDefaultAsync();

            if (findUser != null)
            {
                findUser.reset_password_token = token;

                _context.Users.Update(findUser);
                await _context.SaveChangesAsync();

                return findUser;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserModel> findAll()
        {
            return await _context.Users.FindAsync();
        }

        public async Task<UserModel> findUserWithToken(string token)
        {
            return await _context.Users.Where(u => u.reset_password_token == token).FirstOrDefaultAsync();
        }

        public async Task<UserModel> updatePassword(string token, string password)
        {
            var finduser = await _context.Users.Where(u => u.reset_password_token == token).FirstOrDefaultAsync();

            if (finduser != null)
            {
                string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                finduser.password = password;

                _context.Users.Update(finduser);
                await _context.SaveChangesAsync();

                return finduser;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserModel> updateTokenById(int userId)
        {
            var findUser = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (findUser != null)
            {
                findUser.reset_password_token = "";

                _context.Users.Update(findUser);
                await _context.SaveChangesAsync();

                return findUser;
            }
            else
            {
                return null;
            }
        }
    }
}
