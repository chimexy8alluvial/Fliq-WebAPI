using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public void Update(UserProfile profile)
        {
            _dbContext.Update(profile);

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

                var profiles = connection.Query<dynamic>(
                    "sPGetMatchedUserProfiles",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var newProfiles = new List<UserProfile>();

                foreach(var p in profiles)
                {
                    var profile = new UserProfile();

                    profile.Id = p.Id;
                    profile.DateCreated = p.DateCreated;
                    profile.DateModified = p.DateModified;
                    profile.DOB = p.DOB;

                    // Setting other properties
                    // profile.Gender = p.Gender;
                    profile.Gender = new Gender
                    {
                        Id = p.GenderId,
                        GenderType = (GenderType)p.GenderType,
                        IsVisible = p.GenderVisible
                    };

                    profile.ProfileDescription = p.ProfileDescription;
                    profile.SexualOrientation = p.SexualOrientation;
                    profile.Religion = p.Religion;
                    profile.Ethnicity = p.Ethnicity;
                    profile.Occupation = p.Occupation;
                    profile.EducationStatus = p.EducationStatus;
                    profile.HaveKids = p.HaveKids;
                    profile.WantKids = p.WantKids;
                    profile.Location = p.Location;
                    profile.AllowNotifications = p.AllowNotifications;

                    newProfiles.Add(profile);
                }
                return newProfiles;
                // Since the stored procedure already returns full objects, you don't need complex mapping
                //return profiles.Select(profile => new UserProfile
                //{
                //    UserId = profile.UserId,
                //    DOB = profile.Dob,
                //    Id = profile.Id,
                //    DateCreated = profile.DateCreated,
                //    DateModified = profile.DateModified,
                //    IsDeleted = profile.IsDeleted,
                //    Gender = new Gender
                //    {
                //        Id = profile.Gender.Id,
                //        GenderType = profile.Gender.GenderType,
                //        IsVisible = profile.Gender.IsVisible
                //    },
                //    SexualOrientation = new SexualOrientation
                //    {
                //        Id = profile.SexualOrientation.Id,
                //        SexualOrientationType = profile.SexualOrientation.SexualOrientationType,
                //        IsVisible = profile.SexualOrientation.IsVisible
                //    },
                //    Religion = new Religion
                //    {
                //        Id = profile.Religion.Id,
                //        ReligionType = profile.Religion.ReligionType,
                //        IsVisible = profile.Religion.IsVisible
                //    },
                //    Ethnicity = new Ethnicity
                //    {
                //        Id = profile.Ethnicity.Id,
                //        EthnicityType = profile.Ethnicity.EthnicityType,
                //        IsVisible = profile.Ethnicity.IsVisible
                //    },
                //    HaveKids = new HaveKids
                //    {
                //        Id = profile.HaveKids.Id,
                //        HaveKidsType = profile.HaveKids.HaveKidsType,
                //        IsVisible = profile.HaveKids.IsVisible
                //    },
                //    WantKids = new WantKids
                //    {
                //        Id = profile.WantKids.Id,
                //        WantKidsType = profile.WantKids.WantKidsType,
                //        IsVisible = profile.WantKids.IsVisible
                //    },
                //    Location = new Location
                //    {
                //        Id = profile.Location.Id,
                //        Lat = profile.Location.Lat,
                //        Lng = profile.Location.Lng,
                //        IsVisible = profile.Location.IsVisible
                //    },
                //    AllowNotifications = profile.AllowNotifications,
                //    Passions = profile.Passions,
                //    Photos = profile.Photos
                //}).ToList();

                // return profiles.Select(ProfileMapper.MapToUserProfile).ToList();
            }
        }

        public UserProfile? GetProfileByUserId(int id)
        {
            var query = _dbContext.UserProfiles.AsQueryable();

            // Use reflection to include all navigation properties
            foreach (var property in _dbContext.Model.FindEntityType(typeof(UserProfile)).GetNavigations())
            {
                query = query.Include(property.Name);
            }

            var profile = query.SingleOrDefault(p => p.UserId == id);
            return profile;
        }

        private static DynamicParameters CreateDynamicParameters(int userId, List<ProfileType> userProfileTypes, bool? filterByDating, bool? filterByFriendship, PaginationRequest paginationRequest)
        {
            string serializedProfileTypes = JsonConvert.SerializeObject(userProfileTypes);
            Console.WriteLine(serializedProfileTypes);

            var parameters = new DynamicParameters();

            parameters.Add("@userId", userId);

            // Convert ProfileType enum values to integers and join them as a comma-separated string
            parameters.Add("@profileTypes", serializedProfileTypes);  // Comma-separated list of integers

            parameters.Add("@filterByDating", filterByDating, DbType.Boolean);
            parameters.Add("@filterByFriendship", filterByFriendship, DbType.Boolean);
            parameters.Add("@pageNumber", paginationRequest.PageNumber);
            parameters.Add("@pageSize", paginationRequest.PageSize);

            return parameters;
        }


    }
}