using Domain.Entities;

namespace Domain.Entities.Product
{
    public class ProductGallery : BaseEntity
    {
        public long ProductId { get; set; }
        public string ImageName { get; set; }
        public int Order { get; set; }
    }
}
