using MongoDB.Driver;
using ProductGrpc.DAL.Configuration;
using ProductGrpc.Models;

namespace ProductGrpc.DAL
{
    public class ProductRepository
    {
        private readonly IMongoCollection<Product> _collection;

        public ProductRepository(IProductContext context)
        {
            _collection = context.CreateCollection<Product>();
        }

        public async Task<Product> GetByIdAsync(
            int productId,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Product> filter =
                Builders<Product>.Filter.Eq(
                    pc => pc.ProductId, productId);

            Product? product = await
                    (await _collection.FindAsync(
                        filter,
                        cancellationToken: cancellationToken))
                    .SingleOrDefaultAsync(cancellationToken);

            return product;
        }

        public Task<List<Product>> GetAll()
        {
            return _collection.Find(Builders<Product>.Filter.Empty).ToListAsync();
        }
    }
}
