using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MapsterMapper;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.DashBoard.Command.DeleteUser
{
    public record DeleteUserByIdCommand(int UserId) : IRequest<ErrorOr<DeleteUserResult>>;

    public class DeleteUserByIdCommandHandler : IRequestHandler<DeleteUserByIdCommand, ErrorOr<DeleteUserResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public DeleteUserByIdCommandHandler(IUserRepository userRepository, IMapper mapper, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ErrorOr<DeleteUserResult>> Handle(DeleteUserByIdCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Deleting user with {command.UserId} ");


            var user = _userRepository.GetUserById(command.UserId);

            if (user == null)
            {
                _logger.LogError("User not found");
                return Errors.User.UserNotFound;
            }

            if (user.IsDeleted)
            {
                _logger.LogError($"This user with id {user.Id} has been delete before");
                return Errors.User.UserNotFound;
            }

                user.IsDeleted = true;

            _userRepository.Update(user);

            _logger.LogInfo($"User with Id {command.UserId} was deleted");     

                return new DeleteUserResult(
                   Message:( $"{user.Id} has been deleted" )
                );
          
            
        }
    }
}
