namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.UpdateUser;
public class UpdateUserTestDataGenerator
{
    public static IEnumerable<object[]> GetUsersToUpdate(int times = 10)
    {
        var fixture = new UpdateUserTestFixture();
        for (int indice = 0; indice < times; indice++)
        {
            var exampleUser = fixture.GetValidUser();
            var exampleInput = fixture.GetValidInput(exampleUser.Id);
            yield return new object[] { exampleUser, exampleInput };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int numberOfIterations = 12)
    {
        var fixture = new UpdateUserTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < numberOfIterations; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInvalidInputShortName(),
                    "Name should be greater than 3 characters"
                });
                    break;
                case 1:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInvalidInputTooLongName(),
                    "Name should be less than 255 characters"
                });
                    break;
                case 2:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInputWithInvalidEmail(),
                    "Email is not in a valid format"
                });
                    break;
                case 3:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInputWithInvalidCPF(),
                    "CPF is not valid"
                });
                    break;
            }
        }
        return invalidInputsList;
    }

}
