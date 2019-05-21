using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChuckNorrisFacts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public event EventHandler<bool> LoginChanged;

        private async void LoginClicked(object sender, EventArgs e)
        {
            var loginProvider = DependencyService.Get<ILoginProvider>();
            var authInfo = await loginProvider.LoginAsync();

            var success = string.IsNullOrWhiteSpace(authInfo.AccessToken) || authInfo.IsAuthorized;

            if (LoginChanged != null) LoginChanged(this, success);
            if (!success)
            {
                ErrorLabel.Text = "Login failed.";
                return;
            }

            LoginPanel.IsVisible = false;
            LogoutPanel.IsVisible = true;
            ErrorLabel.Text = "";

            //TODO: Save the access and refresh tokens somewhere secure
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(authInfo.IdToken);
            var claims = jsonToken?.Payload?.Claims;

            var name = claims?.FirstOrDefault(x => x.Type == "name")?.Value;
            var email = claims?.FirstOrDefault(x => x.Type == "email")?.Value;
            var preferredUsername = claims?
                .FirstOrDefault(x => x.Type == "preferred_username")?.Value;
        }

        private void SignupClicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://dev-660868.okta.com/signin/register"));
        }

        private void LogoutClicked(object sender, EventArgs e)
        {
            LoginPanel.IsVisible = true;
            LogoutPanel.IsVisible = false;
            if (LoginChanged != null) LoginChanged(this, false);
        }
    }
}