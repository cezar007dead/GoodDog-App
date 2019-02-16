using Sabio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{

    /// <summary>
    /// Provides basic functionality for Authentication, Authorization and access to App Customized Current IPrincipal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAuthenticationService<T> : IIdentityProvider<T>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="extraClaims"></param>
        /// <returns></returns>
        void LogIn(IUserAuthData user, params Claim[] extraClaims);

        /// <summary>
        /// Logs out the currently signed in user
        /// </summary>
        void LogOut();

        bool IsLoggedIn();


        /// <summary>
        /// Get the current IUserAuthData for the Current IPrincipal
        /// </summary>
        /// <returns></returns>
        IUserAuthData GetCurrentUser();
    }
}
