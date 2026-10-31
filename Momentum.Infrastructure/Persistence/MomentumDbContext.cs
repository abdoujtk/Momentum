

using Microsoft.EntityFrameworkCore;
using Momentum.Domain.Entities;

namespace Momentum.Infrastructure.Persistence;

public partial class MomentumDbContext : DbContext
{
    public MomentumDbContext(DbContextOptions<MomentumDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CheckIn> CheckIns { get; set; }

    public virtual DbSet<Habit> Habits { get; set; }

    public virtual DbSet<Mood> Moods { get; set; }

    public virtual DbSet<Quote> Quotes { get; set; }

    public virtual DbSet<StreakFreeze> StreakFreezes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Categories_Name").IsUnique();

            entity.Property(e => e.ColorHex)
                .HasMaxLength(7)
                .HasDefaultValue("#4CAF50");
            entity.Property(e => e.IconName)
                .HasMaxLength(50)
                .HasDefaultValue("circle");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.HasIndex(e => e.CheckInDate, "IX_CheckIns_CheckInDate");

            entity.HasIndex(e => e.HabitId, "IX_CheckIns_HabitId");

            

            entity.HasIndex(e => e.MoodId, "IX_CheckIns_MoodId");

            entity.HasIndex(e => new { e.HabitId, e.CheckInDate }, "UQ_CheckIns_Habit_Date").IsUnique();

            entity.Property(e => e.ActualValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasOne(d => d.Habit).WithMany(p => p.CheckIns)
                .HasForeignKey(d => d.HabitId)
                .HasConstraintName("FK_CheckIns_Habits");

            entity.HasOne(d => d.Mood).WithMany(p => p.CheckIns)
                .HasForeignKey(d => d.MoodId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_CheckIns_Moods");
        });

        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Habits_CategoryId");

            entity.HasIndex(e => e.IsArchived, "IX_Habits_IsArchived");

            entity.HasIndex(e => e.UserId, "IX_Habits_UserId");

            entity.Property(e => e.ColorHex).HasMaxLength(7);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DaysOfWeek).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FrequencyType).HasMaxLength(20);
            entity.Property(e => e.IconName).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.TargetUnit).HasMaxLength(20);
            entity.Property(e => e.TargetValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Habits)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Habits_Categories");

            entity.HasOne(d => d.User).WithMany(p => p.Habits)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Habits_Users");
        });

        modelBuilder.Entity<Mood>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Moods_Name").IsUnique();

            entity.Property(e => e.ColorHex)
                .HasMaxLength(7)
                .HasDefaultValue("#9E9E9E");
            entity.Property(e => e.Emoji).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Quote>(entity =>
        {
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Text).HasMaxLength(500);
        });

        modelBuilder.Entity<StreakFreeze>(entity =>
        {
            entity.HasKey(e => e.FreezeId);

            entity.HasIndex(e => e.HabitId, "IX_StreakFreezes_HabitId");

            entity.HasIndex(e => new { e.HabitId, e.FreezeDate }, "UQ_StreakFreezes_Habit_Date").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(200);

            entity.HasOne(d => d.Habit).WithMany(p => p.StreakFreezes)
                .HasForeignKey(d => d.HabitId)
                .HasConstraintName("FK_StreakFreezes_Habits");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginAt).HasColumnType("datetime");
            entity.Property(e => e.PreferredLanguage)
                .HasMaxLength(10)
                .HasDefaultValue("ar");
            entity.Property(e => e.TimeZoneId)
                .HasMaxLength(100)
                .HasDefaultValue("Asia/Amman");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
