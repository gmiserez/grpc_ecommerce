using Grpc.Core;
using Grpc.Net.Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7151");
        var client = new ProductGrpc.Protos.ProductProtoService.ProductProtoServiceClient(channel);

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
    }
}