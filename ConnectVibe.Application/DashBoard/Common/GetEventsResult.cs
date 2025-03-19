using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fliq.Application.DashBoard.Common
{
    public record GetEventsResult
    (
   string EventTitle,
    string CreatedBy,
    string Status,
    int Attendees,
    string EventCategory,
    DateTime CreatedOn
);
}