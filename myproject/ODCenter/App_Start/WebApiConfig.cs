using ODCenter.Base;
using PTR.Logging;
using PTR.Web.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace ODCenter
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new ErrorFilterAttribute());
        }
    }

    public class ErrorFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            try
            {
                Exception err = context.Exception;
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("事件类型: {0}\r\n", err.GetType().FullName);
                msg.AppendFormat("请求Url: {0}\r\n", context.Request.RequestUri);
                msg.AppendFormat("客户端IP: {0}\r\n", context.Request.GetUserHostAddress());
                msg.AppendFormat("事件时间: {0}\r\n", DateTime.Now);
                msg.AppendFormat("事件类型: {0}\r\n", err.GetType());
                if (err.GetType() == typeof(HttpException))
                {
                    msg.AppendFormat("错误代码: {0}\r\n", ((HttpException)err).ErrorCode);
                    msg.AppendFormat("HTTP编码: {0}\r\n", ((HttpException)err).GetHttpCode());
                    msg.AppendFormat("事件代码: {0}\r\n", ((HttpException)err).WebEventCode);
                }
                msg.AppendFormat("事件消息: {0}\r\n", err.Message);
                msg.AppendFormat("事件堆栈: {0}\r\n", err.StackTrace);
                Logger.LogError(msg.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogError("Error occurred when processing application error.", ex);
            }
        }
    }

    public class RequireSslAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    status=(Int32)HttpStatusCode.Forbidden,
                    time = DateTime.Now,
                    errors = "SSL Required"
                });
            }
            else
            {
                base.OnActionExecuting(actionContext);
            }
        }
    }
}
