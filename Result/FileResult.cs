using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text;

namespace System.HttpProxy
{
    /// <summary>
    /// 文件转换器
    /// 创建人：YTS
    /// 创建时间：2023-4-4
    /// </summary>
    public class FileResult : ActionResult
    {
        private object _value;

        public FileResult(byte[] buffer, string file_name, string file_ext)
        {            
            this.ContentType = MimeMapping.GetMimeMapping("." + file_ext);
        }

      
        public override string ToResponse()
        {
            
            return string.Empty;
        }

    }

}