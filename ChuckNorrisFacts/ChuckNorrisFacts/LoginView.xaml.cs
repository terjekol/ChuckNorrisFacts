using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public event EventHandler<string> LoginChanged;

        private async void LoginClicked(object sender, EventArgs e)
        {
            var loginProvider = DependencyService.Get<ILoginProvider>();
            var idToken = await loginProvider.LoginAsync();

            string userName = null;
            if (idToken != null)
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var token = jwtHandler.ReadJwtToken(idToken);
                userName = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            }

            if (LoginChanged != null) LoginChanged(this, userName);

            if (userName == null)
            {
                ErrorLabel.Text = "Login failed.";
                return;
            }

            LoginPanel.IsVisible = false;
            LogoutPanel.IsVisible = true;
            ErrorLabel.Text = "";
            LoggedInLabel.Text = "You are logged in as " + userName;
        }

        private void SignupClicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://dev-660868.okta.com/signin/register"));
        }

        private void LogoutClicked(object sender, EventArgs e)
        {
            LoginPanel.IsVisible = true;
            LogoutPanel.IsVisible = false;
            if (LoginChanged != null) LoginChanged(this, null);
        }
    }
}