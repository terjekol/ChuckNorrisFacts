namespace ChuckNorrisFacts
{
    public class Constants
    {
        public const string AuthStateKey = "authState";
        public const string AuthServiceDiscoveryKey = "authServiceDiscovery";

        public const string ClientId = "0oamfrj31sVs13hyx356";
        public const string RedirectUri = "com.okta.dev-660868:/callback";
        public const string OrgUrl = "https://dev-660868.okta.com";
        public const string AuthorizationServerId = "default";

        public static readonly string DiscoveryEndpoint =
            $"{OrgUrl}/oauth2/{AuthorizationServerId}/.well-known/openid-configuration";


        public static readonly string[] Scopes = new string[] {
            "openid", "profile", "email", "offline_access" };
    }
}
