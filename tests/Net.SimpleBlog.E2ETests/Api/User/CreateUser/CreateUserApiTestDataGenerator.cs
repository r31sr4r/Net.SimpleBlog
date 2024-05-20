namespace Net.SimpleBlog.E2ETests.Api.User.CreateUser;
public class UpdateUserApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateUserApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetInput();
                    input1.Name = fixture.GetInvalidShortName();
                    invalidInputsList.Add(new object[]
                    {
                        input1,
                        "Name should be greater than 3 characters"
                    });
                    break;
                case 1:
                    var input2 = fixture.GetInput();
                    input2.Name = fixture.GetInvalidTooLongName();
                    invalidInputsList.Add(new object[]
                    {
                        input2,
                        "Name should be less than 255 characters"
                    });
                    break;
                case 2:
                    var input3 = fixture.GetInput();
                    input3.Email = fixture.GetInvalidEmail();
                    invalidInputsList.Add(new object[]
                    {
                        input3,
                        "Email is not in a valid format"
                    });
                    break;
                case 3:
                    var input4 = fixture.GetInput();
                    input4.CPF = fixture.GetInvalidCPF();
                    invalidInputsList.Add(new object[]
                        {
                        input4,
                        "CPF is not valid"
                    });
                    break;
            }
        }
        return invalidInputsList;
    }
}
