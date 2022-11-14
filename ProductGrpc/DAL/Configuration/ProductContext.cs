using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Extensions.Context;

namespace ProductGrpc.DAL.Configuration
{
    public class ProductContext : MongoDbContext, IProductContext
    {
        public ProductContext(MongoOptions mongoOptions)
            : base(mongoOptions)
        {
        }

        protected override void OnConfiguring(IMongoDatabaseBuilder mongoDatabaseBuilder)
        {
            mongoDatabaseBuilder
                .ConfigureCollection(new ProductConfiguration());
        }
    }

    public interface IProductContext : IMongoDbContext
    {
    }
}
