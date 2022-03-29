using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace System.HttpProxy
{   
    /// <summary>
    /// API路由
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public class ApiRoute : RouteBase
    {
        public Type Controller { get; set; }
        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Method { get; set; }              

        public override void Router(HttpRequest request)
        {
            var contoller = (ApiController)Activator.CreateInstance(Controller);
            contoller.Request = request;
            if (RouteCompletedEvent!=null)
            {
                RouteCompletedEvent(contoller);
            }
            contoller.Context = new ActionContext
            {
                Method = Method
            };
            
            ParameterInfo[] paramsInfo = Method.GetParameters();
            contoller.Context.Values = new object[paramsInfo.Length];
            
            bool fromBody = false;              
            foreach (var paramInfo in paramsInfo)
            {                
                var attrs = paramInfo.GetCustomAttributes(true);
                foreach (Attribute attr in attrs)
                {
                    if (attr is FromBodyAttribute)
                    {
                        fromBody = true;
                        break;
                    }
                }
                if (fromBody)
                {                   
                    JsonSerializerSettings setting = new JsonSerializerSettings();
                    setting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
                    setting.NullValueHandling = NullValueHandling.Ignore;
                    //Body必需是JSON格式
                    contoller.Context.Values[0] = JsonConvert.DeserializeObject(request.Body, paramInfo.ParameterType, setting);
                    break;
                }
            }
            if (!fromBody)
            {
                int index = request.URL.IndexOf("?");
                if (index <= 0)
                {
                    return;
                }
                string param = request.URL.Substring(index + 1, request.URL.Length - index - 1);
                string[] array = param.Split('&');
                
                var pairs = new Dictionary<string, string>();
                foreach (var item in array)
                {
                    string[] keyValue = item.Split('=');
                    if (keyValue.Length == 2)
                    {                        
                        pairs.Add(keyValue[0].ToLower(), HttpUtility.UrlDecode(keyValue[1]));
                    }                    
                }
                for (int i = 0; i < paramsInfo.Length; i++)
                {
                    if (pairs.ContainsKey(paramsInfo[i].Name.ToLower()))
                    {
                        try
                        {
                            contoller.Context.Values[i] = Convert.ChangeType(pairs[paramsInfo[i].Name.ToLower()], paramsInfo[i].ParameterType);
                        }
                        catch
                        {
                            contoller.Context.Values[i] = null;
                        }
                    }
                    else
                    {
                        contoller.Context.Values[i] = null;
                    }
                }             
            }
        }

        public static void Load(Type[] list)
        {             
            string url = string.Empty;
            foreach (var item in list)
            {
                var attrs = item.GetCustomAttributes(false);
                foreach (Attribute classAttr in item.GetCustomAttributes(true))
                {
                    if (classAttr is RouteAttribute)
                    {
                        var classTemplate = ((RouteAttribute)classAttr).Template;
                        classTemplate = "/" + classTemplate;
                        var methods = item.GetMethods();
                        foreach (var method in methods)
                        {
                            foreach (Attribute attr in method.GetCustomAttributes(true))
                            {
                                if (attr is RouteAttribute)
                                {
                                    url = ((RouteAttribute)attr).Template;
                                    if (classTemplate.Length > 0)
                                    {
                                        url = classTemplate + "/" + url;
                                    }
                                    RouteTable.Routes.Add(new ApiRoute
                                    {
                                        Url = url.ToLower(),
                                        Controller = item,
                                        Method = method
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}