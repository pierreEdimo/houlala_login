using Microsoft.AspNetCore.Mvc;
using user_service.Model;
using user_service.Dto;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using user_service.DbContext;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace user_service.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User>? _userManager;
        private readonly SignInManager<User>? _signInManager;
        private IConfiguration? _configuration;
        private readonly IMapper? _mapper;
        private readonly UserDbContext? _context;

        private readonly ILogger? _logger;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            UserDbContext context,
            ILogger logger
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = nameof(GetAllUsers))]
        public async Task<List<UserDto>> GetAllUsers()
        {
            using (var Context = new UserDbContext())
            {
                var Users = await _userManager!.Users.ToListAsync();

                return _mapper!.Map<List<UserDto>>(Users);
            }
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var user = await _userManager!.FindByEmailAsync(email);

            if (user == null) return NotFound();

            return _mapper!.Map<UserDto>(user);
        }

        [HttpGet]
        public async Task<ActionResult<string>> CheckAuthenticated()
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var user = await _userManager!.FindByEmailAsync(email);

            if (user == null)
                return "UNAUTHENTICATED";
            else
                return "AUTHENTICATED";
        }



        [HttpGet("{Email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(String Email)
        {

            var user = await _userManager!.FindByEmailAsync(Email);

            if (user == null) return NotFound();

            return _mapper!.Map<UserDto>(user);
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> GetUserById(String userId)
        {

            var user = await _userManager!.FindByIdAsync(userId);

            if (user == null) return NotFound();

            return _mapper!.Map<UserDto>(user);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Register([FromBody] RegisterDto model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                Country = model.Country,
                HouseNumber = model.HouseNumber,
                LastName = model.LastName,
                FirstName = model.FirstName,
                StreetName = model.StreetName,
                PoBox = model.PoBox
            };

            var result = await _userManager!.CreateAsync(user, model.PassWord);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return GenerateJwtToken(model!.Email!, user);
        }

        [HttpPost]
        public async Task<ActionResult<UserToken>> renewPassWord([FromBody] LoginDto login)
        {

            var user = await _userManager!.FindByEmailAsync(login.Email);

            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, login.PassWord);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _signInManager!.PasswordSignInAsync(user, login.PassWord, false, false);

            return GenerateJwtToken(login.Email!, user);
        }

        [HttpPut("{Email}")]
        public async Task<ActionResult<UserToken>> editUserInformations([FromBody] PersonalData model, String Email)
        {
            var user = await _userManager!.FindByEmailAsync(Email);

            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return GenerateJwtToken(user.Email!, user);
        }

        [HttpPut("{Email}")]
        public async Task<ActionResult<UserToken>> editUserEmail([FromBody] EdtiEmail NewEmail, String Email)
        {
            var user = await _userManager!.FindByEmailAsync(Email);

            if (user == null) return NotFound();

            user.Email = NewEmail.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return GenerateJwtToken(user.Email!, user);
        }


        [HttpPut("{Email}")]
        public async Task<ActionResult<UserToken>> editAddressInformations([FromBody] AdressData model, String Email)
        {
            var user = await _userManager!.FindByEmailAsync(Email);

            if (user == null) return NotFound();

            user.StreetName = model.StreetName;
            user.City = model.City;
            user.Country = model.Country;
            user.PoBox = model.PoBox;
            user.HouseNumber = model.HouseNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return GenerateJwtToken(user.Email!, user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginDto login)
        {
            var user = await _userManager!.FindByEmailAsync(login.Email);

            if (user == null) return NotFound();

            var result = await _signInManager!.PasswordSignInAsync(user, login.PassWord, false, false);

            if (!result.Succeeded) return BadRequest("Invalid Login");

            return GenerateJwtToken(login!.Email!, user);
        }

        [HttpGet("{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<String>> ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration!["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var validator = new JwtSecurityTokenHandler();

            // These need to match the values used to generate the token
            TokenValidationParameters validationParameters = new TokenValidationParameters();
            validationParameters.ValidIssuer = _configuration["JwtIssuer"];
            validationParameters.ValidAudience = _configuration["JwtIssuer"];
            validationParameters.IssuerSigningKey = key;
            validationParameters.ValidateIssuerSigningKey = true;
            validationParameters.ValidateAudience = true;

            if (validator.CanReadToken(token))
            {

                try
                {
                    // This line throws if invalid
                    var result = await validator.ValidateTokenAsync(token, validationParameters);

                    // If we got here then the token is valid
                    if (result.IsValid)
                    {
                        return "AUTHENTICATED";
                    }
                    else
                    {
                        return "UNAUTHENTICATED";
                    }
                }
                catch (Exception e)
                {
                    _logger!.LogError(null, e);
                }
            }

            return String.Empty;
        }


        private UserToken GenerateJwtToken(String Email, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration!["JwtKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                 _configuration["JwtIssuer"],
                 _configuration["JwtIssuer"],
                 claims,
                 signingCredentials: credentials
                 );

            var loggedUserDto = _mapper!.Map<UserDto>(user);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = loggedUserDto.UserName,
                Email = loggedUserDto.Email,
                UserId = loggedUserDto.Id
            };
        }

    }
}