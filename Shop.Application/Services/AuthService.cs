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

public class AuthService(IMapper _mapper, IAuthRepository _repository, IHashHelper _hashHelper) : IAuthService
{
    public async Task<UserReadDTO?> RegisterAsync(UserCreateDTO dto)
    {
       
        var isExist = await _repository.IsExistEmailAsync(dto.Email);
        if(!isExist)
        {
            var hash = _hashHelper.Hash(dto.Password);
            var user = _mapper.Map<User>(dto);
            return _mapper.Map<UserReadDTO>(await _repository.RegisterUserAsync(user,hash));
        }
        return null;
    }
}
