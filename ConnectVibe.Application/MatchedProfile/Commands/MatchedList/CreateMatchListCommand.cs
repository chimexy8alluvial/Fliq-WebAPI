using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using ConnectVibe.Domain.Common.Errors;
using Fliq.Application.MatchedProfile.Common;
using MapsterMapper;
using MediatR;
using Fliq.Domain.Entities.MatchedProfile;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public class CreateMatchListCommand : IRequest<ErrorOr<CreateMatchListResult>>
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = default!;
        public int Age { get; set; }
        public MatchPhotoMapped Photos { get; set; } = default!;
        public DateTime RequestTime { get; set; } = default!;
    }

    //public class CreateMatchListCommandHandler : IRequestHandler<CreateMatchListCommand, ErrorOr<CreateMatchListResult>>
    //{
    //    private readonly IMapper _mapper;
    //    private readonly IImageService _imageService;
    //    private readonly IUserRepository _userRepository;
    //    private readonly IMatchProfileRepository _matchProfileRepository;

    //    public CreateMatchListCommandHandler(IMapper mapper, IImageService imageService, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository)
    //    {
    //        _mapper = mapper;
    //        _imageService = imageService;
    //        _userRepository = userRepository;
    //        _matchProfileRepository = matchProfileRepository;
    //    }

    //    public async Task<ErrorOr<List<CreateMatchListResult>> Handle(CreateMatchListCommand command, CancellationToken cancellationToken)
    //    {
    //        await Task.CompletedTask;

    //        var user = _userRepository.GetUserById(command.UserId);
    //        if (user == null)
    //        {
    //            return Errors.Profile.ProfileNotFound;
    //        }
    //        var requestId = _mapper.Map<MatchProfile>(command);
    //        int pageSize = 1;
    //        int pageNumber = 11;
    //        var filteredValue = _matchProfileRepository.GetMatchListById(requestId.UserId, pageNumber, pageSize);
    //        //var filteredValue = returnedList.Result;
    //        return new List<CreateMatchProfileResult>((IEnumerable<CreateMatchProfileResult>)filteredValue);
            
    //    }
    //}
}
