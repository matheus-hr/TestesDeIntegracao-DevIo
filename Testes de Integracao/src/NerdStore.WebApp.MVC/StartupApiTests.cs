using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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

public class StartupApiTests
{
    public IConfiguration Configuration { get; }

    public StartupApiTests(IConfiguration configuration)
    {
        Configuration = configuration;
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

        services.AddSwaggerGen(c =>
        {
            var security = new Dictionary<string, IEnumerable<string>>
            {
                { "Bearer", new string[] { } }
            };

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } }
            });

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "desenvolvedor.io API",
                Description = "desenvolvedor.io API",
                TermsOfService = null,
            });
        });

        services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));
        services.AddMediatR(typeof(Program));
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

        app.UseSwagger();
        app.UseSwaggerUI(s =>
        {
            s.SwaggerEndpoint("/swagger/v1/swagger.json", "desenvolvedor.io API v1.0");
        });
    }
}
