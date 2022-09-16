using Mango.Service.Email.DbContexts;
using Mango.Service.Email.Messages;
using Mango.Service.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;
        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            //throw new NotImplementedException();
            EmailLog emailLog = new EmailLog
            {
                Email = message.Email,
                EmailSent = DateTime.UtcNow,
                Log = $"Order - {message.OrderId} has been created successfully"
            };

            await using var _db = new ApplicationDbContext(_dbContext);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();
        }
    }
}
