using Grpc.Core;
using Grpc.Net.Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7151");
        var client = new ProductGrpc.Protos.ProductProtoService.ProductProtoServiceClient(channel);

        using var reply = client.GetAllProducts(new ProductGrpc.Protos.GetAllProductRequest());
        await foreach(ProductGrpc.Protos.ProductModel responseData in reply.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(responseData);
        }
    }
}