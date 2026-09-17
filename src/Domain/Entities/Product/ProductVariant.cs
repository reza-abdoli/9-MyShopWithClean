using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities.Product
{
    public class ProductVariant : BaseEntity
    {
        public long ProductId { get; set; }
        public long ColorId { get; set; }
        public long SizeId { get; set; }
        public int StockCount { get; set; }
        public string ColorTitle { get; set; }
        public string ColorCode { get; set; }
        public string SizeTitle { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
