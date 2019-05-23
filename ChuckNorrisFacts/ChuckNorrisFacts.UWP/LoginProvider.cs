using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.Data.Json;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using ChuckNorrisFacts.UWP;


[assembly: Xamarin.Forms.Dependency(typeof(LoginProvider))]
namespace ChuckNorrisFacts.UWP
{
    class LoginProvider : ILoginProvider
    {
        public async Task<string> LoginAsync()
        {
            string state = GetBase64UrlData(32);
            string nonce = GetBase64UrlData(12);
            string codeVerifier = GetBase64UrlData(32);
            var tmp = CryptographicBuffer.ConvertStringToBinary(codeVerifier, BinaryStringEncoding.Utf8);
            var codeVerifierBuffer = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256).HashData(tmp);
            string code_challenge = GetBase64UrlData(null, codeVerifierBuffer);

            var callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
            var absoluteUri = callbackUri.AbsoluteUri;
            var callback = Uri.EscapeDataString(absoluteUri);

            var url = "https://dev-660868.okta.com/oauth2/default/v1/authorize";
            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format(
                "{0}?response_type=token id_token&scope=openid&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method=S256&nonce={5}",
                url,
                callback,
                Constants.ClientId,
                state,
                code_challenge,
                nonce);


            var result = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None,
                new Uri(authorizationRequest), new Uri(absoluteUri));
            if (result.ResponseStatus != WebAuthenticationStatus.Success) return null;
            return HttpUtility.ParseQueryString(result.ResponseData)["access_token"];
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static IBuffer sha256(string inputString)
        {
            var sha = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var buff = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8);
            return sha.HashData(buff);
        }

        public static string GetBase64UrlData(uint? length = null, IBuffer buffer = null)
        {
            if (length != null) buffer = CryptographicBuffer.GenerateRandom(length.Value);
            return CryptographicBuffer.EncodeToBase64String(buffer)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
