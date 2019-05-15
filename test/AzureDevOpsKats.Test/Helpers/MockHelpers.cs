using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;

namespace AzureDevOpsKats.Test.Helpers
{
    public static class MockHelpers
    {
        public static HttpResponseMessage SetHttpResponseMessage(HttpStatusCode statusCode, string responseContent = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = responseContent != null ? new StringContent(responseContent) : null,
            };

            if (headers != null)
            {
                foreach (var res in headers)
                {
                    httpResponseMessage.Headers.Add(res.Key, res.Value);
                }
            }

            return httpResponseMessage;
        }

        public static Mock<HttpMessageHandler> GetHttpMessageHandlerMock(HttpResponseMessage responseMessage)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage)
                .Verifiable();

            return httpMessageHandlerMock;
        }
    }
}
