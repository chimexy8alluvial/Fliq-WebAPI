using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateToGetUsersWIthLatestSub_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
               ALTER PROCEDURE sp_GetUsersWithLatestSubscription
                 @RoleId INT,
                 @Offset INT,
                @Fetch INT
            AS
            BEGIN
                SELECT 
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    ISNULL(s.ProductId, 'No Subscription') AS SubscriptionType,
                    u.DateCreated AS DateCreated,
                    u.LastActiveAt AS LastActive
                FROM Users u
                LEFT JOIN (
                    SELECT s1.UserId, s1.ProductId
                    FROM Subscriptions s1
                    WHERE s1.StartDate = (
                        SELECT MAX(s2.StartDate)
                        FROM Subscriptions s2
                        WHERE s2.UserId = s1.UserId
                    )
                ) s ON u.Id = s.UserId
                WHERE u.RoleId = @RoleId
                ORDER BY u.DateCreated DESC
                OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY;
            END;"
               );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetUsersWithLatestSubscription;");
        }
    }
}
