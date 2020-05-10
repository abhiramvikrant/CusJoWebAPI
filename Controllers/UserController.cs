using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CusJoWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AutoMapper;

namespace CusJoWebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
         public class UserController : Controller
    {
        private readonly CusJoAPIContext db;
        private readonly UserManager<ApplicationUser> umanager;
        private readonly SignInManager<ApplicationUser> smanager;
   
        private readonly IMapper mapper;
        private readonly RoleManager<IdentityRole> rmanager;

        public UserController(CusJoAPIContext db, UserManager<ApplicationUser> umanager,
            SignInManager<ApplicationUser> smanager, 
             IMapper mapper,RoleManager<IdentityRole> rmanager)
        {
            this.db = db;
            this.umanager = umanager;
            this.smanager = smanager;
         
            this.mapper = mapper;
            this.rmanager = rmanager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var aUser = new ApplicationUser();
            aUser.UserName = userDto.Username;
        
            var result = await umanager.CreateAsync(aUser);
            if (result.Succeeded)
            {
                var c = await umanager.FindByNameAsync(userDto.Username);
                var result1 = await umanager.AddPasswordAsync(c, userDto.Password);
                return Ok($"{aUser.UserName} created");
            }
                
            else
                return BadRequest("Error occured");
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDto userDto)
        {
            var c = await umanager.FindByNameAsync(userDto.Username);
            var result = await smanager.PasswordSignInAsync(c,userDto.Password, false, false);
            var user = await umanager.FindByNameAsync(userDto.Username);
            var role = await umanager.GetRolesAsync(user);
            IdentityOptions _options = new IdentityOptions();

            if (!result.Succeeded )
                return BadRequest(new { message = "Username or password is incorrect" });
            

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id),
                     new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            
            return Ok(new
            {
                
                Username = user.UserName,
               Token = tokenString
            });
        }

        [HttpPost]
        [Authorize( Roles ="admin" )]
        [Route("createrole")]
        public async Task<IActionResult> CreateRole([FromQuery]string rolename)
        {
            if (string.IsNullOrWhiteSpace(rolename))
            {
                //throw new ArgumentException(resManager.GetString("rolenamenull", CultureInfo.InvariantCulture), nameof(rolename));

            }

            IdentityRole role = new IdentityRole(rolename);
            var result = await rmanager.CreateAsync(role).ConfigureAwait(true);
            if (result.Succeeded)
                return Ok($"{rolename} created");
            else
                return BadRequest("error occured");
       

        }
        [HttpGet]
        [Authorize(Roles ="admin")]
        [Route("getalluser")]
        public IActionResult GetAllUser()
        {
            var list = db.Users.ToList();
            return Json(list);
           // return Ok("hello");
        }
        [HttpGet]
        [Authorize(Roles = "users")]
        [Route("getallroles")]
        public IActionResult GetAllRoles()
        {
            var r = db.Roles.ToList();
            return Json(r);
        }
        [HttpPost]
        [Authorize(Roles ="admin")]
        [Route("addusertoroles")]
        public async Task<IActionResult> AddUserToRoles(string username, string rolename)
        {       

           
            var role = rmanager.Roles.Where(r => r.Name== rolename).FirstOrDefault();
            var u = await umanager.FindByNameAsync(username).ConfigureAwait(true);
            var result = await umanager.AddToRoleAsync(u, role.Name).ConfigureAwait(true);
            if (result.Succeeded)
                return Ok("user added to role");
            else
                return BadRequest("Unable to add user to roles");
               
                

            




            
        }
    }
}