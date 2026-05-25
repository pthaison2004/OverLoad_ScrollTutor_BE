using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Overload.DataAccessLayer.Entities;

namespace Overload.DataAccessLayer.Context;

public partial class OverloadDbContext : DbContext
{
    public OverloadDbContext(DbContextOptions<OverloadDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLessonProgress> UserLessonProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__courses__3213E83F2E6CF590");

            entity.ToTable("courses", tb => tb.HasTrigger("trg_courses_update"));

            entity.HasIndex(e => e.Slug, "UQ__courses__32DD1E4C507C93A7").IsUnique();

            entity.HasIndex(e => new { e.IsPublished, e.CreatedAt }, "idx_courses_published").IsDescending(false, true);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasDefaultValue("Khác")
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(false)
                .HasColumnName("is_published");
            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("beginner")
                .HasColumnName("level");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("slug");
            entity.Property(e => e.ThumbnailUrl).HasColumnName("thumbnail_url");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TotalDurationMinutes)
                .HasDefaultValue(0)
                .HasColumnName("total_duration_minutes");
            entity.Property(e => e.TotalLessons)
                .HasDefaultValue(0)
                .HasColumnName("total_lessons");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__enrollme__3213E83FEE596959");

            entity.ToTable("enrollments");

            entity.HasIndex(e => new { e.UserId, e.CourseId }, "UQ__enrollme__414FD874F9DB2016").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EnrolledAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("enrolled_at");
            entity.Property(e => e.LastAccessedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("last_accessed_at");
            entity.Property(e => e.ProgressPercentage)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progress_percentage");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__enrollmen__cours__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__enrollmen__user___45F365D3");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__lessons__3213E83F59568192");

            entity.ToTable("lessons", tb => tb.HasTrigger("trg_lessons_update"));

            entity.HasIndex(e => new { e.CourseId, e.OrderIndex }, "UQ__lessons__E4AE81D0DACB41AB").IsUnique();

            entity.HasIndex(e => new { e.CourseId, e.OrderIndex }, "idx_lessons_course");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationMinutes)
                .HasDefaultValue(0)
                .HasColumnName("duration_minutes");
            entity.Property(e => e.IsFree)
                .HasDefaultValue(true)
                .HasColumnName("is_free");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__lessons__course___3E52440B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F8E63D016");

            entity.ToTable("users", tb => tb.HasTrigger("trg_users_update"));

            entity.HasIndex(e => e.Email, "UQ__users__AB6E61640B70E37E").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("student")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserLessonProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_les__3213E83F3987A88D");

            entity.ToTable("user_lesson_progress", tb => tb.HasTrigger("trg_user_lesson_progress_update"));

            entity.HasIndex(e => new { e.UserId, e.LessonId }, "UQ__user_les__4FFC2875601347B1").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.LessonId }, "idx_progress_user_lesson");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Completed)
                .HasDefaultValue(false)
                .HasColumnName("completed");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.LastPositionSeconds)
                .HasDefaultValue(0)
                .HasColumnName("last_position_seconds");
            entity.Property(e => e.LastScrollPercentage)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("last_scroll_percentage");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.UnlockedCheckpointIndex)
                .HasDefaultValue(0)
                .HasColumnName("unlocked_checkpoint_index");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WatchTimeSeconds)
                .HasDefaultValue(0)
                .HasColumnName("watch_time_seconds");

            entity.HasOne(d => d.Lesson).WithMany(p => p.UserLessonProgresses)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("FK__user_less__lesso__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.UserLessonProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__user_less__user___52593CB8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
