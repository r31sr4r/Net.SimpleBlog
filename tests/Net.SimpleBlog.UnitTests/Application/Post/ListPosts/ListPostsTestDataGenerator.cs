using Net.SimpleBlog.Application.UseCases.Post.ListPosts;

namespace Net.SimpleBlog.UnitTests.Application.Post.ListPosts;

public class ListPostsTestDataGenerator
{
    public static IEnumerable<object[]> GetInputWithoutAllParameters(int times = 18)
    {
        var fixture = new ListPostsTestFixture();
        var inputExample = fixture.GetInput();
        for (int i = 0; i < times; i++)
        {
            switch (i % 6)
            {
                case 0:
                    yield return new object[] { new ListPostsInput() };
                    break;
                case 1:
                    yield return new object[] { new ListPostsInput(inputExample.Page) };
                    break;
                case 2:
                    yield return new object[] { new ListPostsInput(
                        inputExample.Page,
                        inputExample.PerPage
                        ) };
                    break;
                case 3:
                    yield return new object[] { new ListPostsInput(
                        inputExample.Page,
                        inputExample.PerPage,
                        inputExample.Search
                        ) };
                    break;
                case 4:
                    yield return new object[] { new ListPostsInput(
                        inputExample.Page,
                        inputExample.PerPage,
                        inputExample.Search,
                        inputExample.Sort
                        ) };
                    break;
                case 5:
                    yield return new object[] { inputExample };
                    break;

                default:
                    yield return new object[] { new ListPostsInput() };
                    break;
            }
        }
    }
}
