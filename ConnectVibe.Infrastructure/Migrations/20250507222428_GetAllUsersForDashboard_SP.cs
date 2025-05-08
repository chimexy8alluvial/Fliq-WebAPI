using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllUsersForDashboard_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    CREATE  PROCEDURE sp_GetAllUsersForDashBoard
             @PageNumber INT,
              @PageSize INT  ,
               @HasSubscription BIT = NULL,      
               @ActiveSince DATETIME = NULL,     
               @RoleName NVARCHAR(50) = NULL         
    AS
    BEGIN
             SET NOCOUNT ON;
              
                 -- Validate parameters
            IF @PageNumber < 1
            BEGIN
                RAISERROR ('PageNumber must be greater than or equal to 1.', 16, 1);
                RETURN;
            END

            IF @PageSize < 1
            BEGIN
                RAISERROR ('PageSize must be greater than or equal to 1.', 16, 1);
                RETURN;
            END

        SELECT 
            u.FirstName + ' ' + u.LastName AS DisplayName,
            u.Email,
            COALESCE(s.ProductId, 'Free') AS SubscriptionType,
            u.DateCreated AS DateJoined,
            u.LastActiveAt AS LastOnline
        FROM 
            Users u
        LEFT JOIN 
            Subscriptions s ON u.Id = s.UserId 
            AND s.StartDate = (
                SELECT MAX(s2.StartDate)
                FROM Subscriptions s2
                WHERE s2.UserId = u.Id
            )
        LEFT JOIN 
            Roles r ON u.RoleId = r.Id

        WHERE 
            u.IsDeleted = 0
            AND (@HasSubscription IS NULL OR 
                 (@HasSubscription = 1 AND s.ProductId IS NOT NULL) OR 
                 (@HasSubscription = 0 AND s.ProductId IS NULL))
            AND (@ActiveSince IS NULL OR u.LastActiveAt >= @ActiveSince)
            AND (@RoleName IS NULL OR r.Name = @RoleName)
        ORDER BY 
            u.DateCreated DESC
        OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    END;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllUsersForDashBoard");
        }
    }
}
