using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace WcfWebApi.Preview4.Explorations.First
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    class TimerResource
    {
        private readonly IDictionary<int,DateTime> _starts = new Dictionary<int,DateTime>();
        private int _id = 0;

        [WebGet(UriTemplate = "")]
        HttpResponseMessage GetAll(HttpRequestMessage req)
        {
            var feed = new SyndicationFeed("Timers", "Timers", null,
                                    _starts.Select(kv => new SyndicationItem(
                                        kv.Key.ToString(), kv.Value.ToString(), 
                                        new Uri(String.Format("{0}/{1}", req.RequestUri.AbsoluteUri, kv.Key)))));

            var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CloseOutput = false }))
            {
                new Atom10FeedFormatter(feed).WriteTo(writer);
            }
            stream.Seek(0, SeekOrigin.Begin);

            var resp = new HttpResponseMessage()
            {
                Content = new StreamContent(stream)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
            return resp;
        }

        [WebInvoke(Method = "POST", UriTemplate="")]
        HttpResponseMessage Post(HttpRequestMessage req)
        {
            int ix = _id++;
            _starts.Add(ix,DateTime.Now);
            var resp = new HttpResponseMessage()
                           {
                               StatusCode = HttpStatusCode.Created
                           };
            resp.Headers.Location = new Uri(String.Format("{0}/{1}",req.RequestUri.AbsoluteUri,ix));
            return resp;
        }

        [WebGet(UriTemplate = "{ix}")]
        HttpResponseMessage Get(int ix)
        {
            DateTime start;
            if(!_starts.TryGetValue(ix, out start))
            {
                return new HttpResponseMessage()
                           {
                               StatusCode = HttpStatusCode.NotFound
                           };
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(DateTime.Now.Subtract(start).ToString()),
                StatusCode = HttpStatusCode.OK
            };
        }

        [WebInvoke(Method="DELETE", UriTemplate = "{ix}")]
        HttpResponseMessage Delete(int ix)
        {
            if(!_starts.Remove(ix))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NoContent
            };
        }

    }
}
