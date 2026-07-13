using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OverLoad.Domain.Entities;

namespace OverLoad.Repositories.Configurations;

public class BugReportConfiguration : IEntityTypeConfiguration<BugReport>
{
    public void Configure(EntityTypeBuilder<BugReport> builder)
    {
        builder.ToTable("BugReports");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(250);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.Status).HasConversion<int>();

        builder.HasOne(p => p.User)
            .WithMany(u => u.BugReports)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Course)
            .WithMany(c => c.BugReports)
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Lesson)
            .WithMany(l => l.BugReports)
            .HasForeignKey(p => p.LessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
