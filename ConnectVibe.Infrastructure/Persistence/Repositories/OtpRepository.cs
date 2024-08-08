using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Infrastructure.Persistence.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ConnectVibeDbContext _dbContext;
        public OtpRepository(ConnectVibeDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool CheckActiveOtp(string email, string code)
        {
            var otp = _dbContext.OTPs.Where(o => o.Email == email && o.Code == code && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
        .FirstOrDefault();
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
    }
}
