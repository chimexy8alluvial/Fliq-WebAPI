using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        public OtpRepository(ConnectVibeDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CheckActiveOtpAsync(string email, string code)
        {
            var otp = await _dbContext.OTPs.Where(o => o.Email == email && o.Code == code && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
        .FirstOrDefaultAsync();
            if (otp == null)
                return false;

            otp.IsUsed = true;
            _dbContext.Update(otp);
            _dbContext.SaveChanges();
            return true;
        }

        public void Add(OTP otp)
        {
            _dbContext.Add(otp);
            _dbContext.SaveChanges();
        }

        public async Task<bool> OtpExistAsync(string email, string code)
        {
            var otp = await _dbContext.OTPs.Where(o => o.Email == email && o.Code == code && o.IsUsed)
        .FirstOrDefaultAsync();
            if (otp == null)
                return false;
            return true;
        }
    }
}
