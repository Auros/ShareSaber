using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShareSaber.Models;
using Blazor.FileReader;

namespace ShareSaber
{
    public class Program
    {
        public static string APIURL = "https://sharesaber.com/";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(new State());

            builder.Services.AddFileReaderService(options => options.UseWasmSharedBuffer = true);

            await builder.Build().RunAsync();
        }
    }
}
