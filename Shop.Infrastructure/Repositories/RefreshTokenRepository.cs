using Microsoft.Identity.Client;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Repositories;

internal class RefreshTokenRepository(ShopDbContext _context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(RefreshToken refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetByTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(RefreshToken refreshToken)
    {
        throw new NotImplementedException();
    }
}
