using ConnectVibe.Domain.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Common.Interfaces.Persistence
{
    public interface IProfileRepository
    {
        void Add(UserProfile userProfile);

        UserProfile? GetUserProfileByUserId(int id);
    }
}