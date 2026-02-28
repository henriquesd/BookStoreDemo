namespace BookStore.API.Dtos
{
    public record ErrorResponse(
        string Message,
        string? Detail = null,
        Dictionary<string, string[]>? Errors = null);
}
