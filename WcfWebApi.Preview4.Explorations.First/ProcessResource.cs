using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ServiceModel.Web;

namespace WcfWebApi.Preview4.Explorations.First
{
    [ServiceContract]
    class ProcessResource
    {
        [WebGet(UriTemplate = "")]
        HttpResponseMessage GetAll()
        {
            var list = String.Concat(
                Process.GetProcesses().Select(p => String.Format("{0}:{1}\n", p.Id, p.ProcessName)));
            return new HttpResponseMessage()
                       {
                           Content = new StringContent(list)
                       };
        }

        [WebGet(UriTemplate = "{id}")]
        HttpResponseMessage Get(int id)
        {
            var proc = Process.GetProcesses().Where(p => p.Id == id).FirstOrDefault();
            if (proc == null)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            return new HttpResponseMessage()
                {
                    Content = new StringContent(
                        String.Format("{0}:{1}:{2}", proc.Id, proc.ProcessName, proc.MainModule.FileName))
                };
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        HttpResponseMessage Delete(int id)
        {
            var resp = new HttpResponseMessage();
            var proc = Process.GetProcesses().Where(p => p.Id == id).FirstOrDefault();
            if (proc == null)
            {
                resp.StatusCode = HttpStatusCode.NotFound;
                return resp;
            }
            try
            {
                proc.Kill();
                resp.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Win32Exception)
            {
                return DeleteIsNotPossible();
            }
            catch (NotSupportedException e)
            {
                return DeleteIsNotPossible();
            }
            catch (InvalidOperationException e)
            {
                resp.StatusCode = HttpStatusCode.NotFound;
            }
            return resp;
        }

        private static HttpResponseMessage DeleteIsNotPossible()
        {
            var resp = new HttpResponseMessage()
                           {
                               StatusCode = HttpStatusCode.MethodNotAllowed
                           };
            resp.Content = new StringContent("");
            resp.Content.Headers.Allow.Add("GET");
            return resp;
        }

        [WebInvoke(UriTemplate = "",Method = "POST")]
        HttpResponseMessage Post(HttpRequestMessage req)
        {
            var body = req.Content.ReadAsString();
            dynamic formContent = FormUrlEncodedExtensions.ParseFormUrlEncoded(body);
            string imageName = formContent.Image;
            try
            {
                var proc = Process.Start(imageName);
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.Created;
                string uriString = req.RequestUri.AbsoluteUri + "/" + proc.Id.ToString();
                Console.WriteLine(uriString);
                Uri uri = new Uri(uriString);
                response.Headers.Location = uri;
                response.Content = new StringContent(uri.AbsoluteUri, Encoding.UTF8, "text/uri-list");
                return response;
            }
            catch (Win32Exception e)
            {
                return new HttpResponseMessage()
                           {
                               StatusCode = HttpStatusCode.BadRequest
                           };
            }
            
        }
    }
}
