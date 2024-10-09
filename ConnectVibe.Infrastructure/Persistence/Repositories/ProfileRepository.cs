﻿using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public ProfileRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(UserProfile userProfile)
        {
            if (userProfile.Id > 0)
            {
                _dbContext.Update(userProfile);
            }
            else
            {
                _dbContext.Add(userProfile);
            }
            _dbContext.SaveChanges();
        }

        public UserProfile? GetUserProfileByUserId(int id)
        {
            var profile = _dbContext.UserProfiles.SingleOrDefault(p => p.UserId == id);
            return profile;
        }

        public IEnumerable<UserProfile> GetMatchedUserProfiles(
        int userId,
        List<ProfileType> userProfileTypes,
        bool? filterByDating,
        bool? filterByFriendship,
        PaginationRequest paginationRequest)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = CreateDynamicParameters(userId, userProfileTypes, filterByDating, filterByFriendship, paginationRequest);

                var profiles = connection.Query<UserProfile>(
                    "sPGetMatchedUserProfiles",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return profiles;
            }
        }

        private static DynamicParameters CreateDynamicParameters(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship, PaginationRequest paginationRequest)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId);
            parameters.Add("@profileTypes", string.Join(",", userProfileTypes.Select(pt => pt.ToString())));
            parameters.Add("@filterByDating", filterByDating, DbType.Boolean);
            parameters.Add("@filterByFriendship", filterByFriendship, DbType.Boolean);
            parameters.Add("@pageNumber", paginationRequest.PageNumber);
            parameters.Add("@pageSize", paginationRequest.PageSize);

            return parameters;
        }



    }
}