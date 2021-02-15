using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

namespace DistSysACW.Controllers
{
    [Authorize(Roles ="User,Admin")]
    public class ProtectedController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public ProtectedController(Models.UserContext context) : base(context) { }

        //[Authorize(Roles = "User,Admin")]
        [HttpGet]
        [ActionName("Hello")]//Need To fix
        public IActionResult Get([FromHeader]string apikey)
        {
            using (var pCtx = new UserContext())
            {
                
                User user = UserDatabaseAccess.returnUserfromApi(apikey, pCtx);
                if (user == default)
                {
                    return BadRequest("Something went wrong");
                }
                else
                {
                    return Ok("Hello " + user.UserName.ToString());
                }
                
            }
        }


        [HttpGet]
        [ActionName("SHA1")]
        public IActionResult Sha1([FromQuery]string pMessage)
        {
            using (var pCtx = new UserContext())
            {
                this.Request.Query.TryGetValue("message", out var values);
                string verify = values.ToString();

                if (verify.IsNull() || verify == "")
                {
                    return BadRequest("Bad Request");
                }
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    var pHash = sha1.ComputeHash(Encoding.UTF8.GetBytes(values));
                    var sb = new StringBuilder(pHash.Length * 2);

                    foreach (byte b in pHash)
                    {
                         // x2 will give lower case, needs to be upper 
                        sb.Append(b.ToString("X2"));
                    }
                    return Ok(sb.ToString());

                }

            }
        }
        [HttpGet]
        [ActionName("SHA256")]
        public IActionResult Sha256([FromQuery]string pMessage)
        {
            using (var pCtx = new UserContext())
            {
                this.Request.Query.TryGetValue("message", out var values);
                string verify = values.ToString();
                if (verify.IsNull() || verify == "")
                {
                    return BadRequest("Bad Request");
                }
                var pCrypt = new SHA256Managed();
                var pHash = new StringBuilder();
                byte[] crypto = pCrypt.ComputeHash(Encoding.UTF8.GetBytes(values));
                foreach (byte theByte in crypto)
                {
                    // x2 will give lower case, needs to be upper 
                    pHash.Append(theByte.ToString("X2"));
                }
                return Ok(pHash.ToString());
            }
           
        }


    }
}
