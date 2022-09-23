using System.Net;
using System.IO;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Environment;
using Microsoft.Win32;

namespace Ellipsis.Api
{
    class Connect
    {
        public string SubmitHTTPRequest(string method, string URL)
        {
            String responseText = "";

            HttpWebRequest request;
            Uri uri = new Uri(URL);
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "text/xml; charset=UTF-8";
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            if (method.Equals("DOWNLOAD"))
            {
                FileStream file = null;
                Debug.WriteLine(response);
                string fileName = response.GetResponseHeader("Content-Disposition");
                Debug.WriteLine("break3");
                Debug.WriteLine(fileName);

                string[] s = null;
                if (fileName.ToLower().EndsWith(".tif"))
                {
                    s = URL.Split(new String[] { "coverage=" }, 100, StringSplitOptions.RemoveEmptyEntries);
                    s[1] = s[1].Trim() + ".tif";
                }
                else
                {
                    s = fileName.Split('=');
                    s[1] = s[1].Replace('\\', ' ');
                    s[1] = s[1].Replace('"', ' ');
                    s[1] = s[1].Trim();
                }
                
                try
                {
                    Debug.WriteLine("filename");
                    Debug.WriteLine(GetSpecialFolderPath());
                    downloadFileName = System.IO.Path.Combine(GetSpecialFolderPath(), s[1]);
                    Debug.WriteLine("filename");
                    Debug.WriteLine(downloadFileName);
                    System.IO.File.Delete(downloadFileName);
                    file = System.IO.File.Create(downloadFileName);
                    // Buffer to read 10K bytes in chunk:
                    byte[] buffer = new Byte[10000];
                    int length = 10000;
                    int offset = 0;
                    while (length > 0)
                    {
                        length = responseStream.Read(buffer, 0, length);
                        offset += length;
                        file.Write(buffer, 0, length);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("wrong");
                }
                finally
                {
                    if (file != null)
                        file.Close();
                    if (responseStream != null)
                        responseStream.Close();
                }

                return downloadFileName;
            }
            StreamReader reader = new StreamReader(responseStream);
            responseText = reader.ReadToEnd();

            reader.Close();
            responseStream.Close();

            return responseText;
        }

        private static string GetSpecialFolderPath()
        {
            string folderPath = "";
            string configFileDir = null;
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\PDOK\\Applications\\PDOK4ArcGIS");
            if (regKey != null) configFileDir = (String)regKey.GetValue("ConfigFileDir");
            if (configFileDir == null) configFileDir = ExecutingAssemblyPath();
            Debug.WriteLine("Break");
            try
            {
                System.IO.StreamReader fStream = System.IO.File.OpenText(configFileDir + "\\CswConfigPath.properties");
                folderPath = fStream.ReadLine();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Wrong");
            }
            return folderPath;
        }

        private static string ExecutingAssemblyPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
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

        public string GetLoginToken()
        {
            return this.login_token;
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

            try
            {
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
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
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

        public bool LogoutRequest()
        {
            this.username = "";
            this.password = "";
            this.login_token = "";
            this.logged_in = false;
            return false;
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

        private String downloadFileName = "";
        private string username;
        private string password;
        private string login_token;
        private bool logged_in = false;
        private string URL = "https://api.ellipsis-drive.com/v1/account/login";
        private string path_URL = "https://api.ellipsis-drive.com/v1";
    }
}
