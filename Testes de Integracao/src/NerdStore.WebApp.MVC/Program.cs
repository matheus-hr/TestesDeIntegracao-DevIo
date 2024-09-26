using System.Collections.Generic;
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
using NerdStore.Vendas.Application.Commands;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços (ConfigureServices)
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<CatalogoContext>(options =>
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<VendasContext>(options =>
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
        .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
}).AddMvcOptions(options => options.EnableEndpointRouting = false); // Importante para MVC sem Razor Pages

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(c =>
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
        TermsOfService = null,  // Ajuste aqui
    });
});

builder.Services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));

builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(AdicionarItemPedidoCommand).Assembly));

builder.Services.RegisterServices();

var app = builder.Build();

// Configuração do pipeline HTTP (Configure)
if (app.Environment.IsDevelopment())
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

app.Run();
