using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetRecentUsersMatch_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                    migrationBuilder.Sql(@"
               CREATE PROCEDURE sPGetRecentUserMatches  
            @UserId INT,  
            @Limit INT,  
            @AcceptedStatus INT  
        AS  
        BEGIN  
            SET NOCOUNT ON;  
        
            SELECT TOP (@Limit)  
                u.Id AS UserId,  
                u.FirstName,  
                u.LastName,  
                PP.PictureUrl,   
                m.DateModified  
            FROM MatchRequests m  
            JOIN Users u  
                ON (m.MatchReceiverUserId = @UserId AND m.MatchInitiatorUserId = u.Id)  
                OR (m.MatchInitiatorUserId = @UserId AND m.MatchReceiverUserId = u.Id)  
            JOIN UserProfiles up  
                ON u.Id = up.UserId  
        	JOIN ProfilePhoto pp
        		ON up.Id = PP.UserProfileId
            WHERE m.MatchRequestStatus = @AcceptedStatus  
            ORDER BY m.DateModified DESC;  
        END;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPGetRecentUserMatches;");
        }
    }
}
