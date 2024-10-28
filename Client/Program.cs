using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NewBlazorProjecct.Client;
using NewBlazorProjecct.Shared.Services;
using Blazorise;
using Blazorise.QRCode;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

object value = builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddScoped<UserService>()
    .AddBlazorise(options => options.Immediate = true) // Configures Blazorise
    .AddBootstrapProviders() // Adds Bootstrap styling provider
    .AddFontAwesomeIcons();  // Adds FontAwesome icons


//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//// Register UserService
//builder.Services.AddScoped<UserService>();

await builder.Build().RunAsync();
