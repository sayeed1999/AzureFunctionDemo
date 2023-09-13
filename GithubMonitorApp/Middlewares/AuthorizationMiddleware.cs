using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace GithubMonitorApp.Middlewares
{
    public class AuthorizationMiddleware : IFunctionsWorkerMiddleware
    {
        public const string HeaderAuthorization = "Authorization";

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();

            try
            {
                // api creds from azure ad b2c api registration
                var instance = "https://sayeed1999azureadb2c.b2clogin.com";
                var domain = "sayeed1999azureadb2c.onmicrosoft.com";
                var tenantID = "5f7ed123-a70a-4108-a62c-24492e73a881";
                var clientID = "b5581784-f829-412e-adec-c3511f54ae2c";

                var token = GetAuthorizationToken(request);

                // This is the 'Azure AD B2C OpenID Connect metadata document' endpoint from azure ad b2c app regs
                var stsDiscoveryEndpoint = $"{instance}/{domain}/b2c_1_signup_signin/v2.0/.well-known/openid-configuration";

                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());

                var config = configManager.GetConfigurationAsync().Result;

                var validationParameters = new TokenValidationParameters
                {
                    //decode the JWT to see what these values should be
                    ValidAudience = clientID,
                    ValidIssuer = $"{instance}/{tenantID}/v2.0/",
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    IssuerSigningKeys = config.SigningKeys,
                    ValidateLifetime = true,
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, validationParameters, out var jwt);

                await next(context);
            }
            catch (Exception)
            {
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                await response.WriteStringAsync("You are not authorized to access this resource.");
                context.GetInvocationResult().Value = response;
            }
        }

        private string GetAuthorizationToken(Microsoft.Azure.Functions.Worker.Http.HttpRequestData request)
        {
            request.Headers.TryGetValues(HeaderAuthorization, values: out var tokens);
            string token = tokens?.FirstOrDefault();
            token = token.Split(' ')[1];
            return token;
        }
    }
}
