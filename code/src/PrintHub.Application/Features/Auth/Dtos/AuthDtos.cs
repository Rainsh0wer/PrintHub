namespace PrintHub.Application.Features.Auth.Dtos;

public record RegisterRequest(
    string FullName,
    string Email,
    string? PhoneNumber,
    string Password,
    string ConfirmPassword);

public record LoginRequest(string Email, string Password);

public record RefreshRequest(string RefreshToken);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Token, string NewPassword);

/// <summary>Generic message; ResetToken is included only in this dev build (would be emailed).</summary>
public record ForgotPasswordResponse(string Message, string? ResetToken);

/// <summary>Public view of a user account. Never carries the password hash.</summary>
public record UserDto(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string Role,
    decimal WalletBalance,
    string? DefaultAddress);

/// <summary>Returned on successful register/login/refresh.</summary>
public record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    UserDto User);
