
using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Dating
        {
            public static Error DuplicateBlindDateCategory => Error.Conflict(
            code: "BlindDateCategory.AlreadyExist",
            description: "Blind Date Category already exists");

            //public static Error DuplicateCategoryQuestion => Error.Conflict(
            //code: "PromptQuestion.AlreadyExist",
            //description: "Prompt Question already exists");

            //public static Error AnswerNotProvided => Error.Validation(
            //    code: "PromptAnswer.NotProvided",
            //    description: "No prompt answer provided.");

            public static Error BlindDateCategoryNotFound => Error.NotFound(
            code: "BlindDateCategory.NotFound",
            description: "Blind Date Category not found");

            //public static Error CategoryNotFound => Error.NotFound(
            //code: "PromptCategory.NotFound",
            //description: "Prompt Category not found");


        }
    }
}
