using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysACW.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        #endregion
        public User()
        {

        }
        [Key]
        public string APIKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }

    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    public static class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 

        public static void NewUser(string pUserName, UserContext pContext)
        {
            Guid guid = Guid.NewGuid();
            User ifadmin = pContext.Users.Where(a => a.Role == "Admin").FirstOrDefault();
            string userRole;
            
            //Making sure the user is null to not override any excisting users
            if(ifadmin == default)
            {
                //Setting user role to admin 
                userRole = "Admin";
            }
            else
            {
                //setting user role to user
                userRole = "User";
            }
            //Making final user
            User newUser = new User() { UserName = pUserName, APIKey = guid.ToString(), Role = userRole };
            //adding user into userContext
            pContext.Add(newUser);
            //Saving the changes 
            pContext.SaveChanges();

        }
        public static bool CheckAPIKey(string pApiKey,UserContext pContext)
        {
            //Only checking the API Key 
            User newUser = pContext.Users.FirstOrDefault(a => a.APIKey == pApiKey);
            if(newUser ==default)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool removeUser(string pApikey, string username, UserContext context)//Change this somehow
        {
            User newUser = context.Users.FirstOrDefault(a => a.APIKey == pApikey);
            User newUserAPI = context.Users.FirstOrDefault(a => a.UserName == username);
            if (newUser ==default)
            {
                return false;
            }
            else if (newUserAPI == default) {
                return false;
            }
            else if (newUser != newUserAPI)
            {
                return false;
            }
            else
            {
                //removing the user if all else is false 
                context.Users.Remove(newUser);
                context.SaveChanges();
                return true;
            }
            
            
        }
        public static bool CheckAPIandUserName(string pApiKey,string pUserName,UserContext pContext)
        {
            //checking both the user name and API Key
            User newUser = pContext.Users.First(a => a.APIKey == pApiKey && a.UserName ==pUserName);//First
            if(newUser == default)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool CheckUserNameonly(string pUserName, UserContext pContext)
        {
            User newUser = pContext.Users.FirstOrDefault(a => a.UserName == pUserName);
            if(newUser ==default)
            {
                return false;
            }
                return true;
            
        }

        public static User returnUserfromApi(string pApiKey,UserContext pContext)
        {
            User newUser = pContext.Users.FirstOrDefault(a => a.APIKey == pApiKey);
            if(newUser == newUser)
            {
                return newUser;
            }
            return newUser;
        }

        public static string returnAPIkeyfromUser(string pUsername, UserContext pContext)
        {
            User newUser = pContext.Users.FirstOrDefault(a => a.UserName == pUsername);
            

            string Api = newUser.APIKey.ToString();//Getting the API and saving it to the string API 
            return Api;//Method is returning the API only no saved changes 
        
        }

        #endregion
        //Method to test of a object is null
        public static bool IsNull(this object T)
        {
            return T == null;
        }


    }


    



}

//https://localhost:44307/api/user/new?username=user