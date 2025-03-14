﻿using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using MapsterMapper;
using MediatR;


namespace Fliq.Application.DashBoard.Queries.GetAllUser
{
    public record GetAllUsersQuery(PaginationRequest PaginationRequest = default!,
                                        bool? HasSubscription=null ,
                                          DateTime? ActiveSince = null,
                                            string? RoleName = null)          : IRequest<ErrorOr<List<GetUsersResult>>>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<List<GetUsersResult>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper, ILoggerManager logger)
        {
            _userRepository = userRepository;          
            _logger = logger;
        }

        public async Task<ErrorOr<List<GetUsersResult>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _logger.LogInfo($"Getting users for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}");
          

            var request = new GetUsersListRequest
            {
                PaginationRequest = query.PaginationRequest,
                HasSubscription = query.HasSubscription,
                ActiveSince = query.ActiveSince,
                RoleName = query.RoleName
             
              
            };
         
            var users = _userRepository.GetAllUsersForDashBoard(request);

            _logger.LogInfo($"Got {users.Count()} users for page {query.PaginationRequest.PageNumber}");

            var results = users.Select(user =>
            {
                
                var latestSubscription = user.Subscriptions?
                    .OrderByDescending(s => s.StartDate)
                    .FirstOrDefault();

                return new GetUsersResult(
                    DisplayName: user.DisplayName,
                    Email: user.Email,
                    SubscriptionType: latestSubscription?.ProductId ?? "Free",
                    DateJoined: user.DateCreated,
                    LastOnline: user.LastActiveAt
                );
            }).ToList();

            return results;
        }
    }
}
