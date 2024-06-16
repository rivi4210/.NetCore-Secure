using Entities;
using Repositories;
using DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IConfiguration _configuration;
        private IPasswordHashHelper _passwordHashHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserService(IUserRepository userRepository, IConfiguration configuration, IPasswordHashHelper passwordHashHelper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHashHelper = passwordHashHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        async Task<User> IUserService.Update(int id, User user)
        {
            var checkStrength = Check(user.Password);
            if (checkStrength < 2)
                return null;
            User u = await _userRepository.Update(id, user);
            return u;
        }
        async Task<User> IUserService.Login(LoginDTO userLogin)
        {
            User u = await _userRepository.Login(userLogin);
            //byte[] existingHash = PasswordHashHelper.GetHash(u.Password.Trim(), u.Salt.Trim());
            //byte[] existingHash= Encoding.Unicode.GetBytes(String.Concat(u.Salt.Trim(), u.Password.Trim()));
            //bool goodPassword = PasswordHashHelper.CompareHash(userLogin.Password, existingHash, u.Salt.Trim());
            //if (goodPassword)
            //{
            u.Token = generateJwtToken(u);
            return u;
            //}
            //else
            // {
            //   return null;
            //}

        }
        async Task<User> IUserService.Register(User user)
        {
            var checkStrength = Check(user.Password);
            if (checkStrength < 2)
                return null;
            user.Salt = _passwordHashHelper.GenerateSalt(8);
            user.Password = _passwordHashHelper.HashPassword(user.Password, user.Salt, 1000, 8);
            User u = await _userRepository.Register(user);
            return u;
        }

        public async Task<User> Get(int id)
        {
            return await _userRepository.Get(id);
        }
        public int Check(object password)
        {
            var result = Zxcvbn.Core.EvaluatePassword(password.ToString());
            return result.Score;
        }

        public async Task<UserDTO> ReturnPrev(int id, UserDTO user)
        {
            User prevUser = await Get(id);

            user.UserName = user.UserName == null ? prevUser.UserName : user.UserName;
            user.Password = user.Password == null ? prevUser.Password : user.Password;
            user.FirstName = user.FirstName == null ? prevUser.FirstName : user.FirstName;
            user.LastName = user.LastName == null ? prevUser.LastName : user.LastName;
            user.Email = user.Email == null ? prevUser.Email : user.Email;

            return user;
        }

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("key").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
             new Claim(ClaimTypes.Name, user.UserId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int GetId()
        {
            HttpContext context = _httpContextAccessor.HttpContext;
            int userId = context.User.FindFirst(ClaimTypes.Name)?.Value != null ? int.Parse(context.User.FindFirst(ClaimTypes.Name)?.Value) : -1;
            if (userId == -1)
                return -1;
            return userId;
        }
    }
}
