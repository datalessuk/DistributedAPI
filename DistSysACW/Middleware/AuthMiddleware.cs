using DistSysACW.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DistSysACW.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        //private ClaimsIdentity name;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, Models.UserContext dbContext)
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then set the correct roles for the User, using claims
            #endregion
            var allClaims = new List<Claim>();
       
            ClaimsIdentity useridentiy = null;
            string apiKey = context.Request.Headers["ApiKey"];//x-api-key header for authentication
            bool inDataBase = UserDatabaseAccess.CheckAPIKey(apiKey, dbContext);

            if (inDataBase)//Simple if data in the database etc 
            {
                User mUser;
                mUser = UserDatabaseAccess.returnUserfromApi(apiKey, dbContext);
                string userName;
                userName = mUser.UserName;
                string userRole;
                userRole = mUser.Role;

                Claim cUserName;
                cUserName = new Claim(ClaimTypes.Name, userName);

                Claim cUserrole;
                cUserrole = new Claim(ClaimTypes.Role, userRole);
                
                allClaims.Add(cUserName);
                allClaims.Add(cUserrole);

                useridentiy = new ClaimsIdentity(allClaims, apiKey);
                context.User.AddIdentity(useridentiy);// adds the claims here that i have set 
            }

            
            await _next(context);// 
        }

    }
}
