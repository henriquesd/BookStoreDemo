using BookStore.API.Dtos;

namespace BookStore.API.Mappings
{
    public static class OperationResultExtensions
    {
        public static IActionResult ToActionResult<T>(this IOperationResult<T> result, Func<T, object>? mapper = null)
        {
            if (result.Success && result.Payload != null)
            {
                var payload = mapper != null ? mapper(result.Payload) : result.Payload;
                return new OkObjectResult(payload);
            }

            return ToErrorResult(result);
        }

        public static IActionResult ToActionResult(this IOperationResult<bool> result)
        {
            if (result.Success)
            {
                return new NoContentResult();
            }

            return ToErrorResult(result);
        }

        public static IActionResult ToActionResult(this IOperationResult result)
        {
            if (result.Success)
            {
                return new NoContentResult();
            }

            return ToErrorResult(result);
        }

        private static IActionResult ToErrorResult(IOperationResult result)
        {
            var errorResponse = new ErrorResponse(result.Message ?? "An error occurred");

            return result.ErrorCode switch
            {
                OperationErrorCode.NotFound => new NotFoundObjectResult(errorResponse),
                OperationErrorCode.Duplicate => new ConflictObjectResult(errorResponse),
                OperationErrorCode.HasDependencies => new ConflictObjectResult(errorResponse),
                OperationErrorCode.ValidationError => new BadRequestObjectResult(errorResponse),
                OperationErrorCode.UnexpectedError => new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError },
                _ => new BadRequestObjectResult(errorResponse)
            };
        }
    }
}
