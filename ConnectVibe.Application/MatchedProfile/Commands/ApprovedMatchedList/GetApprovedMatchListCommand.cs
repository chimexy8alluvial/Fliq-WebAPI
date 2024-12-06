﻿using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Pagination;
using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.ApprovedMatchedList
{
    public record GetApprovedMatchListCommand(int UserId, PaginationRequest PaginationRequest = default!, MatchRequestStatus? MatchRequestStatus = null) : IRequest<ErrorOr<List<MatchRequestDto>>>;

    public class GetApprovedMatchListCommandHandler : IRequestHandler<GetApprovedMatchListCommand, ErrorOr<List<MatchRequestDto>>>
    {
        private readonly IMatchProfileRepository _matchProfileRepository;

        public GetApprovedMatchListCommandHandler(IMatchProfileRepository matchProfileRepository)
        {
            _matchProfileRepository = matchProfileRepository;
        }

        public async Task<ErrorOr<List<MatchRequestDto>>> Handle(GetApprovedMatchListCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var filteredValue = await _matchProfileRepository.GetApproveMatchListById(command);
            var result = filteredValue.ToList();

            return result;
        }
    }
}