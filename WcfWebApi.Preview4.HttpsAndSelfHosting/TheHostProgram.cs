using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace WcfWebApi.Preview4.HttpsAndSelfHosting
{
    class TheHostProgram
    {
        static void Main(string[] args)
        {
            //HostWithHttpsAndNoneClientCredential();
            HostWithHttpsAndBasicClientCredential();
        }
        static void HostWithHttpsAndNoneClientCredential()
        {
            using(var host = new HttpServiceHost(typeof(TheResourceClass),new string[0]))
            {
                var binding = new HttpBinding(HttpBindingSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                host.AddServiceEndpoint(typeof (TheResourceClass), binding, "https://localhost:8435/greet");
                host.Open();
                Console.WriteLine("Service is opened, press any key to continue");
                Console.ReadKey();
            }
        }

        static void HostWithHttpsAndBasicClientCredential()
        {
            using (var host = new HttpServiceHost(typeof(TheResourceClass), new string[0]))
            {
                var binding = new HttpBinding(HttpBindingSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                var ep = host.AddServiceEndpoint(typeof(TheResourceClass), binding, "https://localhost:8435/greet");
                ep.Behaviors.Add(new HttpBehavior()
                                     {
                                         OperationHandlerFactory = new MyOperationHandlerFactory()
                                     });
                host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode =
                    UserNamePasswordValidationMode.Custom;
                host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new MyCustomValidator();
                host.Open();
                Console.WriteLine("Service is opened, press any key to continue");
                Console.ReadKey();
            }
        }
    }

    class MyCustomValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if(!Object.Equals(userName,password)) throw new FaultException("Unknown Username or Incorrect Password");
        }
    }

    class PrincipalFromBasicAuthenticationOperationHandler : HttpOperationHandler<HttpRequestMessage,IPrincipal>
    {
        public PrincipalFromBasicAuthenticationOperationHandler() : base("Principal")
        {
        }

        public override IPrincipal OnHandle(HttpRequestMessage input)
        {
            Console.WriteLine("PrincipalFromBasicAuthenticationOperationHandler.OnHandle");
            if (input.Headers.Authorization == null || input.Headers.Authorization.Scheme != "Basic")
            {
                // If properly configured, this should never happen:
                // this OperationHandler should only be used when 
                // Basic authorization is required
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            var encoded = input.Headers.Authorization.Parameter;
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var userPass = encoding.GetString(Convert.FromBase64String(encoded));
            int sep = userPass.IndexOf(':');
            var username = userPass.Substring(0, sep);
            var identity = new GenericIdentity(username, "Basic");
            return new GenericPrincipal(identity, new string[] { });
        }
    }

    class MyOperationHandlerFactory : HttpOperationHandlerFactory
    {
        protected override Collection<HttpOperationHandler> OnCreateRequestHandlers(ServiceEndpoint endpoint, HttpOperationDescription operation)
        {
            Console.WriteLine("Adding PrincipalFromBasicAuthenticationOperationHandler");
            var coll = base.OnCreateRequestHandlers(endpoint, operation);
            if (operation.InputParameters.Any(p => p.Type.Equals(typeof(IPrincipal))))
            {
                var binding = endpoint.Binding as HttpBinding;
                if (binding != null && binding.Security.Transport.ClientCredentialType == HttpClientCredentialType.Basic)
                {
                    coll.Add(new PrincipalFromBasicAuthenticationOperationHandler());
                }
            }
            return coll;
        }
    }
}
