﻿using Mango.Service.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Service.Email.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        public DbSet<EmailLog> EmailLogs { get; set; }

    }
}
