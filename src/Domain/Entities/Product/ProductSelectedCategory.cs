using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities.Product
{
    public class ProductSelectedCategory : BaseEntity
    {
        public long ProductId { get; set; }
        public long CategoryId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
        public ProductCategory Category { get; set; }
    }
}
