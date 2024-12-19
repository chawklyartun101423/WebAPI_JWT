using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_JWT.Models;

namespace WebAPI_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public static User user = new User();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            string passwordHash
                = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            return Ok(user);

        }
        [HttpPost("login")]
        public ActionResult<User> Login(UserDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User Not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("wrong password");
            }
            string token = CreateToken(user);
            return Ok(token);

        }
        //creating one list of claims for this token and then we will see if we can actually find the name then  when we get this token and 
        //we can actually find the name then when we get this token a list of claims and claims is in the namespace use System security claims

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
          {     new Claim(ClaimTypes.Name, user.Username)
             // new Claim(ClaimTypes.Role,user.Role)
           };  //Object Initializer to store again  the username 

            //first needed symmetric security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Authentication:Schemes:Bearer:SigningKeys:0:Value").Value!));  //AppSettings:Token
            // need signing credentials 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
