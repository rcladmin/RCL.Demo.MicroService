#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RCL.Demo.Data;

IConfiguration configuration = null;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
      .ConfigureAppConfiguration(builder =>
      {
          builder.AddJsonFile("local.settings.json", true, true);
          builder.AddEnvironmentVariables();
          configuration = builder.Build();
      })
    .ConfigureServices(services =>
    {
        services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings:Database")));
        services.AddRCLCoreApiAuthorizationServices();
    })
    .Build();

host.Run();
