using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartDormitory.Data.Models;
using SmartDormitory.Data.Models.Contracts;
using System;
using System.Linq;

namespace SmartDormitory.App.Data
{
    public class SmartDormitoryContext : IdentityDbContext<User>
    {
        public SmartDormitoryContext()
        {

        }
        public SmartDormitoryContext(DbContextOptions<SmartDormitoryContext> options)
            : base(options)
        {
        }

        public DbSet<Sensor> Sensors { get; set; }

        public DbSet<IcbSensor> IcbSensors { get; set; }

        public DbSet<MeasureType> MeasureTypes { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            this.SetupEntitiesRelations(builder);
            this.SeedMeasureTypes(builder);

            base.OnModelCreating(builder);
        }

        private void SetupEntitiesRelations(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(u => u.Sensors)
                .WithOne(s => s.User)
                .HasForeignKey(u => u.UserId);

            builder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.Receiver)
                .HasForeignKey(n => n.ReceiverId);

            builder.Entity<IcbSensor>()
                .HasMany(x => x.Sensors)
                .WithOne(s => s.IcbSensor)
                .HasForeignKey(s => s.IcbSensorId);

            builder.Entity<MeasureType>()
                .HasMany(mt => mt.IcbSensors)
                .WithOne(s => s.MeasureType)
                .HasForeignKey(s => s.MeasureTypeId);

            builder.Entity<Sensor>()
                   .OwnsOne(s => s.Coordinates,
                            c =>
                            {
                                c.Property(p => p.Latitude).HasColumnName("Latitude");
                                c.Property(p => p.Longitude).HasColumnName("Longitude");
                            });
        }

        private void SeedMeasureTypes(ModelBuilder builder)
        {
            builder.Entity<MeasureType>()
                   .HasData(
                            new MeasureType
                            {
                                Id = Guid.NewGuid().ToString(),
                                MeasureUnit = "°C",
                                SuitableSensorType = "Temperature",
                                CreatedOn = DateTime.Now
                            },
                           new MeasureType
                           {
                               Id = Guid.NewGuid().ToString(),
                               MeasureUnit = "%",
                               SuitableSensorType = "Humidity",
                               CreatedOn = DateTime.Now
                           },
                           new MeasureType
                           {
                               Id = Guid.NewGuid().ToString(),
                               MeasureUnit = "W",
                               SuitableSensorType = "Electric power consumtion",
                               CreatedOn = DateTime.Now
                           },
                           new MeasureType
                           {
                               Id = Guid.NewGuid().ToString(),
                               MeasureUnit = "(true/false)",
                               SuitableSensorType = "Boolean switch (door/occupancy/etc)",
                               CreatedOn = DateTime.Now
                           },
                           new MeasureType
                           {
                               Id = Guid.NewGuid().ToString(),
                               MeasureUnit = "dB",
                               SuitableSensorType = "Noise",
                               CreatedOn = DateTime.Now
                           });
        }

        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges();
        }

        private void ApplyAuditInfoRules()
        {
            var newlyCreatedEntities = this.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditable && ((e.State == EntityState.Added) || (e.State == EntityState.Modified)));

            foreach (var entry in newlyCreatedEntities)
            {
                var entity = (IAuditable)entry.Entity;

                if (entry.State == EntityState.Added && entity.CreatedOn == null)
                {
                    entity.CreatedOn = DateTime.Now;
                }
                else
                {
                    entity.ModifiedOn = DateTime.Now;
                }
            }
        }
    }
}
