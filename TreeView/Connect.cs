using System.Text;
using System.Net;
using System.IO;
using System;

namespace TreeView.Connect
{
    class Connect
    {
        public void SetUsername(string text)
        {
            this.username = text;
        }

        public void SetPassword(string text)
        {
            this.password = text;
        }

        private void GetData(string token)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.token_URL);
            using (StreamWriter writer = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                writer.Write("Bearer=" + token);
            }
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception e)
            {
                return;
            }
        }

        public bool LoginRequest()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.URL);
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)";
            req.Method = "POST";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.Headers.Add("Accept-Language: en-us,en;q=0.5");
            req.Headers.Add("Accept-Encoding: gzip,deflate");
            req.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            req.KeepAlive = true;
            req.Headers.Add("Keep-Alive: 3155760000");
            req.Referer = "http://sso.bhmobile.ba/sso/login";

            req.ContentType = "application/x-www-form-urlencoded";

            using (StreamWriter writer = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                writer.Write("username=" + this.username + "&password=" + this.password);
            }
            HttpWebResponse response;
            try
            {
                 response = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception e)
            {
                return false;
            }

            this.logged_in = true;
            string xtoken = response.Headers["custom-header"];
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
        private bool logged_in = false;
        private string URL = "https://api.ellipsis-drive.com/v1/account/login";
        private string token_URL = "https://api.ellipsis-drive.com/v1/account/info";
    }
}
