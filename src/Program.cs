global using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartlyDemo.RiotSPA.Domain.Interface;
using SmartlyDemo.RiotSPA.Domain.Model.Tax;
using SmartlyDemo.RiotSPA.Domain.Service;
using VueCliMiddleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSpaStaticFiles(opt => opt.RootPath = "ClientApp/public");
builder.Services.AddFastEndpoints();
builder.Services.Configure<TaxCalculator>(builder.Configuration.GetSection("TaxCalculator"));
builder.Services.AddScoped<ITaxService,TaxService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

app.UseAuthorization();

app.UseFastEndpoints();


app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseVueCli(
            npmScript: (System.Diagnostics.Debugger.IsAttached || app.Environment.IsDevelopment()) ? "start" : null,
            regex: "runtime modules",
            forceKill: true
         );
    }
});

app.Run();
public partial class Program { }