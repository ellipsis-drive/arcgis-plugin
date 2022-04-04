using System.Net;
using System.IO;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ellipsis.Api
{
    class Connect
    {
        private void DisplayLoginError()
        {
            string message = "Please enter your correct username and password.";
            string title = "Login failed";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void SetUsername(string text)
        {
            this.username = text;
        }

        public void SetPassword(string text)
        {
            this.password = text;
        }

        public bool GetStatus()
        {
            return this.logged_in;
        }

        public JObject GetPath(string pathId, bool isFolder, string pageStart, bool isRoot)
        {
            string route = isRoot ? "path/listRoot" : "path/listFolder";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"{path_URL}/{route}");

            var body = JsonConvert.SerializeObject(new
            {
                root = pathId,
                pathId = pathId,
                type = isFolder ? "folder" : "map",
                pageStart = pageStart
            });

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + login_token);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(body);
                streamWriter.Flush();
            }


            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpResponse.StatusDescription != "OK") return null;

            using (var reader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                try
                {
                    JObject data = JObject.Parse(responseFromServer);
                    return data;
                    /*return data;
                    foreach (JObject item in data["result"]) // <-- Note that here we used JObject instead of usual JProperty
                    {
                        foreach (JProperty jp in item.Properties())
                        {
                            Debug.WriteLine("kom op:");
                            Debug.WriteLine(jp.Name);
                        }
                    }
                    return "result";*/
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return null;
        }

        public JObject SearchByName(string name, string type, string pageStart)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"{path_URL}/account/{type}");
            var body = JsonConvert.SerializeObject(new
            {
                canView = true,
                access = new string[] { "public", "subscribed", "owned", "favorited" },
                name = name,
                pageStart = pageStart
            });

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + login_token);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(body);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpResponse.StatusDescription != "OK") return null;

            using (var reader = new StreamReader(httpResponse.GetResponseStream()))
            {
                try
                {
                    return JObject.Parse(reader.ReadToEnd());
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return null;
        }

        public bool LoginRequest()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.URL);
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Headers.Add("Keep-Alive: 3155760000");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new { username = this.username, password = this.password });
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            
            HttpWebResponse httpResponse;
            try
            {
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception e)
            {
                DisplayLoginError();
                return false;
            }

            this.logged_in = true;
            Stream dataStream = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            try
            {
                dynamic data = JObject.Parse(responseFromServer);
                login_token = data["token"];
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return true;
            /*
            //Retrieve your cookie that id's your session
            //response.Cookies
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                Console.WriteLine(reader.ReadToEnd());
            }
            return false;
            */
        }

        private string username;
        private string password;
        private string login_token;
        private bool logged_in = false;
        private string URL = "https://api.ellipsis-drive.com/v1/account/login";
        private string path_URL = "https://api.ellipsis-drive.com/v1";
    }
}
