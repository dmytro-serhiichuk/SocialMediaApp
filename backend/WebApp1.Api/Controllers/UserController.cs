using WebApp1.Api.Data;
using WebApp1.Api.Models;
using WebApp1.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(AppDbContext dbContext, UserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await dbContext.Users.ToListAsync();
        return Ok(users);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
    {
        var user = await userService.Create(createUserRequest);

        if (user == null) return Conflict();

        return Ok();
    }
}