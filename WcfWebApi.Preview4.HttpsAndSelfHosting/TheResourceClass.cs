using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfWebApi.Preview4.HttpsAndSelfHosting
{
    [ServiceContract]
    class TheResourceClass
    {
        [WebGet(UriTemplate = "")]
        HttpResponseMessage GetGreetings(IPrincipal principal)
        {
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("hello "+principal.Identity.Name)
            };
        }
    }
}
