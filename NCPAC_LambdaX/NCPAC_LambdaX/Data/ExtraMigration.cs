using Microsoft.EntityFrameworkCore.Migrations;

namespace NCPAC_LambdaX.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            //Triggers for Member
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetMemberTimestampOnUpdate
                    AFTER UPDATE ON Members
                    BEGIN
                        UPDATE Members
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetMemberTimestampOnInsert
                    AFTER INSERT ON Members
                    BEGIN
                        UPDATE Members
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

            //Triggers for Commitee
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetCommiteeTimestampOnUpdate
                    AFTER UPDATE ON Commitees
                    BEGIN
                        UPDATE Commitees
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetCommiteeTimestampOnInsert
                    AFTER INSERT ON Commitees
                    BEGIN
                        UPDATE Commitees
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

            //Triggers for Employee
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetEmployeeTimestampOnUpdate
                    AFTER UPDATE ON Employees
                    BEGIN
                        UPDATE Employees
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetEmployeeTimestampOnInsert
                    AFTER INSERT ON Employees
                    BEGIN
                        UPDATE Employees
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        }
    }
}
