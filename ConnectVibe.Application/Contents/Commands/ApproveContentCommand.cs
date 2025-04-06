using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Interfaces;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.Contents.Commands
{
    public record ApproveContentCommand<T>(int ContentId, int AdminUserId) : IRequest<ErrorOr<Unit>>;

    public class ApproveContentCommandHandler<T> : IRequestHandler<ApproveContentCommand<T>, ErrorOr<Unit>> where T : class, IApprovableContent
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRepositoryFactory _repositoryFactory;

        public ApproveContentCommandHandler(ILoggerManager logger, IUserRepository userRepository, IRepositoryFactory repositoryFactory)
        {
            _logger = logger;
            _userRepository = userRepository;
            _repositoryFactory = repositoryFactory;
        }

        public async Task<ErrorOr<Unit>> Handle(ApproveContentCommand<T> command, CancellationToken cancellationToken)
        {
            var repository = _repositoryFactory.GetRepository<T>();
            if(repository == null)
            {
                _logger.LogInfo($"Repository for type {typeof(T).Name} not found");
                return Errors.Content.RepositoryNotFound;
            }

            var content = await repository.GetByIdAsync(command.ContentId);
            if (content == null)
            {
                _logger.LogError($" {typeof(T).Name} content with ID: {command.ContentId} was not found.");
                return Errors.Content.ContentNotFound;
            }

            if (content.ContentCreationStatus == ContentCreationStatus.Approved)
            {
                _logger.LogError($" {typeof(T).Name} content with ID: {command.ContentId} has been approved already.");
                return Errors.Content.ContentAlreadyApproved;
            }

            var aadminUser = _userRepository.GetUserById(command.AdminUserId); //update this to get user by id and role for faster fetch
            if (aadminUser == null)
            {
                _logger.LogError($"Admin with Id: {command.AdminUserId} was not found.");
                return Errors.User.UserNotFound;
            }

            content.ContentCreationStatus = ContentCreationStatus.Approved;
            content.ApprovedAt = DateTime.UtcNow;
            content.ApprovedByUserId = aadminUser.Id;

            await repository.UpdateAsync(content);

            _logger.LogInfo($" {typeof(T).Name} content with ID: {command.ContentId} was approved");

            return Unit.Value;

        }
    }

}
