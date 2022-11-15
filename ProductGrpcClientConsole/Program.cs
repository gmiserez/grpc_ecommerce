using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private const string GrpcApi = "https://localhost:7151";

    private static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress(GrpcApi);
        var client = new ProductGrpc.Protos.ProductProtoService.ProductProtoServiceClient(channel);

        var services = new ServiceCollection();
        services.AddGrpcClient<ProductGrpc.Protos.ProductProtoService.ProductProtoServiceClient>(opt => opt.Address = new Uri(GrpcApi));
        var provider = services.BuildServiceProvider();
        var client2 = provider.GetRequiredService<ProductGrpc.Protos.ProductProtoService.ProductProtoServiceClient>();

        /* Get inexistent product */
        try
        {
            var notFoundProduct = await client.GetProductAsync(new ProductGrpc.Protos.GetProductRequest { ProductId = 51515 });
        }
        catch(RpcException ex)
        {
            Console.WriteLine(ex.Message);
        }

        /* Read Product stream */
        using var reply = client.GetAllProducts(new ProductGrpc.Protos.GetAllProductRequest());
        await foreach(ProductGrpc.Protos.ProductModel responseData in reply.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(responseData);
        }

        /* Add Product */
        //await client.AddProductAsync(
        //    new ProductGrpc.Protos.AddProductRequest
        //    {
        //        Product = new ProductGrpc.Protos.ProductModel
        //        {
        //            ProductId = 4,
        //            Name = "Qux",
        //            Status = ProductGrpc.Protos.ProductStatus.Low
        //        }
        //    });

        /* Insert Bulk */
        using var clientBulk = client2.InsertBulkProduct();
        for (var i=0; i<3; i++)
        {
            var product = new ProductGrpc.Protos.ProductModel
            {
                ProductId = i + 10,
                Name = $"Qux{i}",
                Status = ProductGrpc.Protos.ProductStatus.Low
            };

            await clientBulk.RequestStream.WriteAsync(product);
        }
        await clientBulk.RequestStream.CompleteAsync();

        var responseBulk = await clientBulk;
        Console.WriteLine(responseBulk.InsertCount);

        Console.ReadLine();
    }
}