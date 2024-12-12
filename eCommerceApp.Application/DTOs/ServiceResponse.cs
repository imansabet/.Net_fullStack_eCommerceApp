namespace eCommerceApp.Application.DTOs;

public record ServiceResponse(bool Success = false, string Message = "No message provided");
