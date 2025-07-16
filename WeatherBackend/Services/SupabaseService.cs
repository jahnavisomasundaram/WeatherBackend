//using Microsoft.AspNetCore.DataProtection.KeyManagement;
//using Supabase;
//using Supabase.Gotrue;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;

//namespace WeatherBackend.Services
//{
//    public class SupabaseService
//    {
//        public Supabase.Client SupabaseClient { get; private set; }

//        private readonly string SupabaseUrl = "https://mhafwaswlwevrxcvfmah.supabase.co";
//        private readonly string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im1oYWZ3YXN3bHdldnJ4Y3ZmbWFoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTAyMjMzOTMsImV4cCI6MjA2NTc5OTM5M30.5aTUpytqsf-rDBr4SL4_EOCSXalLhJNqa1KXMUJypqE";
//        private readonly HttpClient httpClient;
//        public SupabaseService()
//        {
//            httpClient = new HttpClient { BaseAddress = new Uri(SupabaseUrl) };

//        }

//        public async Task InitializeAsync()
//        {
//            var options = new Supabase.SupabaseOptions
//            {
//                AutoConnectRealtime = false
//            };

//            SupabaseClient = new Supabase.Client(SupabaseUrl, SupabaseKey, options);
//            await SupabaseClient.InitializeAsync();
//        }
//        public async Task<bool> SignUpUserAsync(string email, string password)
//        {
//            try
//            {
//                string endpoint = "/auth/v1/signup";

//                var signUpPayload = new
//                {
//                    email = email,
//                    password = password,
//                    //options = new
//                    //{
//                    //    redirectTo = "https://localhost:7019/confirm"
//                    //}
//                };

//                var json = JsonSerializer.Serialize(signUpPayload);
//                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
//                request.Headers.Add("apikey", SupabaseKey);
//                request.Headers.Add("Authorization", $"Bearer {SupabaseKey}");
//                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

//                var response = await httpClient.SendAsync(request);
//                string responseContent = await response.Content.ReadAsStringAsync();

//                if (response.IsSuccessStatusCode)
//                {
//                    Console.WriteLine("✅ User registered successfully.");
//                    return true;
//                }
//                else
//                {
//                    Console.WriteLine($"❌ Failed to register: {responseContent}");
//                    return false;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"🚨 Exception: {ex.Message}");
//                return false;
//            }
//        }

//        public async Task<string?> SignInUserAsync(string email, string password)
//        {
//            try
//            {
//                string endpoint = "/auth/v1/token?grant_type=password";

//                var signInPayload = new
//                {
//                    email = email,
//                    password = password,

//                };

//                var json = JsonSerializer.Serialize(signInPayload);
//                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
//                request.Headers.Add("apikey", SupabaseKey);
//                request.Headers.Add("Authorization", $"Bearer {SupabaseKey}");
//                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

//                var response = await httpClient.SendAsync(request);
//                string responseContent = await response.Content.ReadAsStringAsync();

//                if (response.IsSuccessStatusCode)
//                {
//                    Console.WriteLine("✅ User signed in successfully.");
//                    Console.WriteLine($"🔐 Supabase token: {responseContent}");

//                    // Parse the response to extract the access_token
//                    using var doc = JsonDocument.Parse(responseContent);
//                    if (doc.RootElement.TryGetProperty("access_token", out var tokenElement))
//                    {
//                        string jwt = tokenElement.GetString();
//                        Console.WriteLine($"🔐 Supabase token: {jwt}");
//                        return jwt;
//                    }
//                    else
//                    {
//                        Console.WriteLine("❌ access_token not found in response.");
//                        return null;
//                    }

//                    // Optionally extract token, user info, etc.

//                }
//                else
//                {
//                    Console.WriteLine($"❌ Failed to sign in: {responseContent}");
//                    return null;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"🚨 Exception during sign-in: {ex.Message}");
//                return null;
//            }
//        }

//        public async Task<Supabase.Gotrue.User> GetUserFromAccessToken(string accessToken)
//        {
//            if (SupabaseClient == null)
//                await InitializeAsync();

//            var user = await SupabaseClient.Auth.GetUser(accessToken);
//            return user;
//        }
//    }
//}

using Supabase;
using Supabase.Gotrue;
using System;
using System.Threading.Tasks;

namespace WeatherBackend.Services
{
    public class SupabaseService
    {
        public Supabase.Client SupabaseClient { get; private set; }

        private readonly string SupabaseUrl = "https://mhafwaswlwevrxcvfmah.supabase.co";
        private readonly string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im1oYWZ3YXN3bHdldnJ4Y3ZmbWFoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTAyMjMzOTMsImV4cCI6MjA2NTc5OTM5M30.5aTUpytqsf-rDBr4SL4_EOCSXalLhJNqa1KXMUJypqE"; // Replace with env variable in production
        private bool _initialized = false;

        public SupabaseService()
        {
            var options = new SupabaseOptions { AutoConnectRealtime = false };
            SupabaseClient = new Supabase.Client(SupabaseUrl, SupabaseKey, options);
        }

        public async Task InitializeAsync()
        {
            if (!_initialized)
            {
                await SupabaseClient.InitializeAsync();
                _initialized = true;
            }
        }

        public async Task<bool> SignUpUserAsync(string email, string password)
        {
            try
            {
                await InitializeAsync();

                var response = await SupabaseClient.Auth.SignUp(email, password);

                if (response?.User != null)
                {
                    Console.WriteLine("✅ User registered successfully.");
                    return true;
                }

                Console.WriteLine("❌ Registration failed, no user returned.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 Exception during signup: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> SignInUserAsync(string email, string password)
        {
            try
            {
                await InitializeAsync();

                var session = await SupabaseClient.Auth.SignIn(email, password);

                if (session != null && !string.IsNullOrWhiteSpace(session.AccessToken))
                {
                    Console.WriteLine("✅ User signed in successfully.");
                    Console.WriteLine($"🔐 Supabase token: {session.AccessToken}");
                    return session.AccessToken;
                }

                Console.WriteLine("❌ Failed to sign in or token missing.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 Exception during sign-in: {ex.Message}");
                return null;
            }
        }

        public async Task<User?> GetUserFromAccessToken(string accessToken)
        {
            await InitializeAsync();

            try
            {
                var user = await SupabaseClient.Auth.GetUser(accessToken);
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 Error getting user: {ex.Message}");
                return null;
            }
        }
    }
}



