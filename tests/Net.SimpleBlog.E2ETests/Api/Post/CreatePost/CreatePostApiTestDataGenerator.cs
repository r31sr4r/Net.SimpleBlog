namespace Net.SimpleBlog.E2ETests.Api.Post.CreatePost;
public class CreatePostApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreatePostApiTestFixture();
        fixture.Authenticate().GetAwaiter().GetResult();

        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetInput();
                    input1.Title = fixture.GetInvalidShortTitle();
                    invalidInputsList.Add(new object[]
                    {
                        input1,
                        "Title should be greater than 3 characters"
                    });
                    break;
                case 1:
                    var input2 = fixture.GetInput();
                    input2.Title = fixture.GetInvalidTooLongTitle();
                    invalidInputsList.Add(new object[]
                    {
                        input2,
                        "Title should be less than 255 characters"
                    });
                    break;
                case 2:
                    var input3 = fixture.GetInput();
                    input3.Content = fixture.GetInvalidShortContent();
                    invalidInputsList.Add(new object[]
                    {
                        input3,
                        "Content should be greater than 3 characters"
                    });
                    break;
                case 3:
                    var input4 = fixture.GetInput();
                    input4.Content = fixture.GetInvalidTooLongContent();
                    invalidInputsList.Add(new object[]
                        {
                        input4,
                        "Content should be less than 10000 characters"
                    });
                    break;
            }
        }
        return invalidInputsList;
    }
}
