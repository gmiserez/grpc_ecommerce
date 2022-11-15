using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using ProductGrpc.DAL;
using ProductGrpc.Models;
using ProductGrpc.Protos;
using static ProductGrpc.Protos.ProductProtoService;

namespace ProductGrpc.Services
{
    [Authorize]
    public class ProductProtoService: ProductProtoServiceBase
    {
        private readonly ProductRepository _productRepository;

        public ProductProtoService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public override async Task<ProductModel> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            Product? product = await _productRepository.GetByIdAsync(request.ProductId, context.CancellationToken);

            if(product == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "didn't find it"));
            }

            return new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Status = (Protos.ProductStatus)product.Status
            };
        }

        public override async Task GetAllProducts(
            GetAllProductRequest request, 
            IServerStreamWriter<ProductModel> responseStream, 
            ServerCallContext context)
        {
            List<Product> products = await _productRepository.GetAll();

            foreach(var product in products)
            {
                ProductModel model = new ProductModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Status = (Protos.ProductStatus)product.Status
                };

                await responseStream.WriteAsync(model);
            }
        }

        public override async Task<ProductModel> AddProduct(AddProductRequest request, ServerCallContext context)
        {
            await _productRepository.Add(
                new Product(
                    request.Product.ProductId, 
                    request.Product.Name, 
                    (Models.ProductStatus)request.Product.Status));

            return request.Product;
        }

        public override async Task<InsertBulkProductResponse> InsertBulkProduct(
            IAsyncStreamReader<ProductModel> requestStream, ServerCallContext context)
        {
            int ct = 0;
            await foreach(var model in requestStream.ReadAllAsync(context.CancellationToken))
            {
                await _productRepository.Add(
                     new Product(
                    model.ProductId,
                    model.Name,
                    (Models.ProductStatus)model.Status));
                ct++;
            }

            return new InsertBulkProductResponse
            {
                Success = true,
                InsertCount = ct
            };
        }
    }
}
