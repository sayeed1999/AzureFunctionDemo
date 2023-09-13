//using GithubMonitorApp.Middlewares;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Azure.Functions.Worker;

//namespace GithubMonitorApp
//{
//    public static class Program
//    {
//        public static void Main()
//        {
//            var host = new HostBuilder()
//                .ConfigureFunctionsWorkerDefaults(worker =>
//                {
//                    //worker.ConfigureSystemTextJson();
//                    worker.UseMiddleware<AuthorizationMiddleware>();
//                })
//                .ConfigureServices(services =>
//                {
//                    services
//                        .AddHttpClient();
//                        //.RegisterAll()
//                        //.AddCors();
//                })
//                .Build();

//            host.Run();
//        }
//    }
//}
