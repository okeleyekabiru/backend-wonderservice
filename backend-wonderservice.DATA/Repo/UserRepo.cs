using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using backend_wonderservice.DATA.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plugins.JwtHandler;
using WonderService.Data.Services;
using WonderService.Data.ViewModel;

namespace WonderService.Data.Repo
{
public    class UserRepo:IUser
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IJwtSecurity _jwtSecurity;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepo(DataContext context,UserManager<User> userManager,IMapper mapper,IMailService mailService,IJwtSecurity jwtSecurity,SignInManager<User> signInManager,RoleManager<IdentityRole> roleManager, IHttpContextAccessor  httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _mailService = mailService;
            _jwtSecurity = jwtSecurity;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ReturnResult> Register(UserModel user)
        {
            var users = _mapper.Map<UserModel, User>(user);
            users.UserName = user.Email;
            var result = await _userManager.CreateAsync(users, user.Password);
            if (result.Succeeded)
            {
                var emailToken = _jwtSecurity.CreateTokenForEmail(users);
                _mailService.VerifyEmail(user.Email,emailToken.Token);
                return new ReturnResult { Succeeded = true};

            }

            return new ReturnResult
            {
                Succeeded = false,
                Error =  result.Errors.ElementAt(0).Description
            };
        }

        public async Task<JwtModel> Login(LoginModel user)
        {
            User users;
            string error = string.Empty;

            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);
            if (result.Succeeded)
            {
                 users = await _userManager.FindByEmailAsync(user.Email);
                var confirmEmail = await _userManager.IsEmailConfirmedAsync(users);
              var role =  await _userManager.GetRolesAsync(users);
            
                if (confirmEmail)
                {
                    var token = _jwtSecurity.CreateToken(users, role.FirstOrDefault());
                    return new JwtModel
                    {
                        Token = token.Token,
                        ExpiryDate = token.ExpiryDate
                    };
                    
                }
            }

           

            if (result.IsLockedOut) error = "account is locked out";
            else if (!result.Succeeded) error = "invalid email or password";
           
            else if (result.IsNotAllowed) error = "account  not allowed";
            else  error = "Email as not been verified";
        

            return new JwtModel
            {
                Error = error
            };


        }

        public async Task<User> Get(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

      

        public PagedList<User> GetAll(int pageNumber, int pageSize)
        {
            return  PagedList<User>.ToPagedList(_context.Users.OrderByDescending(e => e.FirstName),pageNumber,pageSize);
        }

        public async Task<ReturnResult> Delete(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new ReturnResult
                {
                    Succeeded= true
                };
            }

            return new ReturnResult
            {
                Error = result.Errors.ElementAt(0).Description
            };
        }


        public async Task<ReturnResult> Update(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ReturnResult
                {
                    Succeeded = true
                };
            }
            return new ReturnResult
            {
                Error = result.Errors.ElementAt(0).Description
            };
        }

        public async Task<bool> VerifyEmail(string email)
        {
         return   await _userManager.FindByEmailAsync(email) != null;
         
        }

        public async Task<ReturnResult> ConfirmEmail(string token)
        {
            var verifyToken = _jwtSecurity.ReadToken(token);
            var userId = verifyToken.Token;
           var user = await _userManager.FindByIdAsync(userId);
           if (user != null)
           {
               user.EmailConfirmed = true;
               _context.Update(user);
           
            if (await _context.SaveChangesAsync() > 0) return  new ReturnResult
            {
                Succeeded = true
            };
           }
           return  new ReturnResult
           {
               Succeeded = false,
               Error = "invalid user token or token as expired"
           };

        }

        public string GetCurrent()
        {
            return _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<User> UpdatePassword(User user,string newPassword)
        {
            var passwordToken =await _userManager.GeneratePasswordResetTokenAsync(user);
             var result =   await  _userManager.ResetPasswordAsync(user, passwordToken, newPassword);
            if (!result.Succeeded)
            {
                return null;
            }

            return user;
        }

        public async  Task<bool> VerifyOldPassword(User user, string oldPassword)
        {
            return await _userManager.CheckPasswordAsync(user, oldPassword);
        }
    }
}
