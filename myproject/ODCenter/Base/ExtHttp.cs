using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace ODCenter.Base
{
    public static class ExtHttp
    {
        public static String Description(this Enum obj)
        {
            try
            {
                DescriptionAttribute[] attrs = (DescriptionAttribute[])obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attrs.Length > 0 ? attrs[0].Description : obj.ToString();
            }
            catch { }
            return obj.ToString();
        }

        public static String Query(this HttpRequestMessage request, String key)
        {
            KeyValuePair<String, String> pair = request.GetQueryNameValuePairs().FirstOrDefault(p => String.Compare(p.Key, key, true) == 0);
            return pair.Value;
        }

        public static String Param(this HttpRequestMessage request, String key)
        {
            KeyValuePair<String, String> pair = request.GetQueryNameValuePairs().FirstOrDefault(p => String.Compare(p.Key, key, true) == 0);
            if (pair.Key != null)
            {
                return pair.Value;
            }
            var form = request.Content.ReadAsFormDataAsync().Result;
            if (form == null)
            {
                return null;
            }
            var name = form.AllKeys.FirstOrDefault(n => String.Compare(n, key, true) == 0);
            if (name != null)
            {
                return form.GetValues(name)[0];
            }
            return null;
        }

        public static IHttpActionResult Succeed(this ApiController controller, String message)
        {
            return controller.CreateResult(HttpStatusCode.OK, new
            {
                status = ApiStatusCode.Ok,
                message = message
            });
        }

        public static IHttpActionResult Succeed<T>(this ApiController controller, T data)
        {
            return controller.CreateResult(HttpStatusCode.OK, new
            {
                status = ApiStatusCode.Ok,
                time = DateTime.Now,
                data = data
            });
        }

        public static IHttpActionResult Succeed<T>(this ApiController controller, String message, T data)
        {
            return controller.CreateResult(HttpStatusCode.OK, new
            {
                status = ApiStatusCode.Ok,
                message = message,
                data = data
            });
        }

        public static IHttpActionResult Failed<T>(this ApiController controller, T errors, ApiStatusCode status = ApiStatusCode.Undefined)
        {
            return controller.CreateResult(HttpStatusCode.BadRequest, new
            {
                status = (Int32)status,
                time = DateTime.Now,
                errors = errors
            });
        }

        private static IHttpActionResult CreateResult<T>(this ApiController controller, HttpStatusCode status, T content)
        {
            return new ResponseMessageResult(controller.Request.CreateResponse(status, content));
        }

        public static String RandStr(Int32 len)
        {
            String chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder builder = new StringBuilder();
            Random rand = new Random((Int32)DateTime.Now.Ticks);
            for (int i = 0; i < len; i++)
            {
                builder.Append(chars[rand.Next(chars.Length)]);
            }
            return builder.ToString();
        }
    }

    public enum ApiStatusCode : int
    {
        Ok = 0,
        Undefined = 1,
        Forbidden = 10,
        NotFound = 11,
        DataInvalid = 12,
    }
}