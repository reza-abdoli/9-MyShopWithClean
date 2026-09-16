// Application/Features/User/Handlers/Commands/WithdrawBalanceCommandHandler.cs
using System;
using System.Threading.Tasks;
using Application.Features.Users.Requests.Commands;
using Application.Interfaces.Persistence;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers.Commands;

public class WithdrawBalanceCommandHandler
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<WithdrawBalanceCommandHandler> _logger;
    private const int MaxRetries = 5;

    public WithdrawBalanceCommandHandler(IRepository<User> userRepository, ILogger<WithdrawBalanceCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(WithdrawBalanceCommand command)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            var user = await _userRepository.GetEntityById(command.UserId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {command.UserId} was not found.");

            if (user.Balance < command.Amount)
                throw new InvalidOperationException("Insufficient wallet balance for this transaction.");

            user.Balance -= command.Amount;
            _userRepository.UpdateEntity(user);

            try
            {
                await _userRepository.SaveChanges();
                _logger.LogInformation(
                    "Balance withdrawn optimistically. UserId: {UserId}, Amount: {Amount}, Attempt: {Attempt}",
                    command.UserId, command.Amount, attempt);
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(
                    "Concurrency conflict while withdrawing balance. UserId: {UserId}, Attempt: {Attempt}",
                    command.UserId, attempt);

                foreach (var entry in ex.Entries)
                    entry.State = EntityState.Detached;
            }
        }

        throw new InvalidOperationException(
            $"Could not withdraw balance for user {command.UserId} after {MaxRetries} attempts due to concurrent updates.");
    }
}