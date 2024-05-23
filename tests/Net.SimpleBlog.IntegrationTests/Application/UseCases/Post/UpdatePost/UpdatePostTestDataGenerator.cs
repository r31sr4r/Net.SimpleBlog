namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.UpdatePost;
public class UpdatePostTestDataGenerator
{
    public static IEnumerable<object[]> GetPostsToUpdate(int times = 10)
    {
        var fixture = new UpdatePostTestFixture();
        for (int indice = 0; indice < times; indice++)
        {
            var examplePost = fixture.GetValidPost();
            var exampleInput = fixture.GetValidInput(examplePost.Id, examplePost.UserId);
            yield return new object[] { examplePost, exampleInput };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int numberOfIterations = 12)
    {
        var fixture = new UpdatePostTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < numberOfIterations; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInvalidInputShortTitle(),
                    "Title should be greater than 3 characters"
                });
                    break;
                case 1:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInvalidInputTooLongTitle(),
                    "Title should be less than 255 characters"
                });
                    break;
                case 2:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInvalidInputShortContent(),
                    "Content should be greater than 3 characters"
                });
                    break;
                case 3:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInvalidInputTooLongContent(),
                    "Content should be less than 10000 characters"
                });
                    break;
            }
        }
        return invalidInputsList;
    }
}
