using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MtlMiriDemo_V1
    {
    public class Request
        {
        string url = "";
        public static dynamic CreatePostReq(string url, string req)
            {
            WebRequest webRequest = WebRequest.Create(url);
            byte[] bytes = Encoding.ASCII.GetBytes(req);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            string authorization = ConfigurationManager.AppSettings["LOGIN_TICKET"];
            webRequest.Headers.Add("Authorization", "170235694817863420956507894213478935");
            try
                {
                using (Stream stream = webRequest.GetRequestStream())
                    {
                    stream.Write(bytes, 0, bytes.Length);
                    }

                HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                return new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();

                }
            catch (WebException e)
                {

                return null;
                }
            }

        public static dynamic CreateGetReq(string url, string token, string identifier)
            {

            //url = url + token;
            url = "https://qtebt.manipaltechnologies.com/MIRISYSTEMS/Services/MIServiceDecoder/Verify"+"/"+"MiriToken"+"/" + identifier + "/" + token;
            WebRequest webRequest = WebRequest.Create(url);
            //byte[] bytes = Encoding.ASCII.GetBytes(req);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET"
                ;
            try
                {

                var webResponse = webRequest.GetResponse();
                var webStream = webResponse.GetResponseStream();

                var reader = new StreamReader(webStream);
                var data = reader.ReadToEnd();
                return data;

                }
            catch (Exception e) { return e.Message; }

            }
        }
    }
