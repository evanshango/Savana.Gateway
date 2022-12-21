using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOcelot(builder.Configuration).AddPolly();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(opt => {
    opt.AddPolicy(name: "CorsPolicy",
        configurePolicy: policy => {
            policy.WithOrigins(builder.Configuration["Origins:Urls"]).AllowAnyHeader().AllowAnyMethod();
        }
    );
});
builder.Services.Configure<KestrelServerOptions>(o => { o.Limits.MaxRequestBodySize = int.MaxValue; });
builder.Services.Configure<FormOptions>(x => {
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
    x.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseSwaggerForOcelotUI(opt => {
    opt.DownstreamSwaggerEndPointBasePath = "/swagger/docs";
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

app.UseCors("CorsPolicy");
app.MapGet(
    "/", async ctx => await ctx.Response.WriteAsync("SAVANA TREASURES API GATEWAY")
);

await app.UseOcelot();

app.Run();