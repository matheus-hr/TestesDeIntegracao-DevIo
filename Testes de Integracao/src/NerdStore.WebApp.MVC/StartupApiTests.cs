using System.Collections.Generic;
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
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NerdStore.WebApp.MVC.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;

public class StartupApiTests
{
    public IConfiguration Configuration { get; }

    public StartupApiTests(IWebHostEnvironment hostingEnvironment)
    {
        var builder = new ConfigurationBuilder()
           .SetBasePath(hostingEnvironment.ContentRootPath)
           .AddJsonFile("appsettings.json", true, true)
           .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true)
           .AddEnvironmentVariables();

        Configuration = builder.Build(); ;
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

        var appSettingsSection = Configuration.GetSection(key: "AppSettings");
        services.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {

            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = appSettings.ValidoEm,
                ValidIssuer = appSettings.Emissor
            };
        });

        services.AddControllersWithViews().AddRazorRuntimeCompilation();
        services.AddHttpContextAccessor();

        services.AddSwaggerGen(c =>
        {
            var security = new Dictionary<string, IEnumerable<string>>
            {
                { "Bearer", new string[] { } }
            };

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } }
            });
        });

        services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.RegisterServices(); //Classe de DI
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

        app.UseSwagger();
        app.UseSwaggerUI(s =>
        {
            s.SwaggerEndpoint("/swagger/v1/swagger.json", "desenvolvedor.io API v1.0");
        });
    }
}
