using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using backend_wonderservice.DATA.Models;
using backend_wonderservice.DATA.Pagination;
using Plugins.JwtHandler;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Abstration
{
    public interface IUser
    {
        Task<ReturnResult> Register(UserModel user);
        Task<JwtModel> Login(LoginModel user);
        Task<User> Get(string id);
        PagedList<User> GetAll(int pageNumber, int pageSize);
        Task<ReturnResult> Delete(User user);
        Task<ReturnResult> Update(User user);
        Task<bool> VerifyEmail(string email);
        Task<ReturnResult> ConfirmEmail(string token);
        string GetCurrent();
    }
}
