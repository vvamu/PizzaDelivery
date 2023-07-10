using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PizzaDelivery.Domain.Models.User;
using System.Globalization;
using System.Resources;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using PizzaDelivery.Domain.Models;
using Microsoft.Extensions.Options;
using PizzaDelivery.Application.Options;
using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Application.Services.Interfaces;
using AutoMapper;
using PizzaDelivery.Domain.Validators;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PizzaDelivery.Application.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly JwtOptions _jwtOptions;
    private readonly AdminOptions _adminOptions;
    private readonly IMapper _mapper;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context, IOptions<JwtOptions> jwtOptions, IOptions<AdminOptions> adminOptions,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _adminOptions = adminOptions.Value;
        _mapper = mapper;
    }

    public PagedList<UserDTO> GetAllAsync(QueryStringParameters queryStringParameters)
    {
        var items = _userManager.Users;
        var usersDTO = items.Select(x => _mapper.Map<UserDTO>(x));
        return PagedList<UserDTO>.ToPagedList(usersDTO, queryStringParameters.PageNumber, queryStringParameters.PageSize);
    }
    public async Task<ICollection<ApplicationUser>> GetAllAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<UserDTO> LoginAsync(UserLogin user)
    {
        await CreateAdmin();
        var validator = new UserLoginValidator();
        var validationResult = await validator.ValidateAsync(user);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = System.String.Concat(errors);
            throw new Exception(errorsString);
        }

        var users = await _userManager.Users.ToListAsync();
        var dbUser = await _userManager.FindByEmailAsync(user.Email);
        if (dbUser == null) throw new Exception("Not user with such email");


        //var checkpassword = await _signInManager.PasswordSignInAsync(dbUser.Id, user.Password.Trim(), false, false);
        //if (!checkpassword.Succeeded) throw new Exception(checkpassword.IsNotAllowed.ToString());

        var has1 = dbUser.OwnHashedPassword;
        if (!HashProvider.VerifyHash(user.Password, has1)) throw new Exception("Incorrect password");
        await _signInManager.SignInAsync(dbUser, false);
        var dbUserDTO = _mapper.Map<UserDTO>(dbUser);

        return dbUserDTO;
    }
    public async Task<UserDTO> LogoutAsync()
    {
        var username = _signInManager.Context.User.Identity.Name;
        var currentUser = await _userManager.FindByNameAsync(username);
        var currentUserDTO = _mapper.Map<UserDTO>(currentUser);
        await _signInManager.SignOutAsync();

        return currentUserDTO;
    }

    public async Task<UserDTO> GetCurrentUserInfo()
    {
        var username = _signInManager.Context.User.Identity.Name;
        var currentUser = await _userManager.FindByNameAsync(username);
        var currentUserDTO = _mapper.Map<UserDTO>(currentUser);

        return currentUserDTO;
    }

    public async Task<UserDTO> RegisterAsync(UserRegister user)
    {
        var userRegisterValidator = new UserRegisterValidator();
        var validationResult = await userRegisterValidator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = System.String.Concat(errors);
            throw new Exception(errorsString);
        }

        var dbUser = await _userManager.FindByEmailAsync(user.Email);
        if (dbUser != null) throw new Exception("This email already in use");

        string hashedPassword = HashProvider.ComputeHash(user.Password.Trim());

        var applicationUser = _mapper.Map<ApplicationUser>(user);
        applicationUser.OwnHashedPassword = hashedPassword;

        var result = await _userManager.CreateAsync(applicationUser, user.Password.Trim());
        if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault().Description);
        await _context.SaveChangesAsync();

        var shoppingCart = new ShoppingCart() { User = applicationUser };
        await _context.ShoppingCart.AddAsync(shoppingCart);
        applicationUser.ShoppingCart = shoppingCart;
        _context.Update(applicationUser);
        await _context.SaveChangesAsync();


        var adminEmail = _adminOptions.Email;
        if (user.Email != adminEmail) await _userManager.AddToRoleAsync(applicationUser, "User");

        await _context.SaveChangesAsync();
        var userDTO = _mapper.Map<UserDTO>(applicationUser);
        return userDTO;

    }
    public async Task<string> GetRole(string userEmail)
    {
        var dbUser = _userManager.FindByEmailAsync(userEmail).Result;
        var roles = await _userManager.GetRolesAsync(dbUser);
        var rolesString = string.Join(",", roles);

        return rolesString;
    }
    public string GenerateTokenString(UserLogin user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,"Admin"),
            };

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            signingCredentials: signingCred);

        string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return tokenString;
    }

    public async Task CreateAdmin()
    {
        var adminEmail = _adminOptions.Email;
        var adminUserName = _adminOptions.UserName;
        var adminPassword = _adminOptions.Password;

        var result = await _userManager.FindByEmailAsync(adminEmail);
        if (result != null) return;

        var adminNew = new UserRegister()
        {
            Email = adminEmail,
            Username = adminUserName,
            Password = adminPassword
        };
        var dbUser = _mapper.Map<ApplicationUser>(await RegisterAsync(adminNew));
        var resRole = await _userManager.AddToRoleAsync(dbUser, "Admin");
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

    public async Task<UserDTO> DeleteAccount(string userId = null)
    {
        ApplicationUser dbUser = null;
        if (string.IsNullOrEmpty(userId))
        {
            var username = _signInManager.Context.User.Identity.Name;
            dbUser = await _userManager.FindByNameAsync(username);
            userId = dbUser.Id;
            await LogoutAsync();
        }
        dbUser = await _context.Users.FindAsync(userId);
        await _userManager.DeleteAsync(dbUser);
        await _context.SaveChangesAsync();
        var userDTO = _mapper.Map<UserDTO>(dbUser);

        return userDTO;
    }

}
