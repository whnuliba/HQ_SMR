
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace IDS.SSO.Client
{
    public class HttpUtil
    {
        public static Dictionary<string, string> COOKIE_CACHE = new();       
        public static string Post(string servicePath, string postData, Dictionary<string, string> headers = null, int timeOut=6000)
        {
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(servicePath);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version11;
            request.ContentLength = data.Length;
            request.Timeout = timeOut;
            if (COOKIE_CACHE.TryGetValue("Cookies", out string value)){
                if (!string.IsNullOrWhiteSpace(value) && headers == null)
                    headers = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(value))
                    headers.Add("Set-Cookie", value);
            }
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            Stream newStream = request.GetRequestStream();

            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string cks =response.Headers["Set-Cookie"];
            if (!string.IsNullOrWhiteSpace(cks)) {
                if (COOKIE_CACHE.ContainsKey("Cookies"))
                    COOKIE_CACHE["Cookies"] = cks;
                else
                    COOKIE_CACHE.Add("Cookies", cks);
            }          
            StreamReader reader = new(response.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            request.Abort();
            response.Close();
            return content;
        }


        public static string Post(string servicePath, string postData,string ticket, Dictionary<string, string> headers = null, int timeOut = 6000)
        {
            string content = String.Empty;

            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(servicePath);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ProtocolVersion = HttpVersion.Version11;
            request.ContentLength = data.Length;
            request.Timeout = timeOut;
            if (!string.IsNullOrWhiteSpace(ticket) && headers == null)
                headers = new Dictionary<string, string>();
            if(!string.IsNullOrWhiteSpace(ticket))
                headers.Add("Authorization", $"Parameter {ticket}");
           
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            Stream newStream = request.GetRequestStream();

            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
           // CookieCollection cc = response.Cookies;
            StreamReader reader = new (response.GetResponseStream(), Encoding.UTF8);
            content = reader.ReadToEnd();

            request.Abort();

            response.Close();

            return content;
        }

        public static string Get(string servicePath)
        {
            HttpWebRequest request =(HttpWebRequest)WebRequest.Create(servicePath);

            request.Method = "Get";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new (response.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            request.Abort();

            response.Close();

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cks">cookie字符串</param>
        /// <returns></returns>
        public static string GetTicket(string cks,string ticketKey="ticket") {
            //if (!string.IsNullOrWhiteSpace(cks)) {
            //    string[] cookies = cks.Split(';');
            //    foreach (string c in cookies) { 

            //    }
            //}
            string ticket = String.Empty;
            if (String.IsNullOrEmpty(ticket))//尝试从Cookie里提取
            {
                string str = cks;
                if (!string.IsNullOrWhiteSpace(str))
                {
                        if (str.IndexOf(ticketKey + "=") > -1)
                        {
                            string parseStr = str + ';';
                            //有多个Coookie项
                            string s = ticketKey + "=";
                            string e = ";";

                            Regex rg = new ("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                            ticket = rg.Match(parseStr).Value;
                        if (!String.IsNullOrEmpty(ticket))
                            return ticket;
                        }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据HttpHeader里信息获取AccessToken
        /// </summary>
        /// <param name="requestHeader"></param>
        /// <returns></returns>
        public static string GetCookieTicket(HttpRequestHeaders requestHeader, string ticketKey)
        {
            string ticket = String.Empty;

            var authorization = requestHeader.Authorization;
            if ((authorization != null) && (authorization.Parameter != null))
            {
                //解密用户ticket,并校验用户名密码是否匹配
                ticket = authorization.Parameter;
            }

            if (String.IsNullOrEmpty(ticket))//尝试从Cookie里提取
            {
                string headerStr = requestHeader.ToString();
                int cookieIndex = headerStr.IndexOf("Cookie:");
                if (cookieIndex > -1)
                {
                    List<string> strList = headerStr.Substring(cookieIndex).Split('\r').ToList();
                    foreach (string str in strList)
                    {
                        if (str.IndexOf(ticketKey + "=") > -1)
                        {
                            string parseStr = str + ';';
                            //有多个Coookie项
                            string s = ticketKey + "=";
                            string e = ";";

                            Regex rg = new("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                            ticket = rg.Match(parseStr).Value;
                            if (!String.IsNullOrEmpty(ticket))
                                break;
                        }
                    }
                }
            }

            if (ticket.ToLower() == "null")
                ticket = String.Empty;

            return ticket;
        }

        /// <summary>
        /// 获取HttpHeader里的指定Key键数据
        /// </summary>
        /// <param name="requestHeader"></param>
        /// <param name="headerKey"></param>
        /// <returns></returns>
        public static string GetHeaderValue(HttpRequestHeaders requestHeader, string headerKey)
        {
            string result = String.Empty;
            string headerStr = requestHeader.ToString() + "\r\n";
            int index = headerStr.IndexOf(headerKey + ":");
            if (index > -1)
            {
                string s = headerKey + ":";
                string e = "\r\n";

                Regex rg = new ("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                result = rg.Match(headerStr).Value;
                if (!String.IsNullOrEmpty(result))
                    result = result.Trim();
            }

            return result;
        }

    }
}
