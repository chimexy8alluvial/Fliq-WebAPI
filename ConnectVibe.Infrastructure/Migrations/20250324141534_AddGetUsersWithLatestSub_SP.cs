using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetUsersWithLatestSub_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetUsersWithLatestSubscription
                    @RoleId INT
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
                    WHERE u.RoleId = @RoleId;
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
