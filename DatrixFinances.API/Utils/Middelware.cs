using System.Diagnostics;
using System.Text;
using DatrixFinances.API.Models;
using DatrixFinances.API.Models.DTO;
using DatrixFinances.API.Repositories;
using DatrixFinances.API.Services;

namespace DatrixFinances.API.Utils;

public class Middleware(RequestDelegate next, IServiceProvider serviceProvider, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IConfiguration _configuration = configuration;

    private Stopwatch _stopwatch = new();
    private DateTimeOffset _currentDateTimeOffset = new();

    public async Task InvokeAsync(HttpContext context)
    {
        _stopwatch = Stopwatch.StartNew();
        _currentDateTimeOffset = DateTimeOffset.Now;

        var flag = Flag.Unknown;

        var path = context.Request.Path.Value!;

        if (path.StartsWith("/images") || path.StartsWith("/css") || path.StartsWith("/_content") || path.StartsWith("/_framework") || path.StartsWith("/_blazor"))
        {
            await _next(context);
            return;
        }

        if (context.Session.GetString("Swagger-Authentication-Login-Token-24fdsjq_1!-32421:AdFGQdjvDSQEFkmqfQGQZEDfgmgqdf$ùsdf==") == "true" && (path.Equals("/") || path.Equals("/index.html") || path.Equals("/login")))
        {
            context.Response.Redirect("/portal");
            return;
        }

        if (context.Session.GetString("Swagger-Authentication-Login-Token-24fdsjq_1!-32421:AdFGQdjvDSQEFkmqfQGQZEDfgmgqdf$ùsdf==") != "true" && (path.Equals("/") || path.StartsWith("/swagger") || path.StartsWith("/portal")))
        {
            context.Response.Redirect("/login");
            return;
        }

        if ((path.StartsWith("/swagger") && !path.Equals("/swagger/index.html", StringComparison.OrdinalIgnoreCase)) || path.Equals("/login") || path.EndsWith(".ico") || path.StartsWith("/portal"))
        {
            await _next(context);
            return;
        }

        if (path.Equals("/webhook/maventa/notifications/catch", StringComparison.OrdinalIgnoreCase))
        {
            var notificationOriginalBodyStream = context.Response.Body;
            using var notificationResponseBody = new MemoryStream();
            context.Response.Body = notificationResponseBody;

            var (notificationBase64, notificationContentType) = await GetRequestBodyInfoAsync(context);

            await _next(context);
            if (context.Connection.RemoteIpAddress?.MapToIPv4().ToString() == _configuration["Authentication:Trusted_ip_addresses:Maventa_webhook_sender_ip_address"])
                flag = Flag.TrustedRequest;
            else
                flag = Flag.Malicious;
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAPIActivityAspectRepository>();
            var (notificationBase64Response, notificationContentTypeResponse) = await GetResponseBodyInfoAsync(context, notificationResponseBody, notificationOriginalBodyStream);
            await repo.Add(CreateAPIActivityObject(_currentDateTimeOffset, context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "?", context.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "?", context.Request.Headers["Datrix-origin-platform"].FirstOrDefault() ?? "?", context.Request.Headers["Datrix-origin-company"].FirstOrDefault() ?? "?", context.Request.Headers["Datrix-origin-platform-signed-user"].FirstOrDefault() ?? "?", context.Response.StatusCode, ((System.Net.HttpStatusCode)context.Response.StatusCode).ToString(), context.Request.Path, context.Request.Method, context.GetEndpoint()?.DisplayName ?? "?", _stopwatch.ElapsedMilliseconds, flag, notificationBase64, notificationContentType, notificationBase64Response, notificationContentTypeResponse));
            return;
        }

        if (!path.Equals("/swagger/index.html", StringComparison.OrdinalIgnoreCase))
        {
            if (context.Request.Headers["Datrix-origin-platform"].Count == 0
                || context.Request.Headers["Datrix-origin-platform-signed-user"].Count == 0
                || context.Request.Headers["Datrix-origin-company"].Count == 0)
            {
                Console.WriteLine($"Missing Datrix-origin headers {string.Join(", ", context.Request.Headers.Where(h => h.Key.StartsWith("Datrix-origin")).Select(h => h.Key))}");
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Access Denied!");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IAPIActivityAspectRepository>();
                    var (maliciousBase64, maliciousContentType) = await GetRequestBodyInfoAsync(context);
                    await repo.Add(CreateAPIActivityObject(_currentDateTimeOffset, context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "?", context.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "?", context.Request.Headers["Datrix-origin-platform"].FirstOrDefault() ?? "?", context.Request.Headers["Datrix-origin-company"].FirstOrDefault() ?? "?", context.Request.Headers["Datrix-origin-platform-signed-user"].FirstOrDefault() ?? "?", context.Response.StatusCode, ((System.Net.HttpStatusCode)context.Response.StatusCode).ToString(), context.Request.Path, context.Request.Method, context.GetEndpoint()?.DisplayName ?? "?", _stopwatch.ElapsedMilliseconds, Flag.Malicious, maliciousBase64, maliciousContentType, Convert.ToBase64String(Encoding.UTF8.GetBytes("Access Denied!")), "text/plain"));
                }
                _stopwatch.Stop();
                return;
            }
        }
        else
        {
            context.Request.Headers["Datrix-origin-platform"] = "Website visit";
            context.Request.Headers["Datrix-origin-platform-signed-user"] = "?";
            context.Request.Headers["Datrix-origin-company"] = "?";
        }

        if (context.Request.Headers["Datrix-origin-platform"].FirstOrDefault() != "?"
                && context.Request.Headers["Datrix-origin-platform-signed-user"].FirstOrDefault() != "?"
                && context.Request.Headers["Datrix-origin-company"].FirstOrDefault() != "?")
        {
            flag = Flag.TrustedRequest;
        }
        else
        {
            flag = Flag.PotentialMalicious;
        }

        // OLD CODE USED TO GET MAVENTA CREDENTIALS BEFORE THE ACTUAL CONTROLLER IS HIT
        //
        // if (context.GetEndpoint()?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>() != null)
        // {
        //     var tokenHeader = context.Request.Headers["Authentication-Keys"].FirstOrDefault();

        //     if (string.IsNullOrEmpty(tokenHeader))
        //     {
        //         context.Response.StatusCode = 401;
        //         Console.WriteLine("Missing Authentication-Keys header");
        //         return;
        //     }

        //     var parts = tokenHeader.Split(':');
        //     if (parts.Length != 3)
        //     {
        //         context.Response.StatusCode = 400;
        //         Console.WriteLine("Invalid Authentication-Keys header format, expected format: clientId:clientSecret:vendorApiKey, received: " + tokenHeader);
        //         return;
        //     }

        //     var clientId = parts[0];
        //     var clientSecret = parts[1];
        //     var vendorApiKey = parts[2];

        //     using var scope = _serviceProvider.CreateScope();
        //     var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        //     var bearerToken = await authService.MaventaTokenRequest(clientId, clientSecret, vendorApiKey);

        //     if (bearerToken == null || string.IsNullOrEmpty(bearerToken))
        //     {
        //         context.Response.StatusCode = 400;
        //         Console.WriteLine("Failed to obtain bearer token from Maventa.");
        //         return;
        //     }

        //     context.Request.Headers.Authorization = $"Bearer {bearerToken}";
        // }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        var (requestTextBase64, requestContentType) = await GetRequestBodyInfoAsync(context);

        try
        {
             await _next(context);
        } 
        catch (Exception)
        {
            context.Response.StatusCode = 500;
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IAPIActivityAspectRepository>();

            var (responseTextBase64, responseContentType) = await GetResponseBodyInfoAsync(context, responseBody, originalBodyStream);

            await repo.Add(CreateAPIActivityObject(_currentDateTimeOffset, context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "?", context.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "?", context.Request.Headers["Datrix-origin-platform"].FirstOrDefault()!, context.Request.Headers["Datrix-origin-company"].FirstOrDefault()!, context.Request.Headers["Datrix-origin-platform-signed-user"].FirstOrDefault()!, context.Response.StatusCode, ((System.Net.HttpStatusCode)context.Response.StatusCode).ToString(), context.Request.Path, context.Request.Method, context.GetEndpoint()?.DisplayName ?? "?", _stopwatch.ElapsedMilliseconds, flag, requestTextBase64, requestContentType!, responseTextBase64, responseContentType!));
        }

        _stopwatch.Stop();
    }

    public APIActivityAspect CreateAPIActivityObject(DateTimeOffset dateTimeOffset, string ipv4, string ipv6, string originPlatform, string originCompany,
        string signedInCompanyUser, int statusCode, string statusCodeName, string httpMethod, string controllerMethod,
        string path, double elapsedMilliseconds, Flag flag, string sentPayload = "", string sentPayloadContentType = "", string receivedPayload = "", string receivedPayloadContentType = "")
    {
        return new APIActivityAspect
        {
            DateTimeOffset = dateTimeOffset,
            IPv4 = ipv4,
            IPv6 = ipv6,
            OriginPlatform = originPlatform,
            OriginCompany = originCompany,
            SignedInCompanyUser = signedInCompanyUser,
            StatusCode = statusCode,
            StatusCodeName = statusCodeName,
            HttpMethod = httpMethod,
            ControllerMethod = controllerMethod,
            Path = path,
            ElapsedMilliseconds = elapsedMilliseconds,
            Flag = flag.ToString(),
            SentPayload = sentPayload,
            SentPayloadContentType = sentPayloadContentType,
            ReceivedPayload = receivedPayload,
            ReceivedPayloadContentType = receivedPayloadContentType
        };
    }

    public async Task<(string ResponseBase64, string ResponseContentType)> GetResponseBodyInfoAsync(HttpContext context, MemoryStream responseBody, Stream originalStream)
    {
        string responseText;
        try
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(responseBody, Encoding.UTF8, false, leaveOpen: true))
            {
                responseText = await reader.ReadToEndAsync();
            }
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseText));
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalStream);
        }
        finally
        {
            context.Response.Body = originalStream;
        }

        string responseContentType = context.Response.ContentType ?? string.Empty;

        string responseBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseText));
        return (responseBase64, responseContentType);
    }

    public static async Task<(string RequestBase64, string RequestContentType)> GetRequestBodyInfoAsync(HttpContext context)
    {
        string requestText = string.Empty;

        context.Request.EnableBuffering();

        if (context.Request.ContentLength.GetValueOrDefault() > 0 && context.Request.Body.CanSeek)
        {
            StreamReader reader = null!;
            try
            {
                reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                requestText = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0;
            }
            finally
            {
                reader!.Dispose();
            }
        }

        string requestBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestText));
        string requestContentType = context.Request.ContentType ?? string.Empty;

        return (requestBase64, requestContentType);
    }
}