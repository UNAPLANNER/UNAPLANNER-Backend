using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UNAPLANNER_API.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyPlanCourseElectiveType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[FK_Students_Users_UserId]', N'F') IS NOT NULL
                BEGIN
                    ALTER TABLE [Students] DROP CONSTRAINT [FK_Students_Users_UserId];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[CK_StudyPlanCourse_Levels]', N'C') IS NOT NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses] DROP CONSTRAINT [CK_StudyPlanCourse_Levels];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[dbo].[StudyPlanCourses]', N'ElectiveType') IS NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses]
                    ADD [ElectiveType] nvarchar(max) NOT NULL
                    CONSTRAINT [DF_StudyPlanCourses_ElectiveType] DEFAULT N'Obligatorio';
                END
                """);

            migrationBuilder.Sql("""
                UPDATE [StudyPlanCourses]
                SET [ElectiveType] = N'Obligatorio'
                WHERE [ElectiveType] IS NULL OR [ElectiveType] = N'';
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[CK_StudyPlanCourse_ElectiveType]', N'C') IS NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses]
                    ADD CONSTRAINT [CK_StudyPlanCourse_ElectiveType]
                    CHECK ([ElectiveType] IN (N'Obligatorio', N'OptativoDisciplinario', N'OptativoLibre'));
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[CK_StudyPlanCourse_Levels]', N'C') IS NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses]
                    ADD CONSTRAINT [CK_StudyPlanCourse_Levels]
                    CHECK ([Levels] BETWEEN 1 AND 4);
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[FK_Students_Users_UserId]', N'F') IS NULL
                BEGIN
                    ALTER TABLE [Students]
                    ADD CONSTRAINT [FK_Students_Users_UserId]
                    FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[FK_Students_Users_UserId]', N'F') IS NOT NULL
                BEGIN
                    ALTER TABLE [Students] DROP CONSTRAINT [FK_Students_Users_UserId];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[CK_StudyPlanCourse_ElectiveType]', N'C') IS NOT NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses] DROP CONSTRAINT [CK_StudyPlanCourse_ElectiveType];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[CK_StudyPlanCourse_Levels]', N'C') IS NOT NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses] DROP CONSTRAINT [CK_StudyPlanCourse_Levels];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[DF_StudyPlanCourses_ElectiveType]', N'D') IS NOT NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses] DROP CONSTRAINT [DF_StudyPlanCourses_ElectiveType];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[dbo].[StudyPlanCourses]', N'ElectiveType') IS NOT NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses] DROP COLUMN [ElectiveType];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[CK_StudyPlanCourse_Levels]', N'C') IS NULL
                BEGIN
                    ALTER TABLE [StudyPlanCourses]
                    ADD CONSTRAINT [CK_StudyPlanCourse_Levels]
                    CHECK ([Levels] BETWEEN 1 AND 6);
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[FK_Students_Users_UserId]', N'F') IS NULL
                BEGIN
                    ALTER TABLE [Students]
                    ADD CONSTRAINT [FK_Students_Users_UserId]
                    FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
                END
                """);
        }
    }
}
