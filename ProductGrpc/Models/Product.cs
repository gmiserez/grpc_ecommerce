namespace ProductGrpc.Models
{
    public class Product
    {
        public Product(
            int productId, 
            string name, 
            ProductStatus status)
        {
            ProductId = productId;
            Name = name;
            Status = status;
        }

        public int ProductId { get; }
        public string Name { get; }
        public ProductStatus Status { get; }
    }

    public enum ProductStatus
    {
        InStock = 0,
        Low = 1,
        None = 2
    }
}
