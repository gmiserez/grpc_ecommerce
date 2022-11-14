using Grpc.Core;
using ProductGrpc.DAL;
using ProductGrpc.Models;
using ProductGrpc.Protos;
using System.ComponentModel.DataAnnotations;
using static ProductGrpc.Protos.ProductProtoService;

namespace ProductGrpc.Services
{
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
                return null;
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
    }
}
