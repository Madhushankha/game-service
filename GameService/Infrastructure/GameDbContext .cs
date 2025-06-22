using GameService.Model;
using Microsoft.EntityFrameworkCore;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options) { }

    public DbSet<GameModel> Games { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GameModel>(entity =>
        {
            // PK
            entity.HasKey(g => g.Id);

            // Map Guid <→> CHAR(36)
            entity.Property(g => g.Id);

            // Name & Category lengths
            entity.Property(g => g.Name)
                  .HasMaxLength(200)
                  .IsRequired();
            entity.Property(g => g.Category)
                  .HasMaxLength(100)
                  .IsRequired();

            // ReleasedDate as DATE
            entity.Property(g => g.ReleasedDate)
                  .HasColumnType("date")
                  .IsRequired();

            // Price
            entity.Property(g => g.Price)
                  .HasColumnType("decimal(10,2)")
                  .IsRequired();

            // CreatedAt: default CURRENT_TIMESTAMP, generated on add
            entity.Property(g => g.CreatedAt)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .ValueGeneratedOnAdd()
                  .IsRequired();
        });
    }
}
