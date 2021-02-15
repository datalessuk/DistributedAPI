using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using System.Net.Http.HttpClient;

namespace DistSysACWClient
{
    #region Task 10 and beyond
    
    class Client
    {
        //public Hashtable pHashTable = new Hashtable();
        
        static HttpClient client = new HttpClient();
        static Hashtable pHashTable = new Hashtable();
        

        static string mApiKey;
        static string mUserName;

        public static async Task<string> Hello()
        {
            Console.WriteLine("...Please Wait...");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44307/Api/TalkBack/Hello");
                //http get 

                var newR = "";
                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result = responseTask.Result;
                //Console.WriteLine(result);
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    newR = readTask.Result.ToString();
                    //return newR;
                    //Console.WriteLine(newR);
                }
                return newR;
            }
        }
        public static async Task<string> Sort(string pNumbersInput) // Refactor 
        {
            Console.WriteLine("...Please Wait...");
            pNumbersInput = pNumbersInput.Trim();
            string message = null;
            if (pNumbersInput == "TalkBack Sort")
            {
                HttpResponseMessage a = await client.GetAsync("api/talkback/sort?");
                message = await a.Content.ReadAsStringAsync();
                return message;

            }
            pNumbersInput = pNumbersInput.Remove(0, 15);
            pNumbersInput = pNumbersInput.Replace("]", "");
            pNumbersInput = pNumbersInput.Trim();

            //string preturn = null;
            string[] numbers = pNumbersInput.Split(","); // splitting on the "," 

            string pURL = null;
            foreach (string x in numbers)
            {
                pURL += "integers=" + x.ToString() + "&";
            }
            pURL = pURL.Remove(pURL.Length - 1);

            using (var client = new HttpClient())
            {
                var newR = "";


                client.BaseAddress = new Uri("https://localhost:44307/Api/TalkBack/Sort?" + pURL);
                //https://localhost:44307/Api/TalkBack/Sort?

                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result = responseTask.Result;
                //Console.WriteLine(result);
                if (result.IsSuccessStatusCode)
                {
                    //preturn = preturn.Replace(@"""","");

                    var readTask = result.Content.ReadAsStringAsync();
                    newR = readTask.Result.ToString();
                    //return newR;
                    //Console.WriteLine(newR);
                }
                else
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    newR = readTask.Result.ToString();
                    
                }
                //else
                //{
                //newR = message;
                //}
                return newR; // this is returning the sorted numbers they have been set to a string already 

            }


        }
        public static async Task<string> Name(string pNameInput)
        {
            Console.WriteLine("...Please Wait...");

            pNameInput = pNameInput.Remove(0, 9);
            string message = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44307/Api/user/new?username=" + pNameInput);
            

                var newR = "";
                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result = responseTask.Result;
                //Console.WriteLine(result);
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    newR = readTask.Result.ToString();

                    //var readTask = result.Content.ReadAsStringAsync();
                    //newR = readTask.Result.ToString();
                    ////return newR;
                    ////Console.WriteLine(newR);
                }
                return newR;
            }
        }


        public static async Task<string> NamePost(string pNameInput)
        {
            Console.WriteLine("...Please Wait...");

            if (pNameInput.Trim() == "User Post")
            {
                return "Oops.Make sure your body contains a string with your username and" +
                    " your Content-Type is Content-Type:application/json";
            }
            pNameInput = pNameInput.Remove(0, 10);
            //username = pNameInput;
            String message = null;
            string pApiKey;
            pNameInput = (char)34 + pNameInput + (char)34;

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://localhost:44307/Api/user/new");
                
                HttpResponseMessage responce = await client.PostAsync(client.BaseAddress, new StringContent(pNameInput, Encoding.UTF8, "application/json"));
                message = await responce.Content.ReadAsStringAsync();
                if (responce.IsSuccessStatusCode)
                {
                    pApiKey = message;
                        //.Replace(@"""","");
                    if(pHashTable.Count > 0)
                    {
                        pHashTable.Clear();
                    }
                    

                    pHashTable.Add(pNameInput, pApiKey);//Cehck this to make sure its empty
                    message = "Got Api Key";
                }

            }

            return message;
        }

        public static async Task<string> UserSet(string pNameInput)
        {
            Console.WriteLine("...Please Wait...");
            string[] splitInput = pNameInput.Split(" "); // splitting the user name on the space 
            string name = default;
            string api = default;

            if (pHashTable.Count > 0)
            {
                pHashTable.Clear();
            }
            foreach(string input in splitInput)
            {
                name = splitInput[2].ToString().Trim();
                api = splitInput[3].ToString().Trim();
            }
            mApiKey = api;
            pHashTable.Add(name,api);

            
            return "Stored"; 

        }

        public static async Task<bool> Delete()
        {
            Console.WriteLine("...Please Wait...");
            string pMessage = default;
            string pUsername = default;
            string pApikey = default;

            if (pHashTable.Count == 0)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return false;
            }

            foreach (DictionaryEntry element in pHashTable)
            {
                //pUsername = element.Value.ToString();
                pUsername = element.Key.ToString().Trim('"');// Trims off the "" off the user name woo ! 
                pApikey = element.Value.ToString().Trim();
                
                //Console.WriteLine(pUsername);

            }
            //pApikey = pApikey.Replace(" ", " and ");
            //pUsername = pUsername.Replace(" ", " and ");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Remove("ApiKey");
                client.DefaultRequestHeaders.Add("ApiKey", pApikey);

                client.BaseAddress = new Uri("https://localhost:44307/Api/user/removeuser?username="+ pUsername);
                
                HttpResponseMessage response = await client.DeleteAsync(client.BaseAddress);

                //var responseTask = client.GetAsync("");

                //responseTask.Wait();
                //var result = responseTask.Result;

                if (response.IsSuccessStatusCode)
                {
                    if (pHashTable.Count > 0)
                    {
                        //pHashTable.Remove(pHashTable.Values);
                        //pHashTable.Remove(pHashTable.Keys);
                        //pHashTable.Remove(pHashTable.Keys.Count);
                        //pHashTable.Clear();
                        //return true;
                        pHashTable.Clear();
                        return true;
                        //foreach (DictionaryEntry elem
                        //ent in pHashTable)
                        //{

                        //    pHashTable.Remove(element.Key);
                        //    pHashTable.Remove(element.Value);
                        //    pHashTable.Clear();
                        //    //Console.WriteLine(element.Key);
                        //    return true;

                        //}
                    }
                }



                return false;

            }
            return false;
        }

        public static async Task<string> preotectedHello()
        {
            Console.WriteLine("...Please Wait...");
            string pMessage = default;

            if (pHashTable.Count == 0)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return pMessage;
            }
            foreach(DictionaryEntry element in pHashTable)
            {
                mApiKey = element.Value.ToString();
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Remove("ApiKey");
                client.DefaultRequestHeaders.Add("ApiKey", mApiKey);
                client.BaseAddress = new Uri("https://localhost:44307/Api/protected/hello");
                //client.BaseAddress = new Uri("http://distsysacw.azurewebsites.net/7467709/Api/protected/hello");
                HttpResponseMessage response = await client.GetAsync("");
                pMessage = await response.Content.ReadAsStringAsync();

                
                var responseTask = client.GetAsync("");
               
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    return pMessage;
                }


            }
            return pMessage;

        }
        public static async Task<string> protectedSha1(string pNameInput)
        {
            Console.WriteLine("...Please Wait...");
            string pMessage = default;
            try
            {
                if (pNameInput != default)
                {
                    pNameInput = pNameInput.Remove(0, 15);
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
            if (pHashTable.Count == 0)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return pMessage;
            }
            foreach (DictionaryEntry element in pHashTable)
            {
                mApiKey = element.Value.ToString();
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Remove("ApiKey");
                client.DefaultRequestHeaders.Add("ApiKey", mApiKey);

                client.BaseAddress = new Uri("https://localhost:44307/api/protected/sha1?message=" + pNameInput);
                
                HttpResponseMessage response = await client.GetAsync("");
                pMessage = await response.Content.ReadAsStringAsync();
                var responseTask = client.GetAsync("");

                responseTask.Wait();
                var result = responseTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    return pMessage;
                }
            }



            return pMessage;
        }

        public static async Task<string> protectedSHA256(string pNameInput)
        {

            Console.WriteLine("...Please Wait...");
            string pMessage = default;
            try
            {
                if (pNameInput != default)
                {
                    pNameInput = pNameInput.Remove(0, 17);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            if (pHashTable.Count == 0)
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return pMessage;
            }
            foreach (DictionaryEntry element in pHashTable)
            {
                mApiKey = element.Value.ToString();
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Remove("ApiKey");
                client.DefaultRequestHeaders.Add("ApiKey", mApiKey);

                client.BaseAddress = new Uri("https://localhost:44307/api/protected/sha256?message=" + pNameInput);
            
                HttpResponseMessage response = await client.GetAsync("");
                pMessage = await response.Content.ReadAsStringAsync();
                var responseTask = client.GetAsync("");

                responseTask.Wait();
                var result = responseTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    return pMessage;
                }
            }



            return pMessage;


        }

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://localhost:44307/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine("What would you like to do?");
            string whatcommand = Console.ReadLine();
            
            bool firstcommand = true;

            while (whatcommand != null)
            {
                if (!firstcommand)
                {
                    Console.Clear();
                }
                try
                {
                    if (whatcommand == "Exit" || whatcommand =="exit")
                    {
                        Environment.Exit(0);
                    }

                    switch (whatcommand)//For each command the user enters this will select the correct one , if a typo the user will be asked to try again 
                    {
                        case "TalkBack Hello":
                            string responce = await Hello();
                            Console.WriteLine(responce);
                            Console.WriteLine("What would you like to do next?");

                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        case string a when whatcommand.Contains("TalkBack Sort"):
                            string responceSort = await Sort(whatcommand);
                            Console.WriteLine(responceSort);
                            Console.WriteLine("What would you like to do next?");
                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        case string c when whatcommand.Contains("User Get"):
                            string responceNew = await Name(whatcommand);
                            Console.WriteLine(responceNew);
                            Console.WriteLine("What would you like to do next?");
                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        
                        case string c when whatcommand.Contains("User Post"):
                            string responceNewPost = await NamePost(whatcommand);
                            Console.WriteLine(responceNewPost);
                            Console.WriteLine("What would you like to do next?");
                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        case string c when whatcommand.Contains("User Set"):
                            string responceNewSet = await UserSet(whatcommand);
                            Console.WriteLine(responceNewSet);
                            Console.WriteLine("What would you like to do next?");
                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        case "User Delete":
                            bool responceDelete = await Delete();
                            Console.WriteLine(responceDelete);
                            Console.WriteLine("What would you like to do next?");

                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        case "Protected Hello":
                            string responcePHello = await preotectedHello();
                            Console.WriteLine(responcePHello);
                            Console.WriteLine("What would you like to do next?");

                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        case string c when whatcommand.Contains("Protected SHA1"):
                            string responceSHA1 = await protectedSha1(whatcommand);
                            Console.WriteLine(responceSHA1);
                            Console.WriteLine("What would you like to do next?");
                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;
                        case string c when whatcommand.Contains("Protected SHA256"):
                            string responceSHA256 = await protectedSHA256(whatcommand);
                            Console.WriteLine(responceSHA256);
                            Console.WriteLine("What would you like to do next?");
                            firstcommand = false;
                            whatcommand = Console.ReadLine();
                            break;

                        default:
                            Console.WriteLine("The Command "+ whatcommand +" is not valid , please enter a valid command");
                            firstcommand = false;
                            Console.WriteLine("What would you like to do next?");
                            whatcommand = Console.ReadLine();
                            break;
                    }



                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //Console.ReadLine();
            }

            
        }
    }
}

#endregion