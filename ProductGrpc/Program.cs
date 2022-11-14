using MongoDB.Driver;
using MongoDB.Extensions.Context;
using ProductGrpc.DAL;
using ProductGrpc.DAL.Configuration;
using ProductGrpc.Services;

public static partial class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();

        var config = GetConfiguration();

        MongoOptions mongoOptions = GetMongoDbConfiguration(config);

        builder.Services.AddSingleton(mongoOptions)
            .AddSingleton<IProductContext, ProductContext>()
            .AddSingleton<ProductRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<GreeterService>();
        app.MapGrpcService<ProductProtoService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }


    private static IConfiguration GetConfiguration()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder();

        builder.SetBasePath(Directory.GetCurrentDirectory());

        builder.AddJsonFile($"appsettings.json",
                optional: false, reloadOnChange: true);

        return builder.Build();
    }

    private static MongoOptions GetMongoDbConfiguration(
    IConfiguration configuration)
    {
        MongoOptions config = configuration.GetSection("MongoDb")
            .Get<MongoOptions>();

        var errors = new List<string>();
        if (string.IsNullOrEmpty(config.ConnectionString))
        {
            errors.Add(nameof(config.ConnectionString));
        }

        if (string.IsNullOrEmpty(config.DatabaseName))
        {
            errors.Add(nameof(config.DatabaseName));
        }

        if (errors.Any())
        {
            throw new MongoConfigurationException(string.Join(";", errors));
        }

        return config;
    }
}
