// Application/Features/User/Handlers/Queries/GetEditUserDetailQueryHandler.cs
using System.Threading.Tasks;
using Application.DTOs.Account;
using Application.Features.Users.Requests.Queries;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;

namespace Application.Features.Users.Handlers.Queries;

public class GetEditUserDetailQueryHandler
{
    private readonly IRepository<User> _userRepository;

    public GetEditUserDetailQueryHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<EditUserInfoDTO> Handle(GetEditUserDetailQuery query)
    {
        var user = await _userRepository.GetEntityById(query.UserId);
        return new EditUserInfoDTO
        {
            UserId = query.UserId,
            MobileNumber = user.MobileNumber,
            FullName = user.FullName,
            Email = user.Email,
            Address = user.Address,
            PostCode = user.PostCode,
            UserCity = user.UserCity
        };
    }
}