using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

namespace System.HttpProxy
{
    /// <summary>
    /// Json动作转换器
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public class JsonResult : ActionResult
    {
        private object _value;
        private JsonSetting setting;
        public JsonResult(object value)
        {
            _value = value;
            this.ContentType = "application/json";
        }

        public JsonResult(object value, JsonSetting setting = null) : this(value)
        {            
            this.setting = setting;
            this.Encoding = setting.Encoding;
        }

      
        public override string ToResponse()
        {
            if (_value != null)
            {
                if (this.setting == null)
                {
                    this.setting = new JsonSetting();
                    setting.Setting = new JsonSerializerSettings();
                    setting.Setting.NullValueHandling = NullValueHandling.Ignore;
                    setting.Setting.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                    //if(this.format_flag)
                    //{
                    //    setting.Formatting = Formatting.Indented;
                    //}                
                    //Body必需是JSON格式  
                }
                return JsonConvert.SerializeObject(_value, setting.Setting);
            }
            return string.Empty;
        }

    }

    public class JsonSetting
    {
        public Encoding Encoding { get; set; }
        public JsonSerializerSettings Setting { get; set; }
    }
}