using ConnectVibe.Api.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConnectVibe.Api.Controllers
{

    [ApiController]
    [Authorize]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult Problem(List<Error> errors)
        {
            if (errors.Count is 0)
            {
                return Problem();
            }
            if (errors.All(error => error.Type == ErrorType.Validation))
            {
                return ValidationProblem(errors);

            }
            HttpContext.Items[HttpContextItemKeys.Errors] = errors;
            var firstError = errors[0];
            return Problem(firstError);


        }
        private IActionResult Problem(Error error)
        {

            var statusCode = error.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError

            };
            return base.Problem(statusCode: statusCode, title: error.Description);
        }
        private IActionResult ValidationProblem(List<Error> errors)
        {
            var errorDetails = new ValidationProblemDetails();

            foreach (var error in errors)
            {
                if (errorDetails.Errors.ContainsKey(error.Code))
                {
                    var existingErrors = errorDetails.Errors[error.Code];
                    var updatedErrors = existingErrors.Concat(new[] { error.Description }).ToArray();
                    errorDetails.Errors[error.Code] = updatedErrors;
                }
                else
                {
                    errorDetails.Errors.Add(error.Code, new[] { error.Description });
                }
            }

            errorDetails.Status = StatusCodes.Status400BadRequest;
            errorDetails.Title = "One or more validation errors occurred.";

            return new ObjectResult(errorDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}
