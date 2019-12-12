using Amazon;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlazorAuthTest.Services.Auth
{
    public class CognitoAuthService : IAuthService
    {
        private const string PoolID = "hoge";
        private const string ClientID = "hoge";

        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;

        public CognitoAuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;

            // init cognito
            var config = new AmazonCognitoIdentityProviderConfig();
            config.HttpClientFactory = new BlazorWebAssemblyHttpClientFactory(httpClient);
            config.RegionEndpoint = RegionEndpoint.APNortheast1;
            _provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), config);
            _userPool = new CognitoUserPool(PoolID, ClientID, _provider);
        }

        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            var result = new LoginResult();
            try
            {
                var user = new CognitoUser(loginModel.UserID, ClientID, _userPool, _provider);
                var authRequest = new InitiateSrpAuthRequest()
                {
                    Password = loginModel.Password
                };

                var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

                var token = string.Empty;
                // Force set the current password if the new password is required.
                if (authResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
                {
                    authResponse = await user.RespondToNewPasswordRequiredAsync(new RespondToNewPasswordRequiredRequest()
                    {
                        SessionID = authResponse.SessionID,
                        NewPassword = loginModel.Password
                    });
                    // using id token for api auth
                    token = authResponse.AuthenticationResult.IdToken;
                }
                else if (authResponse.AuthenticationResult.IdToken != null)
                {
                    token = authResponse.AuthenticationResult.IdToken;
                }
                else
                {
                    result.IsSuccessful = false;
                    result.Error = new Exception("Unexpected error");
                    return result;
                }

                result.IsSuccessful = true;
                result.IDToken = token;
                await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.UserID, result.IDToken);

                return result;
            }
            catch (NotAuthorizedException e)
            {
                Console.WriteLine("NotAuthorizedException:" + DateTime.Now.ToString());
                result.IsSuccessful = false;
                result.Error = e;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + DateTime.Now.ToString() + e.ToString());
                result.IsSuccessful = false;
                result.Error = e;
                return result;
            }
        }

        public async Task LogoutAsync()
        {
            await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
