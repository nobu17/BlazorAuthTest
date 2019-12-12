using BlazorAuthTest.Services.Auth;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAuthTest.ViewModel
{
    public class LoginViewModel : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected IAuthService AuthService { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsLoading { get; set; } = false;
        public LoginData LoginData { get; set; } = new LoginData();


        public async Task SubmitAsync()
        {
            try
            {
                IsLoading = true;
                var model = new LoginModel() { UserID = LoginData.UserID, Password = LoginData.Password };
                var result = await AuthService.LoginAsync(model);
                if (result.IsSuccessful)
                {
                    NavigationManager.NavigateTo("counter/");
                }
                else
                {
                    ErrorMessage = "ログインに失敗しました。";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "ログインに失敗しました。";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    public class LoginData
    {
        [Required(ErrorMessage = "ユーザIDを入力してください。")]
        [StringLength(32, ErrorMessage = "ユーザIDが長すぎます。")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "パスワードを入力してください。")]
        [StringLength(32, ErrorMessage = "パスワードが長すぎます。")]
        public string Password { get; set; }
    }
}
