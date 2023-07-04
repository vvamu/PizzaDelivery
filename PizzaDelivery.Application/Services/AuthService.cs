using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Domain.Models.User;
using System.Globalization;
using System.Resources;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PizzaDelivery.Application.Helpers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;


    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context, IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _config = config;
    }

    public async Task<ICollection<ApplicationUser>> GetAllAsync()
    {
        var user = _signInManager.Context;
        return await _userManager.Users.ToListAsync();
    }

    public async Task<ApplicationUser> LoginAsync(LoginUser user)
    {
        await CreateAdmin();
        var users = await _userManager.Users.ToListAsync();
        var db_user = await _userManager.FindByEmailAsync(user.Email);
        if (db_user == null) throw new Exception("Not user with such email");


        //var checkpassword = await _signInManager.PasswordSignInAsync(db_user.Id, user.Password.Trim(), false, false);
        //if (!checkpassword.Succeeded) throw new Exception(checkpassword.IsNotAllowed.ToString());

        var has1 = db_user.OwnHashedPassword;
        if(!HashProvider.VerifyHash(user.Password, has1)) throw new Exception("Incorrect password");
        await _signInManager.SignInAsync(db_user,false);

        return db_user;
    }
    public async Task<ApplicationUser> LogoutAsync()
    {
        var username = _signInManager.Context.User.Identity.Name;
        var current_user = await  _userManager.FindByNameAsync(username);

        await _signInManager.SignOutAsync();
       
        return current_user;
    }

    public async Task<ApplicationUser> GetCurrentUserInfo()
    {
        var username = _signInManager.Context.User.Identity.Name;
        var current_user = await _userManager.FindByNameAsync(username);

        return current_user;
    }

    public async Task<ApplicationUser> RegisterAsync(RegisterUser user)
    {
        var db_user = await _userManager.FindByEmailAsync(user.Email);
        if (db_user != null) throw new Exception("This email already in use");

        string hashedPassword = HashProvider.ComputeHash(user.Password.Trim());

        var applicationUser = new ApplicationUser()
        {
            Email= user.Email,
            UserName = user.Username,
            OwnHashedPassword = hashedPassword
        };

        var result = await _userManager.CreateAsync(applicationUser,user.Password.Trim());
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault().Description);
        await _context.SaveChangesAsync();

        var shoppingCart = new ShoppingCart() { User = applicationUser };
        await _context.ShoppingCart.AddAsync(shoppingCart);
        applicationUser.ShoppingCart= shoppingCart;
        _context.Update(applicationUser);
        await _context.SaveChangesAsync();


        var adminEmail = _config["Admin:Email"];
        if (user.Email != adminEmail) await _userManager.AddToRoleAsync(applicationUser, "User"); 

        await _context.SaveChangesAsync();
        
        return applicationUser;

    }


    public async Task<string> GetRole(ApplicationUser user)
    {
        var db_user = _userManager.FindByEmailAsync(user.Email).Result;
        var roles = await _userManager.GetRolesAsync(db_user);
        var rolesString = string.Join(",", roles);

        return rolesString;
    }


    public string GenerateTokenString(LoginUser user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,"Admin"),
            };

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));
        var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            issuer: _config.GetSection("Jwt:Issuer").Value,
            audience: _config.GetSection("Jwt:Audience").Value,
            signingCredentials: signingCred);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return tokenString;
    }

    public async Task CreateAdmin()
    {
        var adminEmail = _config.GetSection("Admin:Email").Value;
        var adminUserName = _config["Admin:UserName"];
        var adminPassword = _config.GetValue<string>("Admin:Password");

        var result = await _userManager.FindByEmailAsync(adminEmail);
        if (result != null) return;

        var adminNew = new RegisterUser()
        {
            Email = adminEmail,
            Username = adminUserName,
            Password = adminPassword
        };
       var db_user = await RegisterAsync(adminNew);
        var resRole = await _userManager.AddToRoleAsync(db_user, "Admin");
        //var resRole = await _userManager.AddClaimAsync(result, new Claim(ClaimTypes.Role,"Admin"));
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAll()
    {
        foreach (var user in _userManager.Users)
            await _userManager.DeleteAsync(user);

        await _context.SaveChangesAsync();
        if (_userManager.Users.Count() > 0) return false;


        return true;
    }

    public async Task<ApplicationUser> DeleteAccount(string userId = null)
    {
        ApplicationUser db_user = null;
        if(string.IsNullOrEmpty(userId))
        {
            var username = _signInManager.Context.User.Identity.Name;
            db_user = await _userManager.FindByNameAsync(username);
            userId = db_user.Id;
            await LogoutAsync();
        }
        db_user = await _context.Users.FindAsync(userId);
        await _userManager.DeleteAsync(db_user);
        await _context.SaveChangesAsync();
        
        return db_user;
    }

}
