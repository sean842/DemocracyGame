using Microsoft.AspNetCore.ResponseCompression;
using TriangleDbRepository;
using NewBlazorProjecct.Server.Hubs;
using NewBlazorProjecct.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<DbRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
       new[] { "application/octet-stream" });
});


var app = builder.Build();

app.UseResponseCompression();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();

// חיבור לHub שיצרתם 
app.MapHub<ChatHub>("/chathub");

app.MapFallbackToFile("index.html");

app.Run();
