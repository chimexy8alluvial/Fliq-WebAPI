
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Prompts
        {
            public static Error AnswerNotProvided => Error.Validation(
                code: "PromptAnswer.NotProvided",
                description: "No prompt answer provided.");

            public static Error QuestionNotFound => Error.NotFound(
            code: "PromptQuestion.NotFound",
            description: "Prompt Question not found");

            public static Error CategoryNotFound => Error.NotFound(
            code: "PromptCategory.NotFound",
            description: "Prompt Category not found");
        }
    }
}
