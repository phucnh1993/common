using System.Net;
using RichardSzalay.MockHttp;

namespace PhucNH.Commons.Extensions.Tests;
public class HttpExtensionTest
{
    private readonly MockHttpMessageHandler _mockHttpClient;

    public HttpExtensionTest()
    {
        _mockHttpClient = new MockHttpMessageHandler();
        _mockHttpClient.When("http://localhost/api/test/*")
            .Respond("application/json", "{'id' : '1', 'name' : 'ABC'}");
    }

    public static IEnumerable<object?[]> CallApiAsyncParam => new List<object?[]>
    {
        new object?[]
        {
            HttpMethod.Get,
            "http://localhost/api/test/case01",
            null,
            new Dictionary<string, string>(),
            new Dictionary<string, string>
            {
                {
                    "Test", "Test"
                }
            },
            null,
            HttpStatusCode.OK
        },
        new object?[]
        {
            HttpMethod.Get,
            "http://localhost/api/test/case01",
            new Dictionary<string, string>
            {
                {
                    "Test", "Test"
                }
            },
            new Dictionary<string, string>
            {
                {
                    "Test", "Test"
                }
            },
            new Dictionary<string, string>(),
            new
            {
                Id = 1,
                Name = "ABC"
            },
            HttpStatusCode.OK
        },
    };

    [Theory]
    [MemberData(nameof(CallApiAsyncParam))]
    public async void CallApiAsync_Test(
        HttpMethod method,
        string url,
        Dictionary<string, string>? headers,
        Dictionary<string, string>? queries,
        Dictionary<string, string>? formDatas,
        object body,
        HttpStatusCode status)
    {
        try
        {
            var client = new HttpClient(_mockHttpClient);
            var result = await client.CallApiAsync<object, object>(method, url, headers, queries, formDatas, body);
            var properties = result.GetType().GetProperties();
            Assert.Equal(status, result.Item1);
            foreach (var property in properties)
            {
                var resultValue = property.GetValue(result)?.ToString();
                Assert.NotEmpty(resultValue!);
            }
        }
        catch (Exception ex)
        {
            Assert.NotNull(ex);
        }
    }

    [Theory]
    [MemberData(nameof(CallApiAsyncParam))]
    public async void CallApiAsyncException_Test(
        HttpMethod method,
        string url,
        Dictionary<string, string>? headers,
        Dictionary<string, string>? queries,
        Dictionary<string, string>? formDatas,
        object body,
        HttpStatusCode status)
    {
        var client = new HttpClient();
        try
        {
            var result = await client.CallApiAsync<object, object>(method, url, headers, queries, formDatas, body);
            var properties = result.GetType().GetProperties();
            Assert.Equal(status, result.Item1);
            foreach (var property in properties)
            {
                var resultValue = property.GetValue(result)?.ToString();
                Assert.NotEmpty(resultValue!);
            }
        }
        catch (Exception ex)
        {
            Assert.NotNull(ex.Message);
        }
    }

    [Fact]
    [Obsolete]
    public async void CallApiAsyncNotOk_Test()
    {
        var mockHttpClient = new MockHttpMessageHandler();
        mockHttpClient.When("http://localhost/api/test/*")
            .Respond(HttpStatusCode.BadGateway);
        var client = new HttpClient(mockHttpClient);
        var result = await client.CallApiAsync<object, object>(HttpMethod.Get, "http://localhost/api/test/case01");
        Assert.Equal(HttpStatusCode.BadGateway, result.Item1);
        Assert.NotNull(result.Item2);

        var mockHttpClient2 = new MockHttpMessageHandler();
        using (var response = new HttpResponseMessage(HttpStatusCode.OK))
        {
            mockHttpClient2.When("http://localhost/api/test/*").Respond(response);
            var client2 = new HttpClient(mockHttpClient2);
            var result2 = await client2.CallApiAsync<object, object>(HttpMethod.Get, "http://localhost/api/test/case01");
            Assert.Equal(HttpStatusCode.OK, result2.Item1);
            Assert.NotNull(result2.Item2);
        }

    }
}