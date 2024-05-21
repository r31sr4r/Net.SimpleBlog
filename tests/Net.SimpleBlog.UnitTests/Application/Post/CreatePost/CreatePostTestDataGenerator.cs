using Net.SimpleBlog.Application.UseCases.Post.CreatePost;

namespace Net.SimpleBlog.UnitTests.Application.Post.CreatePost;
public class CreatePostTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int numberOfIterations = 6)
    {
        var fixture = new CreatePostTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 2;

        for (int index = 0; index < numberOfIterations; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInputWithInvalidTitle(),
                        "Title should be greater than 3 characters"
                    });
                    break;
                case 1:
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInputWithInvalidContent(),
                        "Content should not be empty or null"
                    });
                    break;
            }
        }
        return invalidInputsList;
    }
}
