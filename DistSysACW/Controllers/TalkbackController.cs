using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

namespace DistSysACW.Controllers
{
    public class TalkBackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkBackController(Models.UserContext context) : base(context) { }

        //[Authorize(Roles ="Admin")]
        [ActionName("Hello")]
        public string Get()
        {
            #region TASK1
            // TODO: add api/talkback/hello response
            #endregion
            return "Hello World";
        }

        [ActionName("Sort")]
        public IActionResult Get([FromQuery]string[] integers)
        {
            bool numberRequest = true;
            this.Request.Query.TryGetValue("integers", out var values);
            
            
                for (int x = 0; x <= values.Count; x++){
                try
                {
                    x = Int32.Parse(values);
                    numberRequest = false;

                }
                catch
                {
                   
                }
                                 
            }
            if (numberRequest)
            {
                try
                {
                    int[] sortedInt = new int[integers.Length];
                    for (int i = 0; i < integers.Length; i++)
                    {
                        sortedInt[i] = int.Parse(integers[i]);
                    }
                    Array.Sort(sortedInt);
                    return Ok(sortedInt);
                }
                catch
                {
                    return BadRequest("Bad Request");
                }
            }
            else
            {
                
            }
            return BadRequest("Bad Request");
        }
       
    
    }


    
}
