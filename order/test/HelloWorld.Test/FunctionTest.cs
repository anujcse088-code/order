using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

namespace HelloWorld.Tests;

public class FunctionTest
{
  [Fact]
  public async Task ReturnsOrderDetailsFromPayload()
  {
    var request = new APIGatewayProxyRequest
    {
      Body = JsonSerializer.Serialize(new
      {
        orderId = "12345",
        orderDate = "2026-01-28"
      })
    };

    var context = new TestLambdaContext();

    var expectedBody = JsonSerializer.Serialize(new
    {
      orderId = "12345",
      orderDate = "2026-01-28"
    });

    var function = new Function();
    var response = await function.FunctionHandler(request, context);

    Assert.Equal(200, response.StatusCode);
    Assert.Equal(expectedBody, response.Body);
    Assert.Equal("application/json", response.Headers["Content-Type"]);
  }
}