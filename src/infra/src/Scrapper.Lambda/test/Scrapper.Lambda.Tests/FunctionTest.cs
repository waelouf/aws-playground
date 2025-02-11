using Xunit;
using Amazon.Lambda.Core;
using Moq;
using Amazon.Lambda.TestUtilities;

namespace Scrapper.Lambda.Tests;

public class FunctionTest
{
    [Fact]
    public void TestToUpperFunction()
    {
        var mockedUsersRepository = new Mock<IUsersRepository>();

        mockedUsersRepository.Setup(x => x.SaveAsync(It.IsAny<User>()));
        var function = new Function(mockedUsersRepository.Object);
        var context = new TestLambdaContext();
        var upperCase = function.FunctionHandler("hello world", context);
        mockedUsersRepository.VerifyAll();

    }
}
