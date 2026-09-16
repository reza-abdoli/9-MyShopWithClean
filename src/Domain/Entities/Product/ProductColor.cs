using Domain.Entities;

namespace Domain.Entities.Product
{
    public class ProductColor : BaseEntity
    {
        public string Title { get; set; }
        public string ColorCode { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
