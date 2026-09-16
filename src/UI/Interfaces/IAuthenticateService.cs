using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Interfaces
{
    public interface IAuthenticateService
    {
        Task<RegistrationResult> Login(string username, string password);
        Task<RegistrationResult> Signup(string username, string password);
        Task Logout();
    }
}