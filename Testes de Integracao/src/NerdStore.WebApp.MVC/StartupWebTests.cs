using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using NerdStore.WebApp.MVC.Data;
using NerdStore.Catalogo.Application.AutoMapper;
using NerdStore.Catalogo.Data;
using NerdStore.Vendas.Data;
using NerdStore.WebApp.MVC.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

public class StartupWebTests
{
    public IConfiguration Configuration { get; }

    public StartupWebTests(IWebHostEnvironment hostingEnvironment)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(hostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    // Configuração de serviços (ConfigureServices)
    public void ConfigureServices(IServiceCollection services)
    {
        if (services.BuildServiceProvider().GetService<IWebHostEnvironment>().IsEnvironment("Local"))
        {
            StaticWebAssetsLoader.UseStaticWebAssets(services.BuildServiceProvider().GetService<IWebHostEnvironment>(), Configuration);
        }

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(Configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<CatalogoContext>(options =>
            options.UseInMemoryDatabase(Configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<VendasContext>(options =>
            options.UseInMemoryDatabase(Configuration.GetConnectionString("DefaultConnection")));

        services.AddMvc(options =>
        {
            options.EnableEndpointRouting = false;
        });

        services.AddControllersWithViews().AddRazorRuntimeCompilation();
        services.AddHttpContextAccessor();


        services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.RegisterServices();
    }

    // Configuração do pipeline HTTP (Configure)
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "areas",
                template: "{area:exists}/{controller=Vitrine}/{action=Index}/{id?}");

            routes.MapRoute(
                name: "default",
                template: "{controller=Vitrine}/{action=Index}/{id?}");
        });
    }
}
