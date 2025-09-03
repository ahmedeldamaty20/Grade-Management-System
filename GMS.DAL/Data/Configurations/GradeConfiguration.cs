using GMS.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GMS.DAL.Data.Configurations;
public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Value).IsRequired();

        builder.HasOne(g => g.Session)
            .WithMany(s => s.Grades)
            .HasForeignKey(g => g.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Trainee)
            .WithMany(u => u.Grades)
            .HasForeignKey(g => g.TraineeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.SessionId, e.TraineeId }).IsUnique();
    }
}