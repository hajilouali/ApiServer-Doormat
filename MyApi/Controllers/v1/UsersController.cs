using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Data.Repositories;
using ElmahCore;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApi.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;
using Common;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Filters;
using System.Linq;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    //[AllowAnonymous]
    public class UsersController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UsersController> logger;
        private readonly IJwtService jwtService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<AccountingHeading> _accountingHeading;
        private readonly IRepository<Factor> _Factor;
        private readonly IRepository<Expert> _Expert;
        private readonly IRepository<Role> _Role;
        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, IJwtService jwtService,
            UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IRepository<Client> ClientRepository, IRepository<AccountingHeading> AccountingHeading, IRepository<Factor> factor, IRepository<Expert> expert, IRepository<Role> role = null)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.jwtService = jwtService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this._clientRepository = ClientRepository;
            this._accountingHeading = AccountingHeading;
            _Factor = factor;
            _Expert = expert;
            _Role = role;
        }
        [HttpGet("[action]")]
        [Authorize]
        public virtual async Task<ActionResult<bool>> CheckTokenIsValid(CancellationToken cancellationToken)
        {
            try
            {
                var user =await userManager.FindByNameAsync(User.Identity.Name);
                if (user!=null)
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            catch 
            {
                return Ok(false);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<List<UserModel>>> Get(CancellationToken cancellationToken)
        {
            //var userName = HttpContext.User.Identity.GetUserName();
            //userName = HttpContext.User.Identity.Name;
            //var userId = HttpContext.User.Identity.GetUserId();
            //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);

            var users = await userRepository.TableNoTracking.ToListAsync(cancellationToken);
            List<UserModel> list = new List<UserModel>();
            foreach (var item in users)
            {
                list.Add(new UserModel()
                {
                    id = item.Id,
                    IsActive = item.IsActive,
                    Email = item.Email,
                    FullName = item.FullName,
                    UserName = item.UserName,
                    RollName = await userManager.GetRolesAsync(item)
                });
            }
            return Ok(list);
        }
        [HttpPost("[action]")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<UserDto>> CreateUser(UserDto userDto, CancellationToken cancellationToken)
        {
            logger.LogError("متد Create فراخوانی شد");
            HttpContext.RiseError(new Exception("متد Create فراخوانی شد"));

            var exists = await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
            if (exists)
                return BadRequest("نام کاربری تکراری است");
            Services.Bessines.UsersProcess usersProcess = new Services.Bessines.UsersProcess(signInManager, userManager, roleManager, _clientRepository, _accountingHeading, _Factor, _Expert);

            User user = new User()
            {
                UserName = userDto.UserName,
                PasswordHash = userDto.Password,
                Email = userDto.Email,
                FullName = userDto.FullName
            };
            var result = await usersProcess.AddUserToApp(user, null);
            if (result != true)
                throw new BadRequestException("مشکلی در فرایند ثبت کاربر ایجاد شده است");
            //var user = new User
            //{

            //    FullName = userDto.FullName,

            //    UserName = userDto.UserName,
            //    Email = userDto.Email
            //};
            //var result = await userManager.CreateAsync(user, userDto.Password);



            //var result3 = await userManager.AddToRoleAsync(user, "CellPartner");

            //await userRepository.AddAsync(user, userDto.Password, cancellationToken);
            return Ok(userDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public virtual async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            //var user2 = await userManager.FindByIdAsync(id.ToString());
            //var role = await roleManager.FindByNameAsync("Admin");

            var user = await userRepository.GetByIdAsync(cancellationToken, id);
            if (user == null)
                return NotFound();

            
            //await userRepository.UpdateSecuirtyStampAsync(user, cancellationToken);

            return user;
        }
        [Authorize]
        [HttpGet("[action]")]
        public virtual async Task<ApiResult<UserModel>> GetCorentUserBio( CancellationToken cancellationToken)
        {
            //var user2 = await userManager.FindByIdAsync(id.ToString());
            //var role = await roleManager.FindByNameAsync("Admin");

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var client = await _clientRepository.TableNoTracking.Where(p => p.User_ID == user.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                return NotFound();            
            var dto= new UserModel()
            {
                id=user.Id,
                IsActive=user.IsActive,
                Email=user.Email,
                FullName=user.FullName,
                UserName=user.UserName,
                RollName=await userManager.GetRolesAsync(user),
                
            };
            if (client!=null)
            {
                dto.Discount = client.DiscountPercent;
                dto.UserAddress = client.ClientAddress;
                dto.UserPhone = client.ClientPhone;
            }
            return dto;
        }
        /// <summary>
        /// This method generate JWT Token
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Token([FromForm]TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            if (!tokenRequest.grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password.");

            //var user = await userRepository.GetByUserAndPass(username, password, cancellationToken);
            var user = await userManager.FindByNameAsync(tokenRequest.username);
            if (user == null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, tokenRequest.password);
            if (!isPasswordValid)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");


            //if (user == null)
            //    throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var jwt = await jwtService.GenerateAsync(user);
            return new JsonResult(jwt);
        }        
        [Authorize(Roles = "Admin")]
        [HttpPut("[action]/{id}")]
        public virtual async Task<ApiResult> Update(int id, UserDto UserDto, CancellationToken cancellationToken)
        {
            var updateUser = await userRepository.GetByIdAsync(cancellationToken, id);

            updateUser.UserName = UserDto.UserName;
            updateUser.FullName = UserDto.FullName;
            updateUser.Email = UserDto.Email;
            updateUser.IsActive = UserDto.IsActive;


            await userManager.UpdateAsync(updateUser);
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]/{id}")]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user.Factors==null|| user.ExpertHistories == null || user.ManufactureHistories == null || user.Clients.SingleOrDefault().AccountingHeading.Sanads == null)
            {
                var rols = await userManager.GetRolesAsync(user);
                if (rols.Count!=0)
                {
                    await userManager.RemoveFromRolesAsync(user, rols);

                }
                
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return BadRequest("در فرایند پاک کردن کاربر مشکل پیش امده ");

                return Ok();
            }
            return BadRequest("در فرایند پاک کردن کاربر مشکل پیش امده ");
        }
        [HttpGet("[action]/{id}/{newpassword}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ApiResult> setPassword(int id ,string newpassword)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            var token = await userManager.RemovePasswordAsync(user);
            var settpassword = await userManager.AddPasswordAsync(user, newpassword);
            if (!settpassword.Succeeded)
                return BadRequest("اطلاعات وارد شده معتبر نمی باشد");
            return Ok();
        }
        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<List<string>>> GetUserRolls(int userid)
        {
            var useer =await userManager.FindByIdAsync(userid.ToString());
            var rolss =await userManager.GetRolesAsync(useer);
            return Ok(rolss.ToList());
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<List<Role>>> GeAllRolls(CancellationToken cancellationToken)
        {
            var useer =await _Role.Entities.AsNoTracking().ToListAsync(cancellationToken);
            
            return Ok(useer);
        }
        [HttpGet("[action]/{RollName}")]
        [Authorize(Roles = "Admin,Accountants")]
        public virtual async Task<ActionResult<List<UserModel>>> GetUsersInRoll(string RollName, CancellationToken cancellationToken)
        {
            //var userName = HttpContext.User.Identity.GetUserName();
            //userName = HttpContext.User.Identity.Name;
            //var userId = HttpContext.User.Identity.GetUserId();
            //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);

            var users = await userManager.GetUsersInRoleAsync(RollName);
            List<UserModel> list = new List<UserModel>();
            foreach (var item in users)
            {
                list.Add(new UserModel()
                {
                    id = item.Id,
                    IsActive = item.IsActive,
                    Email = item.Email,
                    FullName = item.FullName,
                    UserName = item.UserName,
                    RollName = await userManager.GetRolesAsync(item)
                });
            }
            return Ok(list);
        }
        [HttpGet("[action]/{id}/{Roll}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ApiResult> addUserstoRolls(int id, string Roll, CancellationToken cancellationToken)
        {
            //var userName = HttpContext.User.Identity.GetUserName();
            //userName = HttpContext.User.Identity.Name;
            //var userId = HttpContext.User.Identity.GetUserId();
            //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);
            var user = await userManager.FindByIdAsync(id.ToString());
            var users = await userManager.AddToRoleAsync(user, Roll);
            if (users.Succeeded)
            {
                return Ok();
            }
            throw new BadRequestException("مشکلی در فرایند ایجاد شده است");
        }
        [HttpGet("[action]/{id}/{Roll}")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ApiResult> removeUsersfromRolls(int id, string Roll, CancellationToken cancellationToken)
        {
            //var userName = HttpContext.User.Identity.GetUserName();
            //userName = HttpContext.User.Identity.Name;
            //var userId = HttpContext.User.Identity.GetUserId();
            //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);
            var user = await userManager.FindByIdAsync(id.ToString());
            var users = await userManager.RemoveFromRoleAsync(user, Roll);
            if (users.Succeeded)
            {
                return Ok();
            }
            throw new BadRequestException("مشکلی در فرایند ایجاد شده است");
        }
    }
}
