using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Hubs;
using Web.Models;
using Web.Models.Options;
using Web.Services;

namespace Web;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSignalR();
        services.AddHostedService<WaitToParentProcessExit>();
        services.Configure<StartupOptions>(Configuration.GetSection(nameof(StartupOptions)));
        services.Configure<PreviewServerVersion>(Configuration.GetSection(nameof(PreviewServerVersion)));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseCors(config => config
            .AllowAnyMethod()
            .WithOrigins("http://localhost:8088"));
        app.UseWhen(
            context => !context.Request.Path.StartsWithSegments("/index.html"),// owerride in SceneController
            appBuilder => appBuilder.UseStaticFiles());
        
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<InteropHub>("/interophub");
        });
    }
}
