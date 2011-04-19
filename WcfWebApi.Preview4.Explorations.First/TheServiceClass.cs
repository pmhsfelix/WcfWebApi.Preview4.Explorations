using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfWebApi.Preview4.Explorations.First
{
    [ServiceContract]
    class TheServiceClass
    {
        [WebGet(UriTemplate = "local")]
        HttpResponseMessage OperationToHandleGetLocalTime()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(DateTime.Now.ToLongTimeString()),
                StatusCode = HttpStatusCode.OK
            };
        }

        [WebGet(UriTemplate = "date")]
        HttpResponseMessage OperationToHandleGetLocalDateWithCultureAwareness(HttpRequestMessage req)
        {
            var culture = WcfWebApi.Preview4.Explorations.Common.HttpUtils.GetCultureInfoForAcceptedLanguages(req.Headers.AcceptLanguage);
            var content = new StringContent(DateTime.Now.ToString(culture.DateTimeFormat));
            content.Headers.ContentLanguage.Add(culture.TwoLetterISOLanguageName);
            return new HttpResponseMessage()
                       {
                           Content = content,
                           StatusCode = HttpStatusCode.OK
                       };
        }

        [WebInvoke(UriTemplate="*", Method = "TRACE")]
        HttpResponseMessage OperationToHandleTheTraceMethod(HttpRequestMessage req)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("TRACE {0} HTTP/1.1\n",req.RequestUri);
            foreach(var h in req.Headers)
            {
                sb.AppendFormat("{0}: {1}", h.Key, h.Value);
            }
            sb.AppendLine();
            var content = new StringContent(sb.ToString(), Encoding.UTF8, "message/http");
            return new HttpResponseMessage()
                       {
                           Content = content,
                           StatusCode = HttpStatusCode.OK
                       };
        }
    }
}
