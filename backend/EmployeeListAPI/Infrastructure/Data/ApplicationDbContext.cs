using EmployeeListAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeListAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Salary)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.HireDate)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Create unique index on email
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Employees_Email");

                // Create index on IsActive for better performance
                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_Employees_IsActive");
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@company.com",
                    PhoneNumber = "123-456-7890",
                    Department = "IT",
                    Position = "Software Developer",
                    Salary = 75000,
                    HireDate = new DateTime(2023, 1, 15),
                    IsActive = true,
                    CreatedAt = new DateTime(2023, 1, 15, 9, 0, 0, DateTimeKind.Utc)
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@company.com",
                    PhoneNumber = "987-654-3210",
                    Department = "HR",
                    Position = "HR Manager",
                    Salary = 85000,
                    HireDate = new DateTime(2022, 6, 1),
                    IsActive = true,
                    CreatedAt = new DateTime(2022, 6, 1, 9, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
