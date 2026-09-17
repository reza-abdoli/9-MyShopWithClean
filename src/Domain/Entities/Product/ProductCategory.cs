using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities.Product
{
    public class ProductCategory : BaseEntity
    {
        public long? ParentId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string MainImage { get; set; }
        public bool IsActive { get; set; }
        public bool ShowInHome { get; set; }
        public int Order { get; set; }

        [JsonIgnore]
        public ProductCategory Parent { get; set; }   // Parent <-> SubCategories چرخه می‌سازه

        public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        [JsonIgnore]
        public ICollection<ProductSelectedCategory> ProductSelectedCategories { get; set; }
        // این هم چرخه می‌سازه: Category -> ProductSelectedCategories -> Product -> SelectedCategories -> Category...
    }
}