

using ErrorOr;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.LiveComms.Common.Chats;
using MediatR;
using StreamChat.Clients;
using StreamChat.Models;

namespace Fliq.Application.LiveComms.Commands.Chats
{
    public record CreateChatChannelCommand(
    string User1Id,
    string User2Id
        ) : IRequest<ErrorOr<CreateChatChannelResult>>;

    public class CreateChatChannelHandler : IRequestHandler<CreateChatChannelCommand, ErrorOr<CreateChatChannelResult>>
    {
        private readonly StreamClientFactory _streamClientFactory;
        private readonly ILoggerManager _logger;

        public CreateChatChannelHandler(StreamClientFactory streamClientFactory, ILoggerManager logger)
        {
            _streamClientFactory = streamClientFactory;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateChatChannelResult>> Handle(CreateChatChannelCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var channelClient = _streamClientFactory.GetChannelClient();

                var channelId = $"chat_{command.User1Id}_{command.User2Id}"; // Unique identifier for the chat

                var data = new ChannelRequest();
                data.CreatedBy = new UserRequest { Id = command.User1Id }; // The initiator of the chat
                data.SetData("participants", new[] { command.User1Id, command.User2Id });

                var channelResponse = await channelClient.GetOrCreateAsync(channelId, new ChannelGetRequest { Data = data });

                _logger.LogInfo($"Chat channel created successfully: {channelId}");

                return new CreateChatChannelResult( channelResponse.Channel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating chat channel: {ex.Message}");
                return Error.Failure("ChatChannel.CreationFailed", "Failed to create or retrieve chat channel.");
            }
        }
    }
}
