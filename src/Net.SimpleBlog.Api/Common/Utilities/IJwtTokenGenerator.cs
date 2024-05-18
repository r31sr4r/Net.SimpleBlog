using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Api.Common.Utilities;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(UserModelOutput user);
}
