using BaseApi.Domain.Entities;

namespace BaseApi.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
