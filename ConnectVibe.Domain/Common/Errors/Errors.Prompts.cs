
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

            public static Error PromptAlreadyApproved => Error.Conflict(
            code: "Question.QuestionAlreadyApproved;",
            description: "This Prompt Question is already Approved");

            public static Error PromptAlreadyRejected => Error.Conflict(
            code: "Question.QuestionAlreadyRejected;",
            description: "This Prompt Question is already Rejected");

            public static Error CategoryNotFound => Error.NotFound(
            code: "PromptCategory.NotFound",
            description: "Prompt Category not found");

            public static Error DuplicateCategory => Error.Conflict(
            code: "PromptCategory.AlreadyExist",
            description: "Prompt Category already exists");
            public static Error DuplicateCategoryQuestion => Error.Conflict(
            code: "PromptQuestion.AlreadyExist",
            description: "Prompt Question already exists");
        }
    }
}
