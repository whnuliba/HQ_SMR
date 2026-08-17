
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace IDS.Common.Utils
{
    public class HttpUtil
    {
        public static Dictionary<string, string> COOKIE_CACHE = new();       
        public static async Task<string> Post(string servicePath, string postData, Dictionary<string, string> headers = null, int timeOut=6000)
        {
            using (var hettpClient = new HttpClient()) {
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");
               // JsonContent json = new JsonContent(postData);
                HttpContent httpContent = new StringContent(postData, Encoding.UTF8, mediaType);
                hettpClient.Timeout = TimeSpan.FromMilliseconds(timeOut);
                if (COOKIE_CACHE.TryGetValue("Cookies", out string value))
                {
                    if (!string.IsNullOrWhiteSpace(value) && headers == null)
                        headers = new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(value))
                        headers.Add("Set-Cookie", value);
                }
                if (headers != null && headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        if (header.Key.ToLower() == "authorization") {
                            hettpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", header.Value);
                            continue;
                        } 
                        httpContent.Headers.Add(header.Key, header.Value);
                    }
                }
                //

                var res = await  hettpClient.PostAsync(servicePath, httpContent);

                if (res.IsSuccessStatusCode) {
                    var body = res.Content;


                    if (body.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? values)) {
                        string cks = values.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(cks))
                        {
                            if (COOKIE_CACHE.ContainsKey("Cookies"))
                                COOKIE_CACHE["Cookies"] = cks;
                            else
                                COOKIE_CACHE.Add("Cookies", cks);
                        }

                    }
  
                    return await  body.ReadAsStringAsync();
                }
                if (res!=null && (int)res.StatusCode != StatusCodes.Status200OK) {
                    var body = res.Content;
                    if (body != null)
                    {
                        string msg = await body.ReadAsStringAsync();
                        if(!string.IsNullOrEmpty(msg))
                           throw new Exception(msg);
                    }
                }
            }
            return null;
        }


        public async Task<string> Post(string servicePath, string postData,string authorization, Dictionary<string, string> headers = null, int timeOut = 6000)
        {
            using (var hettpClient = new HttpClient())
            {
                hettpClient.Timeout = TimeSpan.FromMilliseconds(timeOut);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");
                // JsonContent json = new JsonContent(postData);
              
                HttpContent httpContent = new StringContent(postData, Encoding.UTF8, mediaType);
                if (COOKIE_CACHE.TryGetValue("Cookies", out string value))
                {
                    if (!string.IsNullOrWhiteSpace(value) && headers == null)
                        headers = new Dictionary<string, string>();
                    if (!string.IsNullOrWhiteSpace(value))
                        headers.Add("Set-Cookie", value);
                }
                if (!string.IsNullOrEmpty(authorization)) {
                    hettpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
                }

                if (headers != null && headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        if (header.Key.ToLower() == "authorization")
                        {
                            hettpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", header.Value);
                            continue;
                        }
                        httpContent.Headers.Add(header.Key, header.Value);
                    }
                }
                var res = await hettpClient.PostAsync(servicePath, httpContent);

                if (res.IsSuccessStatusCode)
                {
                    var body = res.Content;


                    if (body.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? values))
                    {

                        string cks = values.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(cks))
                        {
                            if (COOKIE_CACHE.ContainsKey("Cookies"))
                                COOKIE_CACHE["Cookies"] = cks;
                            else
                                COOKIE_CACHE.Add("Cookies", cks);
                        }
                    }
 
                    return await body.ReadAsStringAsync();
                }
                if (res != null && (int)res.StatusCode != StatusCodes.Status200OK)
                {
                    var body = res.Content;
                    if (body != null)
                    {
                        string msg = await body.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(msg))
                            throw new Exception(msg);
                    }
                }
            }
            return null;

        }

        public static async Task<string> Get(string servicePath, Dictionary<string, string> headers)
        {


            HttpClientHandler handler = new HttpClientHandler() { UseCookies = false };
            using (var hettpClient = new HttpClient(handler))
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, servicePath);
                //v=A04waYNMpYJCORGmsp8M3YQvny8VzxLJJJPGrXiXutEM2-CR4F9i2fQjFrxL

                if (headers.Count > 0)
                {
                    foreach (var item in headers)
                    {
                        requestMessage.Headers.Add(item.Key, item.Value);
                    }
                }
                var res = await hettpClient.SendAsync(requestMessage);
                if (res.IsSuccessStatusCode)
                {
                    var body = res.Content;


                    if (body.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? values))
                    {
                        string cks = values.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(cks))
                        {
                            if (COOKIE_CACHE.ContainsKey("Cookies"))
                                COOKIE_CACHE["Cookies"] = cks;
                            else
                                COOKIE_CACHE.Add("Cookies", cks);
                        }

                    }
                    return await body.ReadAsStringAsync();
                }
                if (res != null && (int)res.StatusCode != StatusCodes.Status200OK)
                {
                    var body = res.Content;
                    if (body != null)
                    {
                        string msg = await body.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(msg))
                            throw new Exception(msg);
                    }
                }
            }
            return null;
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
