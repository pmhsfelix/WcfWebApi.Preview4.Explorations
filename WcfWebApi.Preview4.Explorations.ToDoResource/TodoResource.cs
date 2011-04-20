using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.ServiceModel.Web;

namespace WcfWebApi.Preview4.Explorations.First
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    class TodoResource
    {
        private readonly IToDoRepository _repository;

        public TodoResource(IToDoRepository repository)
        {
            _repository = repository;
        }

        [WebGet(UriTemplate = "")]
        HttpResponseMessage GetAllToDos()
        {
            return new HttpResponseMessage()
                       {
                           StatusCode = HttpStatusCode.OK,
                           Content =
                               new StringContent(
                               String.Concat(_repository.All.Select(t => t.ToString())))
                       };
        }

        [WebGet(UriTemplate = "{id}")]
        HttpResponseMessage GetToDo(int id)
        {
            ToDo td = _repository.All.Where(t => t.Id == id).FirstOrDefault();
            if (td == null)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content =
                    new StringContent(td.ToString())
            };
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        HttpResponseMessage DeleteToDo(int id)
        {
            ToDo td = _repository.All.Where(t => t.Id == id).FirstOrDefault();
            if (td == null) 
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            _repository.Remove(td);
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NoContent
            };
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        HttpResponseMessage PostNewToDo(HttpRequestMessage req)
        {
            var body = req.Content.ReadAsString();
            dynamic formContent = FormUrlEncodedExtensions.ParseFormUrlEncoded(body);
            string description = formContent.Description;
            if (description == null)
            {
                return new HttpResponseMessage()
                           {
                               StatusCode = HttpStatusCode.BadRequest
                           };
            }
            var td = new ToDo(description);
            _repository.Add(td);
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.Created;
            string uriString = req.RequestUri.AbsoluteUri + "/" + td.Id.ToString();
            Uri uri = new Uri(uriString);
            response.Headers.Location = uri;
            response.Content = new StringContent(uri.AbsoluteUri, Encoding.UTF8, "text/uri-list");
            return response;
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        HttpResponseMessage UpdateToDo(int id, HttpRequestMessage req)
        {
            ToDo td = _repository.All.Where(t => t.Id == id).FirstOrDefault();
            if (td == null) 
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            var body = req.Content.ReadAsString();
            dynamic formContent = FormUrlEncodedExtensions.ParseFormUrlEncoded(body);
            string description = formContent.Description;
            if (description == null)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            td.Description = description;
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content =
                    new StringContent(td.ToString())
            };
        }
    }
}
