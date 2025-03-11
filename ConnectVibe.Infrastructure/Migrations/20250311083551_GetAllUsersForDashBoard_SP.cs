using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllUsersForDashBoard_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetAllUsersForDashBoard
                     @PageNumber INT,
                      @PageSize INT  
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
                    u.DisplayName,
                    u.Email,
                    COALESCE(s.ProductId, 'None') AS SubscriptionType,
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
                WHERE 
                    u.IsDeleted = 0
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
