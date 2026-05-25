IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Overload')
BEGIN
    CREATE DATABASE Overload;
END
GO

USE Overload;
GO

-- =============================================
-- 1. Bảng Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.users') AND type = 'U')
CREATE TABLE users (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name NVARCHAR(255),
    avatar_url NVARCHAR(MAX),
    bio NVARCHAR(MAX),
    role VARCHAR(20) DEFAULT 'student' CHECK (role IN ('student', 'instructor', 'admin')),
    is_verified BIT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE()
);
GO

-- =============================================
-- 2. Bảng Courses
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.courses') AND type = 'U')
CREATE TABLE courses (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    title NVARCHAR(255) NOT NULL,
    slug VARCHAR(255) UNIQUE NOT NULL,
    description NVARCHAR(MAX),
    thumbnail_url NVARCHAR(MAX),
    category NVARCHAR(100) DEFAULT N'Khác',
    level VARCHAR(20) DEFAULT 'beginner' CHECK (level IN ('beginner', 'intermediate', 'advanced')),
    is_published BIT DEFAULT 0,
    total_duration_minutes INT DEFAULT 0,
    total_lessons INT DEFAULT 0,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE()
);
GO

-- =============================================
-- 3. Bảng Lessons
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.lessons') AND type = 'U')
CREATE TABLE lessons (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    course_id UNIQUEIDENTIFIER NOT NULL,
    title NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    content NVARCHAR(MAX) NOT NULL,           -- Dùng NVARCHAR(MAX) thay JSON tạm thời (hoặc JSON nếu SQL Server 2016+)
    duration_minutes INT DEFAULT 0,
    order_index INT NOT NULL,
    is_free BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE,
    UNIQUE (course_id, order_index)
);
GO

-- =============================================
-- 4. Bảng Enrollments
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.enrollments') AND type = 'U')
CREATE TABLE enrollments (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER NOT NULL,
    course_id UNIQUEIDENTIFIER NOT NULL,
    enrolled_at DATETIME2 DEFAULT GETDATE(),
    completed_at DATETIME2 NULL,
    progress_percentage DECIMAL(5,2) DEFAULT 0.00,
    last_accessed_at DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE CASCADE,
    UNIQUE (user_id, course_id)
);
GO

-- =============================================
-- 5. Bảng User Lesson Progress
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.user_lesson_progress') AND type = 'U')
CREATE TABLE user_lesson_progress (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER NOT NULL,
    lesson_id UNIQUEIDENTIFIER NOT NULL,
    
    last_scroll_percentage DECIMAL(5,2) DEFAULT 0.00,
    unlocked_checkpoint_index INT DEFAULT 0,
    completed BIT DEFAULT 0,
    completed_at DATETIME2 NULL,
    
    last_position_seconds INT DEFAULT 0,
    watch_time_seconds INT DEFAULT 0,

    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (lesson_id) REFERENCES lessons(id) ON DELETE CASCADE,
    UNIQUE (user_id, lesson_id)
);
GO

-- =============================================
-- INDEXES
-- =============================================
CREATE NONCLUSTERED INDEX idx_courses_published 
ON courses(is_published, created_at DESC);

CREATE NONCLUSTERED INDEX idx_lessons_course 
ON lessons(course_id, order_index);

CREATE NONCLUSTERED INDEX idx_progress_user_lesson 
ON user_lesson_progress(user_id, lesson_id);
GO

-- =============================================
-- TRIGGER cập nhật updated_at tự động
-- =============================================
CREATE TRIGGER trg_users_update
ON users
AFTER UPDATE
AS
BEGIN
    UPDATE users
    SET updated_at = GETDATE()
    FROM users u
    INNER JOIN inserted i ON u.id = i.id;
END
GO

CREATE TRIGGER trg_courses_update
ON courses
AFTER UPDATE
AS
BEGIN
    UPDATE courses
    SET updated_at = GETDATE()
    FROM courses c
    INNER JOIN inserted i ON c.id = i.id;
END
GO

CREATE TRIGGER trg_lessons_update
ON lessons
AFTER UPDATE
AS
BEGIN
    UPDATE lessons
    SET updated_at = GETDATE()
    FROM lessons l
    INNER JOIN inserted i ON l.id = i.id;
END
GO

CREATE TRIGGER trg_user_lesson_progress_update
ON user_lesson_progress
AFTER UPDATE
AS
BEGIN
    UPDATE user_lesson_progress
    SET updated_at = GETDATE()
    FROM user_lesson_progress p
    INNER JOIN inserted i ON p.id = i.id;
END
GO