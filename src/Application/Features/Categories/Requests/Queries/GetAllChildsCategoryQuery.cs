// Application/Features/Categories/Requests/Queries/GetAllChildsCategoryQuery.cs
namespace Application.Features.Categories.Requests.Queries;

public class GetAllChildsCategoryQuery
{
    public long? ParentId { get; set; }
}