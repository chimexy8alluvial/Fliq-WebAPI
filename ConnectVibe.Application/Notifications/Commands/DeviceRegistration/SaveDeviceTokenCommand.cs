using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Notifications.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Notifications;
using MediatR;

namespace Fliq.Application.Notifications.Commands.DeviceRegistration
{
    public record SaveDeviceTokenCommand(int UserId, string DeviceToken) : IRequest<ErrorOr<SaveDeviceTokenResult>>;

    public class SaveDeviceTokenCommandHandler : IRequestHandler<SaveDeviceTokenCommand, ErrorOr<SaveDeviceTokenResult>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public SaveDeviceTokenCommandHandler(INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task<ErrorOr<SaveDeviceTokenResult>> Handle(SaveDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                return Errors.User.UserNotFound;
            }

            var deviceToken = new UserDeviceToken
            {
                UserId = request.UserId,
                DeviceToken = request.DeviceToken,
            };

            _notificationRepository.RegisterDeviceToken(deviceToken);

            return new SaveDeviceTokenResult(true, "Device token saved successfully");
        }
    }

}
