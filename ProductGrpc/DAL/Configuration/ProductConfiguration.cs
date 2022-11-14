using MongoDB.Extensions.Context;
using ProductGrpc.Models;

namespace ProductGrpc.DAL.Configuration
{
    public class ProductConfiguration : IMongoCollectionConfiguration<Product>
    {
        public void OnConfiguring(
            IMongoCollectionBuilder<Product> mongoCollectionBuilder)
        {
            mongoCollectionBuilder
                .AddBsonClassMap<Product>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdField(c => c.ProductId);
                    cm.SetIgnoreExtraElements(true);
                });
        }
    }
}
