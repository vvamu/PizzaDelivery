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
using PizzaDelivery.Models.Interfaces;
using PizzaDelivery.Application.DTO;

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
    public ApplicationUser GetCurrentUser()
    {
        var username = _signInManager.Context.User.Identity.Name;
        var user = _signInManager.Context;
        var users = _userManager.Users;
        var userDTO = GetCurrentUserInfo();
        var countUsers = users.Count();
        var currentUser = _context.Users.Include(x => x.Orders).Include(x => x.ExternalConnections).FirstOrDefault(x => x.UserName == username);
        if (currentUser == null)
            currentUser = _userManager.Users.FirstOrDefault(x => x.UserName == username);
        return currentUser;
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

    public async Task<UserDTO> LoginByEmailAsync(UserLogin user)
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

    #region External
    public async Task<ApplicationUser> ExternalLogin(string provider, ExternalUser extUser)
    {
        var externalCollections = await _context.Users.SelectMany(x => x.ExternalConnections).ToListAsync();
        var externalUser = externalCollections.FirstOrDefault(x => x.ProviderUserId == extUser.Id);
        ApplicationUser? user = null;
        if (externalUser != null)
        {
            user = await _userManager.FindByIdAsync(externalUser.UserId) ?? throw new Exception("User not fount");
        }
        else
        {
            user = new ApplicationUser
            {
                UserName = user.UserName ?? user.Id,
                Email = user.Email ?? user.Id + "@example.com",
                OwnHashedPassword = PizzaDelivery.Application.Helpers.HashProvider.ComputeHash("default"),
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            externalUser = await AddToUserExternalConnection(user.Id, provider, user.Id);
            user.ExternalConnections.Add(externalUser);

            await _userManager.AddToRoleAsync(user, "User");

        }
        await _context.SaveChangesAsync();
        user = await _context.Users.Include(x => x.ExternalConnections).
            FirstOrDefaultAsync(x => x.Id == user.Id);


        await _signInManager.SignInAsync(user, false);
        await _context.SaveChangesAsync();


        return user;
    }


    public async Task<ExternalConnection> AddToUserExternalConnection(string userId, string provider, string externalUserId)
    {

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId) ?? throw new Exception("User not found");

        var externalCollections = await _context.Users.SelectMany(x => x.ExternalConnections).ToListAsync();
        var externalUser = externalCollections.FirstOrDefault(x => x.ProviderUserId == externalUserId) ??
            throw new Exception("Account was connected to other user");

        var externalUserConnection = user.ExternalConnections.FirstOrDefault(x => x.Provider == provider);
        if (externalUserConnection != null)
        {
            externalUserConnection.ProviderUserId = externalUserId;
        }
        else
        {
            externalUserConnection = new ExternalConnection()
            {
                Provider = provider,
                ProviderUserId = externalUserId,
                UserId = userId,
            };
            _context.ExternalConnections.Add(externalUserConnection);
        }
        return externalUserConnection;

    }
    #endregion


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
        

        try
        {

            var result = await _userManager.CreateAsync(applicationUser, user.Password.Trim());
            if (!result.Succeeded) throw new Exception(result.Errors.FirstOrDefault().Description);
        }
        catch
        {
            await _context.Users.AddAsync(applicationUser);
        }

       
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
        try
        {
            var resRole = await _userManager.AddToRoleAsync(result, "Admin");

        }
        catch { }
            if (result != null) return;

        var adminNew = new UserRegister()
        {
            Email = adminEmail,
            Username = adminUserName,
            Password = adminPassword
        };
        var dbUser = _mapper.Map<ApplicationUser>(await RegisterAsync(adminNew));
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
