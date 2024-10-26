using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Prompts.Common
{
    public record CreatePromptAnswerResult(
        int QuestionId,
        int AnswerId,
        bool IsAnswered
        );
}
