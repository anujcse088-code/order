using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld;

public class Function
{
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
    {
        Console.WriteLine("Order function activated...");
        Console.WriteLine($"Order received: {apigProxyEvent.Body}");

        if (string.IsNullOrWhiteSpace(apigProxyEvent.Body))
        {
            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(new { error = "Missing order payload" }),
                StatusCode = 400,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        try
        {
            using var document = JsonDocument.Parse(apigProxyEvent.Body);
            var root = document.RootElement;

            if (!root.TryGetProperty("orderId", out var orderIdElement) || !root.TryGetProperty("orderDate", out var orderDateElement))
            {
                return new APIGatewayProxyResponse
                {
                    Body = JsonSerializer.Serialize(new { error = "Order payload must include orderId and orderDate" }),
                    StatusCode = 400,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }

            var responseBody = JsonSerializer.Serialize(new
            {
                orderId = orderIdElement.GetString(),
                orderDate = orderDateElement.GetString(),
                message = "Order processed successfully"
            });

            Console.WriteLine("Order function completed");

            return new APIGatewayProxyResponse
            {
                Body = responseBody,
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Invalid order payload: {ex.Message}");

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(new { error = "Invalid JSON payload" }),
                StatusCode = 400,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}