using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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

        public static ControllerContext GetHttpContext()
        {
            var response = new Mock<HttpResponse>();
            response.Setup(x => x.Headers).Returns(new Mock<IHeaderDictionary>().Object);

            // Http Request (Headers)
            var request = new Mock<HttpRequest>();
            var headers = new HeaderDictionary(new Dictionary<string, StringValues>
            {
                { "Content-Type", "application/xml; charset=utf-8"},
                { "Host", "mock.localhost:6500"},
            }) as IHeaderDictionary;
            request.SetupGet(r => r.Headers).Returns(headers);
            request.SetupGet(r => r.HttpContext.TraceIdentifier).Returns(Guid.NewGuid().ToString);

            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(x => x.Response).Returns(response.Object);
            contextMock.SetupGet(x => x.Request).Returns(request.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            var controllerContext = controllerContextMock.Object;
            controllerContext.HttpContext = contextMock.Object;

            return controllerContext;
        }
    }
}
