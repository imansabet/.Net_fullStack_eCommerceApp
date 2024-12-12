using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.Logging;
using eCommerceApp.Application.Validations;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using FluentValidation;

namespace eCommerceApp.Application.Services.Implementations.Authentication;

public class AuthenticationService
    (
    IValidator<CreateUser> createUserValidator,IValidator<LoginUser> loginUserValidator,
    IMapper mapper, IAppLogger<AuthenticationService> logger,
    IValidationService validationService,
    ITokenManagement tokenManagement, IUserManagement userManagement, IRoleManagement roleManagement
    ) : IAuthenticationService
{
    public async Task<ServiceResponse> CreateUser(CreateUser user)
    {
       var _validationResult = await validationService.ValidateAsync(user,createUserValidator);
        if (_validationResult.Success) return _validationResult;

        var mappedModel = mapper.Map<AppUser>(user);
        mappedModel.UserName = user.Email;
        mappedModel.PasswordHash = user.Password;

        var result = await userManagement.CreateUser(mappedModel);
        if (!result)
            return new ServiceResponse { Message = "Email Address Might be Already in Use Or UnKnown Error Occured" };

        var _user = await userManagement.GetUserByEmail(user.Email);
        var users = await userManagement.GetAllUsers();
        bool assignedResult = await roleManagement.AddUserToRole(_user!, users!.Count() > 1 ? "User" : "Admin");
        
        if (!assignedResult) 
        {
            // remove user
            int removeUserResult = await userManagement.RemoveUserByEmail(_user!.Email!);
            if(removeUserResult <= 0) 
            {
                logger.LogError(new Exception($"User with Email as {_user.Email} failed to be remove as a result of role assign issue . ")
                    ,"User Could not be assigned");

                return new ServiceResponse { Message = "Error Occurred When Creating Account " };
            }
        }
        return new ServiceResponse { Success = true, Message = "Account Created ! " };

    }

    public async Task<LoginResponse> LoginUser(LoginUser user)
    { 
        var _validationResult = await validationService.ValidateAsync(user, loginUserValidator);
        if (_validationResult.Success)
            return new LoginResponse(Message: _validationResult.Message);

        var mappedModel = mapper.Map<AppUser>(user);
        mappedModel.PasswordHash = user.Password;

        bool loginResult = await userManagement.LoginUser(mappedModel);
        if (!loginResult)
            return new LoginResponse(Message: "Email not found or invalid credentials");

        var _user = await userManagement.GetUserByEmail(user.Email);
        var claims = await userManagement.GetUserClaims(_user!.Email!);

        string jwtToken = tokenManagement.GenerateToken(claims);
        string refreshToken = tokenManagement.GetRefreshToken();

        int saveTokenResult = await tokenManagement.AddRefreshToken(_user.Id, refreshToken);
        return saveTokenResult <= 0 ? new LoginResponse(Message: "Internal Error Occured While Authenticating")
            : new LoginResponse(Success : true,Token: jwtToken , RefreshToken : refreshToken);


    }

    public async Task<LoginResponse> ReviveToken(string refreshToken)
    {
        bool validateTokenResult = await tokenManagement.ValidateRefreshToken(refreshToken);
        if (!validateTokenResult)
            return new LoginResponse(Message: "Invalid Token");

        string userId = await tokenManagement.GetUserIdByRefreshToken(refreshToken);
        AppUser? user = await userManagement.GetUserById(userId);
        var claims = await userManagement.GetUserClaims(user!.Email!);

        string newJwtToken = tokenManagement.GenerateToken(claims);
        string newRefreshToken = tokenManagement.GetRefreshToken();

        await tokenManagement.UpdateRefreshToken(userId, newRefreshToken);
        
        return new LoginResponse(Success: true, Token: newJwtToken, RefreshToken: newRefreshToken);

    }
}
