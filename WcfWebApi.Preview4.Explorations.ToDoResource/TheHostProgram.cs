using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;

namespace WcfWebApi.Preview4.Explorations.First
{
    class TheHostProgram
    {
        static void Main(string[] args)
        {
            //HostWithHttpServiceHost();
            //HostWithOldHost();
            HostWithHttpsEndpoint();
        }

        static void HostWithHttpServiceHost()
        {
            var instance = new TodoResource(new ToDoMemoryRepository());
            using (var host = new HttpServiceHost(instance, "http://localhost:8080/todo"))
            {
                host.AddServiceEndpoint(typeof(TodoResource), new HttpBinding(), "http://localhost:8080/todo2");
                host.Open();
                ShowEndpointsOf(host);
                WaitForKey();
            }
        }

        static void HostWithHttpsEndpoint()
        {
            var repository = new ToDoMemoryRepository();
            repository.Add(new ToDo("Must learn HTTP better"));
            var instance = new TodoResource(repository);
            using (var host = new HttpServiceHost(instance, "http://localhost:8080/todo"))
            {
                
                host.AddServiceEndpoint(typeof(TodoResource), new HttpBinding(), "http://localhost:8080/todo");

                var binding = new HttpBinding(HttpBindingSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                host.AddServiceEndpoint(typeof(TodoResource), binding, "https://localhost:8435/todo");
                //host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.CurrentUser,StoreName.My,X509FindType.FindBySubjectName, "gaviao");
                host.Open();
                ShowEndpointsOf(host);
                WaitForKey();
            }
        }

        static void HostWithOldHost()
        {
            var repository = new ToDoMemoryRepository();
            repository.Add(new ToDo("Must learn HTTP better"));
            var instance = new TodoResource(repository);
            using (var host = new ServiceHost(instance))
            {
                var ep = host.AddServiceEndpoint(typeof(TodoResource), new HttpBinding(), "http://localhost:8080/todo2");
                ep.Behaviors.Add(new HttpBehavior());
                host.Open();
                ShowEndpointsOf(host);
                WaitForKey();
            }
        }

        private static void ShowEndpointsOf(ServiceHost host)
        {
            Console.WriteLine("Host is opened with the following endpoints:");
            foreach (var ep in host.Description.Endpoints)
            {
                Console.WriteLine("\tEndpoint: address = {0}; binding = {1}", ep.Address, ep.Binding);
            }
        }

        private static void WaitForKey()
        {
            Console.WriteLine("Press any key to stop host...");
            Console.ReadKey();
        }
    }
}
