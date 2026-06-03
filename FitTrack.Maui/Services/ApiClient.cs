using FitTrack.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitTrack.Maui.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private const string TokenKey = "jwt_token";
        private string? _cachedToken;

        private const string BaseUrl = "https://dotnet.strontiumlabs.com/";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public ApiClient(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri(BaseUrl);
            _http.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<string?> GetTokenAsync()
        {
            if (_cachedToken != null) return _cachedToken;
            try { _cachedToken = await SecureStorage.Default.GetAsync(TokenKey); return _cachedToken; }
            catch { return null; }
        }

        public async Task SaveTokenAsync(string token)
        {
            _cachedToken = token;
            try { await SecureStorage.Default.SetAsync(TokenKey, token); }
            catch { }
        }

        public void ClearToken()
        {
            _cachedToken = null;
            try { SecureStorage.Default.Remove(TokenKey); }
            catch { }
        }

        private async Task AttachTokenAsync()
        {
            var token = await GetTokenAsync();
            _http.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            try
            {
                var json = JsonSerializer.Serialize(new { email = email, password = password });
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _http.PostAsync("api/auth/login", content);
                var body = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Status {(int)response.StatusCode}: {body}");
                
                return JsonSerializer.Deserialize<LoginResponse>(body, JsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP: {ex.GetType().Name}: {ex.Message} (Status: {ex.StatusCode})", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Request timed out (15s)");
            }
        }

        public async Task<DashboardSummary?> GetDashboardAsync()
        {
            await AttachTokenAsync();
            try
            {
                var response = await _http.GetAsync("api/dashboard");
                var body = await response.Content.ReadAsStringAsync();
                
                // DEBUG: dump raw body to app data for inspection
                try { await File.WriteAllTextAsync(
                    System.IO.Path.Combine(FileSystem.AppDataDirectory, "dashboard-debug.json"), body); } catch { }
                
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Dashboard API {(int)response.StatusCode}: {body}");
                
                var result = JsonSerializer.Deserialize<DashboardSummary>(body, JsonOptions);
                
                // DEBUG: dump parsed result
                try { await File.WriteAllTextAsync(
                    System.IO.Path.Combine(FileSystem.AppDataDirectory, "dashboard-parsed.txt"),
                    $"Cal:{result?.TodayCalories} Pro:{result?.TodayProtein} Carb:{result?.TodayCarbs} Fat:{result?.TodayFats}"); } catch { }
                
                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Dashboard HTTP error: {ex.Message}", ex);
            }
        }

        public async Task<List<FitnessGoal>> GetGoalsAsync()
        {
            try { await AttachTokenAsync(); return await _http.GetFromJsonAsync<List<FitnessGoal>>("api/fitnessgoals", JsonOptions) ?? new(); }
            catch { return new(); }
        }

        public async Task<FitnessGoal?> CreateGoalAsync(FitnessGoal goal)
        {
            try { await AttachTokenAsync(); var r = await _http.PostAsJsonAsync("api/fitnessgoals", goal); if (!r.IsSuccessStatusCode) return null; return await r.Content.ReadFromJsonAsync<FitnessGoal>(JsonOptions); }
            catch { return null; }
        }

        public async Task<FitnessGoal?> UpdateGoalAsync(FitnessGoal goal)
        {
            try { await AttachTokenAsync(); var r = await _http.PutAsJsonAsync($"api/fitnessgoals/{goal.FitnessGoalId}", goal); if (!r.IsSuccessStatusCode) return null; return await r.Content.ReadFromJsonAsync<FitnessGoal>(JsonOptions); }
            catch { return null; }
        }

        public async Task<bool> DeleteGoalAsync(int id)
        {
            try { await AttachTokenAsync(); return (await _http.DeleteAsync($"api/fitnessgoals/{id}")).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<List<WorkoutSession>> GetWorkoutsAsync()
        {
            try { await AttachTokenAsync(); return await _http.GetFromJsonAsync<List<WorkoutSession>>("api/workoutsessions", JsonOptions) ?? new(); }
            catch { return new(); }
        }

        public async Task<WorkoutSession?> CreateWorkoutAsync(WorkoutSession session)
        {
            try { await AttachTokenAsync(); var r = await _http.PostAsJsonAsync("api/workoutsessions", session); if (!r.IsSuccessStatusCode) return null; return await r.Content.ReadFromJsonAsync<WorkoutSession>(JsonOptions); }
            catch { return null; }
        }

        public async Task<WorkoutSession?> UpdateWorkoutAsync(WorkoutSession session)
        {
            try { await AttachTokenAsync(); var r = await _http.PutAsJsonAsync($"api/workoutsessions/{session.WorkoutSessionId}", session); if (!r.IsSuccessStatusCode) return null; return await r.Content.ReadFromJsonAsync<WorkoutSession>(JsonOptions); }
            catch { return null; }
        }

        public async Task<bool> DeleteWorkoutAsync(int id)
        {
            try { await AttachTokenAsync(); return (await _http.DeleteAsync($"api/workoutsessions/{id}")).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<List<MealLog>> GetMealsAsync()
        {
            try { await AttachTokenAsync(); return await _http.GetFromJsonAsync<List<MealLog>>("api/meallogs", JsonOptions) ?? new(); }
            catch { return new(); }
        }

        public async Task<MealLog?> CreateMealAsync(MealLog meal)
        {
            try { await AttachTokenAsync(); var r = await _http.PostAsJsonAsync("api/meallogs", meal); if (!r.IsSuccessStatusCode) return null; return await r.Content.ReadFromJsonAsync<MealLog>(JsonOptions); }
            catch { return null; }
        }

        public async Task<MealLog?> UpdateMealAsync(MealLog meal)
        {
            try { await AttachTokenAsync(); var r = await _http.PutAsJsonAsync($"api/meallogs/{meal.MealLogId}", meal); if (!r.IsSuccessStatusCode) return null; return await r.Content.ReadFromJsonAsync<MealLog>(JsonOptions); }
            catch { return null; }
        }

        public async Task<bool> DeleteMealAsync(int id)
        {
            try { await AttachTokenAsync(); return (await _http.DeleteAsync($"api/meallogs/{id}")).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<UserProfile?> GetProfileAsync()
        {
            try { await AttachTokenAsync(); return await _http.GetFromJsonAsync<UserProfile>("api/profile", JsonOptions); }
            catch { return null; }
        }

        public async Task<UserProfile?> UpsertProfileAsync(UserProfile profile)
        {
            try { await AttachTokenAsync(); var r = await _http.PostAsJsonAsync("api/profile", profile); if (!r.IsSuccessStatusCode) return null; return await r.Content.ReadFromJsonAsync<UserProfile>(JsonOptions); }
            catch { return null; }
        }

        public class LoginResponse { public string Token { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string UserId { get; set; } = string.Empty; }

        public class DashboardSummary
        {
            public int TodayCalories { get; set; }
            public double TodayProtein { get; set; }
            public double TodayCarbs { get; set; }
            public double TodayFats { get; set; }
            public int WeekWorkouts { get; set; }
            public int WeekCaloriesBurned { get; set; }
            public int ActiveGoals { get; set; }
            public List<MealLog> RecentMeals { get; set; } = new();
            public List<WorkoutSession> RecentWorkouts { get; set; } = new();
        }
    }
}
