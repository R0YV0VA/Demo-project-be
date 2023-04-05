using AuthWithJWTExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AuthWithJWTExample.Controllers;

[ApiController]
[Route("")]
public class AuthorizeController : ControllerBase
{
    private readonly JwtAuthentificationManager jwtAuthentificationManager;
    private readonly UsersContext usersContext;

    public AuthorizeController(JwtAuthentificationManager jwtAuthentificationManager, UsersContext usersContext)
    {
        this.jwtAuthentificationManager = jwtAuthentificationManager;
        this.usersContext = usersContext;
    }
    
    [AllowAnonymous]
    [EnableCors]
    [HttpPost("signin")]
    public IActionResult SignIn([FromBody] SignInUser user)
    {
        var token = jwtAuthentificationManager.Authenticate(user.Login, user.Password, usersContext);
        if (token == null)
            return Unauthorized();
        return Ok(token);
    }

    [AllowAnonymous]
    [EnableCors]
    [HttpPost("signup")]
    public IActionResult SignUp([FromBody] SignUpUser signUpUser)
    {
        User user = new User();
        user.Id = new Random().Next(1000, Int32.MaxValue);
        user.Nickname = signUpUser.Nickname;
        user.Login = signUpUser.Login;
        user.Password = JwtAuthentificationManager.CreateMD5(signUpUser.Password);
        user.Status = 2;
        usersContext.Users.Add(user);
        usersContext.SaveChanges();
        return Ok();
    }
}
