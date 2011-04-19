using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.ApplicationServer.Http;

namespace WcfWebApi.Preview4.Explorations.First
{
    class TheHostProgram
    {
        static void Main(string[] args)
        {
            //using (var host = new HttpServiceHost(typeof(TheServiceClass), "http://localhost:8080/first"))
            //using (var host = new HttpServiceHost(typeof(TimerResource), "http://localhost:8080/first"))
            //using (var host = new HttpServiceHost(typeof(ProcessResource), "http://localhost:8080/procs"))
            using (var host = new HttpServiceHost(new TodoResource (new ToDoMemoryRepository()), "http://localhost:8080/todo"))
            {
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
