using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WeatherBackend.Attributes;
using WeatherBackend.Models;
using WeatherBackend.Services;
using static System.Net.WebRequestMethods;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterServices _registerService;
        private readonly SupabaseService _supabaseService;
        private readonly WeatherService _weatherService;
        //private readonly EmailService _emailService;

        public AuthController(RegisterServices registerService, SupabaseService supabaseService, WeatherService weatherService)
        {
            _registerService = registerService;
            _supabaseService = supabaseService;
            _weatherService = weatherService;
            //_emailService = emailService;
        }

        [HttpGet("validate")]
        [AuthState]
        public IActionResult ValidateToken()
        {
            var email = HttpContext.Items["Email"]?.ToString();

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Email could not be determined from token.");

            return Ok(new { Email = email });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterData data)
        {

            var existing = await _registerService.GetUser(data.Email);
            if (existing != null)
                return Conflict("User already exists.");

            await _registerService.CreateAsyncUser(data);
            return Ok("Registered successfully.");
        }

        [HttpPost("register-Google")]
        public async Task<IActionResult> RegisterGoogle(GoogleData data)
        {

            var existing = await _registerService.GetUser(data.Email);
            if (existing != null)
                return Conflict("User already exists.");

            await _registerService.CreateAsyncUserGoogle(data);
            return Ok("Registered successfully.");
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<RegisterData>> GetUser(string email)
        {
            var user = await _registerService.GetUser(email);
            return user != null ? Ok(user) : NotFound();
        }

        [AuthState]
        [HttpPost("favourite")]
        public async Task<IActionResult> AddFavourite([FromQuery] string city)
        {
            //var email = User.FindFirst("email")?.Value;
            var email = HttpContext.Items["Email"]?.ToString(); // ✅ Updated line
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized("Email claim missing in JWT.");

            await _registerService.AddFavouriteAsync(email, city);
            return Ok("Favourite added.");
        }

        //[AuthState]
        //[HttpGet("debug-claims")]
        //public IActionResult DebugClaims()
        //{
        //    var claims = User.Claims.Select(c => new { c.Type, c.Value });
        //    return Ok(claims);
        //}

        [HttpGet("suggest")]
        [AuthState]
        public async Task<IActionResult> GetCitySuggestions([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is empty.");

            var apiKey = "c06d0be622563127c49524eb461ced09"; // Replace with your actual API key
            var url = $"http://api.openweathermap.org/geo/1.0/direct?q={query}&limit=20&appid={apiKey}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to fetch suggestions.");

            var json = await response.Content.ReadAsStringAsync();
            var rawList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

            var cityNames = rawList?
                .Where(c => c.ContainsKey("name"))
                .Select(c => c["name"].ToString())
                .Distinct()
                .ToList();

            return Ok(cityNames ?? new List<string>());
        }

        [HttpGet("favourites")]
        [AuthState]
        public async Task<ActionResult<List<string>>> GetFavourites()
        {
            var email = HttpContext.Items["Email"]?.ToString(); // ✅ Updated line

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized("Email missing in request context.");

            var favs = await _registerService.GetFavouritesAsync(email);
            return Ok(favs);
        }

       
        [HttpGet("getWeather/{city}")]
        [AuthState]
        public async Task<ActionResult<WeatherResponse?>> GetWeather(string city)
        {
            var weather = await _weatherService.GetWeatherAsync(city);
            return weather != null ? Ok(weather) : NotFound();
        }


        [HttpGet("forecast")]
        
        public async Task<IActionResult> Get5DayForecast(string city)
        {
            var apiKey = "c06d0be622563127c49524eb461ced09";
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return BadRequest("Could not fetch forecast.");

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

       

        //[HttpPost("send-report")]
        //[AuthState]
        //public async Task<IActionResult> SendWeatherReport([FromQuery] string city)
        //{
        //    var email = HttpContext.Items["Email"]?.ToString();
        //    if (string.IsNullOrWhiteSpace(email))
        //        return Unauthorized("Email is missing in the request context.");

        //    var fetched = await _weatherService.GetWeatherAsync(city);
        //    if (fetched == null)
        //        return NotFound("Could not retrieve weather for the specified city.");

        //    var body = $"Weather Report for {city}:\n\n" +
        //               $"Temperature: {fetched.main.temp}°C\n" +
        //               $"Condition: {fetched.weather[0].description}\n" +
        //               $"Humidity: {fetched.main.humidity}%\n" +
        //               $"Wind Speed: {fetched.wind.speed} m/s\n";

        //    await _emailService.SendEmailAsync(email, $"Your Weather Report for {city}", body);
        //    return Ok("Weather report sent to your email.");
        //}


            [HttpPost("supabase-signup")]
        public async Task<IActionResult> SupabaseSignUp([FromBody] SupabaseSignUpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

            bool result = await _supabaseService.SignUpUserAsync(request.Email, request.Password);

            if (result)
                return Ok("✅ Supabase user registered successfully.");
            else
                return StatusCode(500, "❌ Failed to sign up on Supabase.");
        }

        public class SupabaseSignUpRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("supabase-login")]
        public async Task<IActionResult> SupabaseLogin([FromBody] SupabaseLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

            var jwt = await _supabaseService.SignInUserAsync(request.Email, request.Password);

            if (!string.IsNullOrEmpty(jwt))
                return Ok(new { token = jwt }); 
            else
                return Unauthorized("❌ Invalid email or password.");
        }

        public class SupabaseLoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("google-oauth")]
        public async Task<IActionResult> HandleGoogleOAuth([FromBody] GoogleOAuthToken tokenData)
        {
            if (string.IsNullOrWhiteSpace(tokenData.AccessToken))
                return BadRequest("Access token is required.");

            try
            {
                var user = await _supabaseService.GetUserFromAccessToken(tokenData.AccessToken);

                if (user != null)
                {
                    return Ok(new { token = tokenData.AccessToken, Email = user.Email, Id = user.Id });
                }

                return Unauthorized("User not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error verifying token: " + ex.Message);
            }
        }

        public class GoogleOAuthToken
        {
            public string AccessToken { get; set; }
        }

    }
}
