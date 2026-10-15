using Charon.WakeOnLanWebService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Charon.WakeOnLanWebService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<NetworkDeviceEntity> Devices { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NetworkDeviceEntity>(entity =>
        {
            entity.HasKey(x => x.MacAddress);
            entity
                .Property(x => x.MacAddress)
                .Metadata.SetValueComparer(new ValueComparer<byte[]>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray()
                ));
            entity
                .Property(x => x.MacAddress)
                .HasMaxLength(6);
            
            entity
                .Property(x => x.IpAddress)
                .HasMaxLength(16);
            
            entity
                .Property(x => x.Title)
                .HasMaxLength(256);
        });
    }
}