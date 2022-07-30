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


namespace user_service.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User>? _userManager;
        private readonly SignInManager<User>? _signInManager;
        private IConfiguration? _configuration;
        private readonly IMapper? _mapper;
        private readonly UserDbContext? _context;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            UserDbContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
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
            return _mapper!.Map<UserDto>(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserToken>> Register([FromBody] RegisterDto model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await _userManager!.CreateAsync(user, model.passWord);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return GenerateJwtToken(model!.Email!, user);
        }

        [HttpPost]
        public async Task<ActionResult<UserToken>> renewPassWord([FromBody] LoginDto login)
        {

            var user = await _userManager!.FindByEmailAsync(login.Email);

            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, login.PassWortd);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _signInManager!.PasswordSignInAsync(user, login.PassWortd, false, false);

            return GenerateJwtToken(login.Email!, user);
        }

        [HttpPost]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginDto login)
        {
            var user = await _userManager!.FindByEmailAsync(login.Email);

            if (user == null) return NotFound();

            var result = await _signInManager!.PasswordSignInAsync(user, login.PassWortd, false, false);

            if (!result.Succeeded) return BadRequest("Invalid Login");

            return GenerateJwtToken(login!.Email!, user);
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