using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
#nullable disable

namespace EventProjectFinal.Models
{
    public partial class EVENTS3Context : IdentityDbContext<AppUser>
    {
        public readonly IHttpContextAccessor httpContextAccessor;
        public EVENTS3Context()
        {
        }
        //To be able to access current user.
        public EVENTS3Context(DbContextOptions<EVENTS3Context> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        
        
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventUser> EventUsers { get; set; }

        public DbSet<AppUserTokens> AppUserTokens { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=ILKER\\SQLEXPRESS;Database=EVENTS3;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");
            OnModelCreatingPartial(modelBuilder);
            //Identity implementation
            base.OnModelCreating(modelBuilder);
            
            //Add admin user by default.
            const string ADMIN_ID = "1";
            const string ROLE_ID = "1";

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = ROLE_ID,
                Name = "Admin",
                NormalizedName = "Admin"
            });

            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = ADMIN_ID,
                UserName = "admin@gmail.com",
                NormalizedUserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com",
                Name = "Admin",
                EmailConfirmed = false,
                PasswordHash = hasher.HashPassword(null, "admin"),
                SecurityStamp = String.Empty,
                Role = "Admin"
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }


}