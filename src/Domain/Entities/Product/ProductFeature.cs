using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities.Product
{
    public class ProductFeature : BaseEntity
    {
        public long ProductId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
