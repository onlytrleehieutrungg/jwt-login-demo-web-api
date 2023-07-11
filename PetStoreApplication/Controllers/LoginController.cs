using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccess.Models;
using DataAccess.Repository.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace PetStoreApplication.Controllers;

public class LoginController : ControllerBase
{
    private readonly ILoginRepository _loginRepository;

    public LoginController(ILoginRepository loginRepository)
    {
        _loginRepository = loginRepository;
    }

    [HttpPost, Route("login")]
    public IActionResult Login([FromBody] LoginModel loginModel)
    {
        try
        {
            if (string.IsNullOrEmpty(loginModel.Email) ||
                string.IsNullOrEmpty(loginModel.Password))
                return BadRequest("Email and/or Password not specified");
            PetShopMember? member = _loginRepository.Login(loginModel.Email, loginModel.Password);
            if (member != null)
            {
                var tokenString = GenerateJwtToken(member.EmailAddress, member.MemberRole.ToString() );
                return Ok(tokenString);
            }
        }
        catch (Exception e)
        {
            BadRequest(e.Message);
        }

        return Unauthorized();
    }

    string GenerateJwtToken(string? email,string role )
    {
        var securityKey =
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes("thisismysecretkeywhichyouwillneversee"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var permClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
        };

        // if (roles != null && roles.Length > 0)
        // {
        //     permClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        // }

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenDescription = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(permClaims),
            Expires = DateTime.Now.AddMinutes(Convert.ToDouble(60)),
            SigningCredentials = credentials
        };
        var token = jwtSecurityTokenHandler.CreateToken(tokenDescription);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}