using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using VegetableSupplier.Models;

namespace VegetableSupplier.Services;

public class AuthenticationService
{
    private readonly DatabaseService _database;
    private readonly GoogleService _googleService;
    private const string ClientId = "YOUR_CLIENT_ID";
    private const string ClientSecret = "YOUR_CLIENT_SECRET";

    public AuthenticationService(DatabaseService database, GoogleService googleService)
    {
        _database = database;
        _googleService = googleService;
    }

    public async Task<bool> SignInWithGoogleAsync()
    {
        try
        {
            var authFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                },
                Scopes = new[]
                {
                    "email",
                    "profile",
                    "https://www.googleapis.com/auth/drive.file",
                    "https://www.googleapis.com/auth/spreadsheets.readonly"
                }
            });

            // Use WebAuthenticator to handle the OAuth flow
            var authResult = await WebAuthenticator.AuthenticateAsync(
                new Uri($"https://accounts.google.com/o/oauth2/v2/auth?" +
                       $"client_id={ClientId}&" +
                       $"response_type=code&" +
                       $"scope=email profile https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/spreadsheets.readonly&" +
                       $"redirect_uri=com.companyname.vegetablesupplier:/oauth2redirect"),
                new Uri("com.companyname.vegetablesupplier:/oauth2redirect"));

            var code = authResult.Properties["code"];
            
            // Exchange authorization code for tokens
            var token = await authFlow.ExchangeCodeForTokenAsync(
                "com.companyname.vegetablesupplier",
                code,
                "com.companyname.vegetablesupplier:/oauth2redirect",
                CancellationToken.None);

            // Get user info from Google
            var payload = await GoogleJsonWebSignature.ValidateAsync(token.IdToken);
            
            // Check if user is authorized in Google Sheet
            await _googleService.InitializeServicesAsync(token.AccessToken);
            var isAuthorized = await _googleService.IsUserAuthorizedAsync(payload.Email);

            if (!isAuthorized)
                return false;

            // Save user to database
            var user = new AuthUser
            {
                Email = payload.Email,
                DisplayName = payload.Name,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                TokenExpiry = DateTime.UtcNow.AddSeconds(token.ExpiresInSeconds.GetValueOrDefault()),
                IsAuthorized = true,
                LastChecked = DateTime.UtcNow
            };

            await _database.SaveUserAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ValidateCurrentUserAsync()
    {
        var preferences = Preferences.Default;
        var currentEmail = preferences.Get("CurrentUserEmail", string.Empty);
        
        if (string.IsNullOrEmpty(currentEmail))
            return false;

        var user = await _database.GetUserAsync(currentEmail);
        if (user == null)
            return false;

        // Check if token needs refresh
        if (DateTime.UtcNow >= user.TokenExpiry)
        {
            // Implement token refresh logic here
            return false;
        }

        // Recheck authorization periodically
        if (DateTime.UtcNow >= user.LastChecked.AddHours(24))
        {
            await _googleService.InitializeServicesAsync(user.AccessToken);
            user.IsAuthorized = await _googleService.IsUserAuthorizedAsync(user.Email);
            user.LastChecked = DateTime.UtcNow;
            await _database.SaveUserAsync(user);
        }

        return user.IsAuthorized;
    }

    public async Task SignOutAsync()
    {
        var preferences = Preferences.Default;
        var currentEmail = preferences.Get("CurrentUserEmail", string.Empty);
        
        if (!string.IsNullOrEmpty(currentEmail))
        {
            var user = await _database.GetUserAsync(currentEmail);
            if (user != null)
            {
                user.AccessToken = null;
                user.RefreshToken = null;
                user.IsAuthorized = false;
                await _database.SaveUserAsync(user);
            }
        }

        preferences.Remove("CurrentUserEmail");
    }
}