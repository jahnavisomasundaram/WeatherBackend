using static Supabase.Gotrue.Constants;
using System.Text.Json;

namespace WeatherBackend.Services
{
    public class ApiAuth
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im1oYWZ3YXN3bHdldnJ4Y3ZmbWFoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTAyMjMzOTMsImV4cCI6MjA2NTc5OTM5M30.5aTUpytqsf-rDBr4SL4_EOCSXalLhJNqa1KXMUJypqE";

        public ApiAuth(IConfiguration config, HttpClient http)
        {
            _config = config;
            _http = http;
        }

        public async Task<AuthResult> CheckAuthenticationState(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://mhafwaswlwevrxcvfmah.supabase.co/auth/v1/user");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Headers.Add("apikey", SupabaseKey);

                var response = await _http.SendAsync(request);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    
                    return new AuthResult { status = AuthStatus.Unauthorized };
                }

                var user = JsonSerializer.Deserialize<UserContent>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return new AuthResult
                {
                    status = AuthStatus.Authorized,
                    UserContent = user
                };
            }
            catch (Exception ex)
            {
                // Optionally log exception
                return new AuthResult { status = AuthStatus.Unauthorized };
            }
        }
    }

    public class AuthResult
    {
        public AuthStatus status { get; set; }
        public UserContent UserContent { get; set; }
    }

    public enum AuthStatus
    {
        Authorized,
        Unauthorized
    }

    public class UserContent
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Aud { get; set; }
        // Add other fields as needed
    }

    // Optionally, for error handling:
    public class SupabaseError
    {
        public string Message { get; set; }
    }
}
