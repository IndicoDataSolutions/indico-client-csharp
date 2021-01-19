using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DocFx
{
    public class Program
    {
        public static void Main(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(c =>
                c
                    .UseWebRoot("_site/")
                    .UseStartup<DocFxStartup>()
                )
            .Start();
    }

    public class DocFxStartup
    {
        public static void Configure(IApplicationBuilder app) => app
            .UseDefaultFiles()
            .UseStaticFiles()
        ;
    }
}
