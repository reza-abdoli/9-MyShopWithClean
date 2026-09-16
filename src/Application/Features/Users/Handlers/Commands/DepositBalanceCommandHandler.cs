// Application/Features/User/Handlers/Commands/DepositBalanceCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Features.Users.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers.Commands;

public class DepositBalanceCommandHandler
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<DepositBalanceCommandHandler> _logger;
    private const int MaxRetries = 5;

    public DepositBalanceCommandHandler(IRepository<User> userRepository, ILogger<DepositBalanceCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DepositBalanceCommand command)
    {
        var dto = command.DepositBalanceDto;
        if (dto.Amount <= 0)
            throw new ArgumentException("Deposit amount must be greater than zero.", nameof(dto.Amount));

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            var user = await _userRepository.GetEntityById(dto.Id);
            if (user == null)
                throw new InvalidOperationException($"User with ID {dto.Id} was not found.");

            user.Balance += dto.Amount;
            _userRepository.UpdateEntity(user);
            try
            {
                await _userRepository.SaveChanges();
                _logger.LogInformation(
                    "Balance deposited optimistically. UserId: {UserId}, Amount: {Amount}, Attempt: {Attempt}",
                    dto.Id, dto.Amount, attempt);
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                    entry.State = EntityState.Detached;

                _logger.LogWarning(
                    "Concurrency conflict while depositing balance, retrying. UserId: {UserId}, Attempt: {Attempt}",
                    dto.Id, attempt);
            }
        }

        throw new InvalidOperationException(
            $"Could not deposit balance for user {dto.Id} after {MaxRetries} attempts due to concurrent updates.");
    }
}