using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistSysACW.Filters
{
    public class AuthFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)

        {
            bool adminOnly = true;
            try
            {
                AuthorizeAttribute authAttribute = (AuthorizeAttribute)context.ActionDescriptor.EndpointMetadata.Where(e => e.GetType() == typeof(AuthorizeAttribute)).FirstOrDefault();

                
                if (authAttribute != null)
                {
                    string[] roles = authAttribute.Roles.Split(',');
                    int userCounter = roles.Length;
                    {
                        foreach (string role in roles)
                        {
                            if (role == "User")// can be used by admin or user
                            {
                                if (context.HttpContext.User.IsInRole(role))
                                {
                                    //istrue = false;
                                    return;
                                }
                                else if (userCounter == 1)
                                {
                                    context.HttpContext.Response.StatusCode = 401;
                                    context.Result = new JsonResult("Unauthorized.");
                                    //istrue = false;
                                    userCounter--;

                                }
                            }
                            else if (role == "Admin")//Setting the admin only 
                            {
                                if (context.HttpContext.User.IsInRole(role))
                                {
                                    return;
                                }
                                if (userCounter == 1)
                                {
                                    adminOnly = false; // So it goes to the final statment in the catch 
                                    context.HttpContext.Response.StatusCode = 401;
                                    context.Result = new JsonResult("Unauthorized. Admin access only.");
                                }

                            }
                            else
                            {
                                context.HttpContext.Response.StatusCode = 401;
                                context.Result = new JsonResult("Unauthorized.");
                            }
                        }
                        throw new UnauthorizedAccessException();
                    }

                }

                
                
            }
            catch
            {
                if (!adminOnly)
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new JsonResult("Unauthorized. Admin access only."); // Will drop here if admin access is needed 
                }
                else
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new JsonResult("Unauthorized. Check ApiKey in Header is correct."); // will drop here if the wrong API key for the user is entered or none at all ? 
                }
                
                
            }
        }
    }
}