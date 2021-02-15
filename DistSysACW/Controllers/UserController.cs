using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json.Linq;

namespace DistSysACW.Controllers
{
    public class UserController: BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public UserController(Models.UserContext context) : base(context) { }

        [HttpGet]
        [ActionName("New")]
        public IActionResult Get([FromQuery]string unsusedString)

        {

            using (var pValue = new UserContext())
            {
                this.Request.Query.TryGetValue("username", out var value);
                if (UserDatabaseAccess.CheckUserNameonly(value, pValue))
                {
                    return Ok("\"True - User Does Exist! Did you mean to do a POST to create a new user?\"");
                }
                else
                {
                    return Ok("\"False - User Does Not Exist! Did you mean to do a POST to create a new user?\"");
                }
                
            }

        }

        [HttpPost]
        [ActionName("New")]
        public IActionResult Post([FromBody]string pBodycontent)
        {
            using (var pCtx = new UserContext())
            {
                if (pBodycontent ==default)
                {
                    return BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
                }
                pBodycontent = pBodycontent.Trim('"').ToString().Trim();
                bool usernameExists = UserDatabaseAccess.CheckUserNameonly(pBodycontent, pCtx);
                if (!usernameExists)
                {
                    UserDatabaseAccess.NewUser(pBodycontent, pCtx);
                    string apikey = UserDatabaseAccess.returnAPIkeyfromUser(pBodycontent, pCtx);
                    return Ok(apikey);
                }
                else
                {
                    string Returnerror = "Oops. This username is already in use. Please try again with a new username.";
                    int errorCode = (int)HttpStatusCode.Forbidden;
                    return StatusCode(errorCode, Returnerror);
                }
            }
        }

     

        [Authorize(Roles = "User, Admin")]
        [HttpDelete]
        [ActionName("removeuser")]
        public IActionResult Delete([FromQuery] string username)
        {
            using (var pCtx = new UserContext())
            {

                this.Request.Query.TryGetValue("username", out var pUserName);
                this.Request.Headers.TryGetValue("ApiKey", out var pApiKey);

                User newUser = UserDatabaseAccess.returnUserfromApi(pApiKey, pCtx);
                try//Used for testing 
                {

                    if (newUser ==default)
                    {
                        return Ok(false);//If newuser does not excist then return false 
                    }
                   else if (newUser.APIKey == pApiKey && newUser.UserName == pUserName)
                    {
                       
                            return Ok(UserDatabaseAccess.removeUser(pApiKey, username, pCtx));//If both match then use method to remove the user and return ok 

                    }
                    else
                    {
                        return Ok(UserDatabaseAccess.removeUser(pApiKey, username, pCtx));
                    }
                }
                catch(Exception e)
                {
                    return BadRequest("NOT DONE: An error occured");
                }
                
            }
        }
        [Authorize(Roles = "Admin")]// Maybe mod this 
        [HttpPost]
        [ActionName("ChangeRole")]
        public IActionResult Changerole([FromBody]JObject bodycontent, [FromHeader] string adminApikey)
        {
            using (var ctx = new UserContext())
            {
                var dataFromJson = JObject.Parse(bodycontent.ToString());
                if (dataFromJson ==default)
                {
                    return BadRequest("NOT DONE: An error occured");//if the datafrom the jObject is null / empty return this last bad Rquest 
                }
                else
                {

                    try
                    {
                        //Bools to check if user & role is exists 
                        bool validRole = false;
                        bool userExists = true;

                        //Getting the username and Role
                        string username = dataFromJson["username"].ToString().Trim();
                        string newRole = dataFromJson["role"].ToString().Trim();

                        //Setting valid role to true if role is Admin or User , added low case for testing not needed 
                        if ((newRole.ToString().Trim() == "Admin") || (newRole.ToString().Trim() == "User") || (newRole.ToString().Trim() == "admin") || (newRole.ToString().Trim() == "user"))
                        {
                            validRole = true;
                        }

                        User newUser = ctx.Users.FirstOrDefault(s => s.UserName == username);

                        if (newUser ==default)
                        {//Making sure the new user is not null , if so the user name is not in the database
                            userExists = false;
                            return BadRequest("NOT DONE: Username does not exist");
                        }
                        else if (!validRole)
                        {
                            return BadRequest("NOT DONE: Role does not exist");//if the role is false then the user does not exist
                        }
                        else if (userExists && validRole)
                        {
                            newUser.Role = newRole; // changing the role for the intended user 
                            ctx.SaveChanges();
                            return Ok("DONE");
                        }


                        return BadRequest("NOT DONE: An error occured");//if anything else happens return an error 


                    }

                    catch (Exception e)
                    {
                        if (dataFromJson == default)
                        {
                            return BadRequest("NOT DONE: An error occured");// if all above fails 
                        }
                    }
                }
                    return BadRequest("NOT DONE: An error occured");//if all else fails ? 
            }
                

        }
        
    }

    

}