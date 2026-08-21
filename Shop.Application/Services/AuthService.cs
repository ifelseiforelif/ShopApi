using AutoMapper;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Services;
public class AuthService(
    IMapper _mapper, 
    IAuthRepository _repository, 
    IHashHelper _hashHelper, 
    IJWTService _jwtService, 
    IRefreshTokenRepository _refreshTokenRepository, 
    IQueueService _queueService) : IAuthService
{
    public async Task<(UserReadDTO? User, string? Token, string? RefreshToken)> RegisterAsync(UserCreateDTO dto)
    {
       
        var isExist = await _repository.IsExistEmailAsync(dto.Email);
        if(!isExist)
        {
            var hash = _hashHelper.Hash(dto.Password);
            var user = _mapper.Map<User>(dto);
            var registerUser = await _repository.RegisterUserAsync(user, hash);
            if (registerUser != null)
            {
              
                var token = _jwtService.GenerateAccessToken(_mapper.Map<UserLoginDTO>(user), user.Role.ToString());
                var refreshToken = _jwtService.GenerateRefreshToken();
                await _refreshTokenRepository.AddAsync(new RefreshToken
                {
                    Token = refreshToken.Item1,
                    UserId = user.Id,
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshToken.Item2)
                });
                await _queueService.PublishAsync<UserCreateDTO>("Users",dto);
                return (_mapper.Map<UserReadDTO>(registerUser), token, refreshToken.Item1);
            }
        }
        return (null,null,null);
    }

    public async Task<(string? Token, string? RefreshToken)> LoginAsync(UserLoginDTO dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);
        if (user == null) return (null, null);

        if (!_hashHelper.IsValidPassword(dto.Password, user.PasswordHash))
            return (null, null);
        await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id);
        var token = _jwtService.GenerateAccessToken(dto, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken.Item1,
            UserId = user.Id,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshToken.Item2)
        });

        return (token, refreshToken.Item1);
    }
}
