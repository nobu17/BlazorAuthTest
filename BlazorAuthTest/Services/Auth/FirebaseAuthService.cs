using Firebase.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAuthTest.Services.Auth
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public FirebaseAuthService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var provider = new FirebaseAuthProvider(new FirebaseConfig("hoge"));
                var result = await provider.SignInWithEmailAndPasswordAsync(loginModel.UserID, loginModel.Password);
                var res = new LoginResult()
                {
                    IsSuccessful = true,
                    IDToken = result.FirebaseToken
                };
                await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.UserID, res.IDToken);
                return res;
            }
            catch (FirebaseAuthException e)
            {
                return new LoginResult()
                {
                    IsSuccessful = false,
                    Error = e
                };
            }
            catch (Exception e)
            {
                return new LoginResult()
                {
                    IsSuccessful = false,
                    Error = e
                };
            }
        }

        public async Task LogoutAsync()
        {
            await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
