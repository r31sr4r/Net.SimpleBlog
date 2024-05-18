using Net.SimpleBlog.Domain.Common.Security;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Common.Security;
public class PasswordHasherTest
{
    [Fact(DisplayName = nameof(ShouldGenerateHash_ForGivenPassword))]
    [Trait("Common/Security", "PasswordHasher")]
    public void ShouldGenerateHash_ForGivenPassword()
    {
        var password = "TestPassword123!";

        var hashedPassword = PasswordHasher.HashPassword(password);

        Assert.NotEmpty(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
    }

    [Theory(DisplayName = nameof(ShouldValidatePasswordCorrectly))]
    [Trait("Common/Security", "PasswordHasher")]
    [InlineData("MySecurePassword!", true)]
    [InlineData("WrongPassword", false)]
    public void ShouldValidatePasswordCorrectly(string testPassword, bool expectedResult)
    {
        var originalPassword = "MySecurePassword!";
        var hashedPassword = PasswordHasher.HashPassword(originalPassword);

        var result = PasswordHasher.VerifyPasswordHash(testPassword, hashedPassword);

        Assert.Equal(expectedResult, result);
    }
}

