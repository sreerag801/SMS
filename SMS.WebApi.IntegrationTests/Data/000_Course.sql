INSERT INTO dbo.Courses (CourseName, CourseDescription, DurationMonths, EntityId, CreatedAt, UpdatedAt, DeletedAt)
VALUES('Physics', 'Course physics', 10, newid(), GETDATE(), GETDATE(), null);