﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace BlazorAuthTest.Services.Auth
{
    public class DummyAuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public DummyAuthService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            // wait 3 seconds
            await Task.Delay(3000);
            if (loginModel == null)
            {
                throw new ArgumentException("loginModel is null");
            }
            if (loginModel.UserID == "demo" && loginModel.Password == "demo")
            {
                var res = new LoginResult()
                {
                    IsSuccessful = true,
                    IDToken = "hoge"
                };
                var roles = new List<string>();
                roles.Add("Admin");
                await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.UserID, res.IDToken, roles);
                return res;
            }
            else if (loginModel.UserID == "demo2" && loginModel.Password == "demo2")
            {
                var res = new LoginResult()
                {
                    IsSuccessful = true,
                    IDToken = "hoge"
                };
                var roles = new List<string>();
                roles.Add("User");
                await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.UserID, res.IDToken, roles);
                return res;
            }
            else
            {
                return new LoginResult()
                {
                    IsSuccessful = false,
                    Error = new AuthenticationException("NotAuthrized")
                };
            }
        }

        public async Task LogoutAsync()
        {
            await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
