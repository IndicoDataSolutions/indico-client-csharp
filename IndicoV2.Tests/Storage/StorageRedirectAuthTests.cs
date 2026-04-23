using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace IndicoV2.Tests.Storage
{
    [TestFixture]
    public class StorageRedirectAuthTests
    {
        private sealed class LocalHttpServer : IAsyncDisposable
        {
            private readonly HttpListener _listener;
            private readonly Func<HttpListenerContext, Task> _handler;
            private readonly Task _loopTask;

            public Uri BaseUri { get; }

            public LocalHttpServer(Func<HttpListenerContext, Task> handler, string host = "127.0.0.1")
            {
                _handler = handler;
                var port = GetFreeTcpPort();
                BaseUri = new Uri($"http://{host}:{port}/");
                _listener = new HttpListener();
                _listener.Prefixes.Add(BaseUri.ToString());
                _listener.Start();
                _loopTask = Task.Run(LoopAsync);
            }

            private async Task LoopAsync()
            {
                while (_listener.IsListening)
                {
                    HttpListenerContext context;
                    try
                    {
                        context = await _listener.GetContextAsync();
                    }
                    catch (HttpListenerException)
                    {
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }

                    await _handler(context);
                }
            }

            public async ValueTask DisposeAsync()
            {
                _listener.Stop();
                _listener.Close();
                await _loopTask;
            }

            private static int GetFreeTcpPort()
            {
                using var listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                var port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                return port;
            }
        }

        private static async Task WriteJson(HttpListenerResponse response, string payload, int statusCode = 200)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            var bytes = Encoding.UTF8.GetBytes(payload);
            response.ContentLength64 = bytes.Length;
            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            response.Close();
        }

        [Test]
        public async Task HttpClient_RefreshesTokenAfter401_AndRetries()
        {
            var seenPaths = new ConcurrentQueue<string>();
            var seenAuth = new ConcurrentQueue<string>();
            var storageAttempts = 0;

            await using var server = new LocalHttpServer(async context =>
            {
                var path = context.Request.Url?.AbsolutePath ?? "";
                seenPaths.Enqueue(path);
                seenAuth.Enqueue(context.Request.Headers["Authorization"] ?? "");

                if (path == "/auth/users/refresh_token")
                {
                    await WriteJson(context.Response, "{\"auth_token\":\"short-lived\"}");
                    return;
                }

                if (path == "/storage/auth-retry")
                {
                    storageAttempts++;
                    if (storageAttempts == 1)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.Close();
                        return;
                    }

                    await WriteJson(context.Response, "{\"ok\":true}");
                    return;
                }

                context.Response.StatusCode = 404;
                context.Response.Close();
            }, host: "localhost");

            var client = new IndicoClient("refresh-token", server.BaseUri);
            var response = await client.HttpClient.GetAsync(new Uri(server.BaseUri, "/storage/auth-retry"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            storageAttempts.Should().Be(2);
            seenPaths.Should().Contain("/auth/users/refresh_token");
            seenAuth.Should().Contain("Bearer refresh-token");
            seenAuth.Should().Contain("Bearer short-lived");
        }

        [Test]
        public async Task HttpClient_FollowsCrossHostRedirect_WithoutForwardingAuthHeader()
        {
            string redirectedAuthHeader = null;

            await using var signedServer = new LocalHttpServer(async context =>
            {
                redirectedAuthHeader = context.Request.Headers["Authorization"];
                await WriteJson(context.Response, "{\"ok\":true}");
            }, host: "127.0.0.1");

            await using var appServer = new LocalHttpServer(async context =>
            {
                var path = context.Request.Url?.AbsolutePath ?? "";
                if (path == "/storage/redirect-me")
                {
                    context.Response.StatusCode = 302;
                    context.Response.RedirectLocation = new Uri(signedServer.BaseUri, "/signed/blob").ToString();
                    context.Response.Close();
                    return;
                }

                context.Response.StatusCode = 404;
                context.Response.Close();
            }, host: "localhost");

            var client = new IndicoClient("refresh-token", appServer.BaseUri);
            var response = await client.HttpClient.GetAsync(new Uri(appServer.BaseUri, "/storage/redirect-me"));
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            body.Should().Contain("\"ok\":true");
            redirectedAuthHeader.Should().BeNullOrEmpty();
        }
    }
}
