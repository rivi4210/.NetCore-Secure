using Entities;
using Repositories;
using DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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
            u.Token = generateJwtToken(u);
            return u;
        }
        async Task<User> IUserService.Register(User user)
        {
            var checkStrength = Check(user.Password);
            if (checkStrength < 2)
                return null;
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
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("key").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
             new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    // new Claim("roleId", 7.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
