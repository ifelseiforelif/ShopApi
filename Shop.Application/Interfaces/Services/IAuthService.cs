using Shop.Application.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Services;

public interface IAuthService
{
     Task<UserReadDTO?> RegisterAsync(UserCreateDTO dto);
}
