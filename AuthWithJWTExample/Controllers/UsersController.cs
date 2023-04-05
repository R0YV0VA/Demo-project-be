using AuthWithJWTExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthWithJWTExample.Controllers;
[ApiController]
[Route("")]

public class UsersController : ControllerBase
{
    private readonly JwtAuthentificationManager jwtAuthentificationManager;
    private readonly UsersContext usersContext;

    public UsersController(JwtAuthentificationManager jwtAuthentificationManager, UsersContext usersContext)
    {
        this.jwtAuthentificationManager = jwtAuthentificationManager;
        this.usersContext = usersContext;
    }

    [EnableCors]
    [Authorize]
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var myUser = usersContext.Users.Find(userId);
        List<User> users = usersContext.Users.ToList();
        if (myUser.Status == 2)
        {
            foreach (var u in users)
            {
                u.Password = "No premissions!";
                u.Login = "No premissions!";
            }
            return Ok(users);
        }
        if (myUser.Status == 1)
        {
            foreach (var u in users)
                u.Password = "No premissions!";
            return Ok(users);
        }
        if (myUser.Status == 0)
            return Ok(users);
        return BadRequest("You don`t have a premission!");
    }

    [EnableCors]
    [Authorize]
    [HttpGet("user/{id}")]
    public IActionResult GetUser(int id)
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var myUser = usersContext.Users.Find(userId);
        User user = usersContext.Users.Find(id);
        if (myUser.Status == 2)
        {
            user.Password = "No premissions!";
            user.Login = "No premissions!";
            return Ok(user);
        }
        if (myUser.Status == 1)
        {
            user.Password = "No premissions!";
            return Ok(user);
        }
        if (myUser.Status == 0)
            return Ok(user);
        return BadRequest("You don`t have a premission!");
    }

    [EnableCors]
    [Authorize]
    [HttpGet("users/myaccount")]
    public IActionResult GetMyUser()
    {
        int? userId = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name));
        var myUser = usersContext.Users.Find(userId);
        return Ok(myUser);
    }

    [EnableCors]
    [Authorize]
    [HttpPost("users/filter")]
    public IActionResult GetUserByParam([FromBody] User user)
    {
        var myUser = usersContext.Users.Find(Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name)));
        if (myUser.Status == 0)
        {
            var filteredUsers = usersContext.Users
            .Where(u =>
                (user.Id == 0 || u.Id.ToString().Contains(user.Id.ToString())) &&
                (user.Nickname == null || u.Nickname.Contains(user.Nickname)) &&
                (user.Login == null || u.Login.Contains(user.Login)) &&
                (user.Password == null || u.Password.Contains(user.Password)))
            .ToList();
            return Ok(filteredUsers);
        }
        return BadRequest("You don`t have a premission!");
    }

    [EnableCors]
    [Authorize]
    [HttpPost("users")]
    public IActionResult CreateUser([FromBody] User user)
    {
        var myUser = usersContext.Users.Find(Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name)));
        if (myUser.Status == 0)
        {
            user.Id = new Random().Next(1000, Int32.MaxValue);
            user.Password = JwtAuthentificationManager.CreateMD5(user.Password);
            usersContext.Users.Add(user);
            usersContext.SaveChanges();
            return Ok();
        }
        return BadRequest("You don`t have a premission!");
    }

    [EnableCors]
    [Authorize]
    [HttpPut("users")]
    public IActionResult UpdateUser([FromBody] User user)
    {
        var myUser = usersContext.Users.Find(Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name)));
        if (myUser.Status == 0)
        {
            usersContext.Users.Update(user);
            usersContext.SaveChanges();
            return Ok();
        }
        return BadRequest("You don`t have a premission!");
    }

    [EnableCors]
    [Authorize]
    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var myUser = usersContext.Users.Find(Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.Name)));
        if (myUser.Status == 0)
        {
            usersContext.Users.Remove(usersContext.Users.Find(id));
            usersContext.SaveChanges();
            return Ok();
        }
        return BadRequest("You don`t have a premission!");
    }
}
