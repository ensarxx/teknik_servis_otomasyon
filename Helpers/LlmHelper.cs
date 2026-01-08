using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using TeknikServisOtomasyon.Data.Repositories;

namespace TeknikServisOtomasyon.Helpers
{
    public class LlmHelper
    {
        private const string OPENROUTER_API_URL = "https://openrouter.ai/api/v1/chat/completions";
        private static readonly HttpClient _httpClient = new();

        public class TeşhisResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("olasıSorunlar")]
            public List<string> OlasıSorunlar { get; set; } = new();

            [JsonPropertyName("çözümÖnerileri")]
            public List<string> ÇözümÖnerileri { get; set; } = new();

            [JsonPropertyName("hataMesaji")]
            public string HataMesaji { get; set; } = "";

            [JsonPropertyName("kontrol")]
            public List<string> Kontrol { get; set; } = new();

            [JsonPropertyName("uyarı")]
            public string Uyarı { get; set; } = "";
        }

        public static async Task<TeşhisResponse> ArizaTeshisiAsync(
            string cihazTuru,
            string marka,
            string model,
            string arizaAciklamasi,
            string arizaDetay = "")
        {
            try
            {
                // API anahtarı environment variable'dan al
                var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    return new TeşhisResponse
                    {
                        Success = false,
                        HataMesaji = "OpenRouter API anahtarı yapılandırılmadı. Lütfen sistem yöneticisine başvurun."
                    };
                }

                var prompt = GeneratePrompt(cihazTuru, marka, model, arizaAciklamasi, arizaDetay);

                var request = new
                {
                    model = "openrouter/auto",
                    messages = new object[]
                    {
                        new
                        {
                            role = "system",
                            content = "Sen bir teknik servis uzmanısın. Kullanıcının tanımladığı arıza için olası sorunları ve çözüm önerilerini JSON formatında ver."
                        },
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    temperature = 0.7,
                    max_tokens = 1000
                };

                var jsonRequest = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Headers ekle
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://teknik-servis-otomasyon.local");
                _httpClient.DefaultRequestHeaders.Add("X-Title", "Teknik Servis Otomasyon");

                var response = await _httpClient.PostAsync(OPENROUTER_API_URL, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new TeşhisResponse
                    {
                        Success = false,
                        HataMesaji = $"API Hatası: {response.StatusCode} - {errorContent}"
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseContent);

                // OpenRouter yanıtından metni al
                var messageContent = jsonResponse.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrEmpty(messageContent))
                {
                    return new TeşhisResponse
                    {
                        Success = false,
                        HataMesaji = "API'den geçersiz yanıt alındı."
                    };
                }

                // JSON yanıtını parse et
                return ParseTeşhisResponse(messageContent);
            }
            catch (Exception ex)
            {
                return new TeşhisResponse
                {
                    Success = false,
                    HataMesaji = $"Hata: {ex.Message}"
                };
            }
        }

        private static string GeneratePrompt(
            string cihazTuru,
            string marka,
            string model,
            string arizaAciklamasi,
            string arizaDetay)
        {
            var detay = string.IsNullOrEmpty(arizaDetay) ? "" : $"\n\nDetaylı Bilgi: {arizaDetay}";

            return $@"
Aşağıdaki cihaz arızası için teknik teşhis ve çözüm önerileri sun:

Cihaz Bilgisi:
- Tür: {cihazTuru}
- Marka: {marka}
- Model: {model}

Müşteri Arızası: {arizaAciklamasi}{detay}

Lütfen yanıtı aşağıdaki JSON formatında ver (Türkçe kullan):
{{
    ""success"": true,
    ""olasıSorunlar"": [
        ""Olası Sorun 1"",
        ""Olası Sorun 2"",
        ""Olası Sorun 3""
    ],
    ""çözümÖnerileri"": [
        ""Çözüm Adım 1"",
        ""Çözüm Adım 2"",
        ""Çözüm Adım 3""
    ],
    ""kontrol"": [
        ""Kontrol Noktası 1"",
        ""Kontrol Noktası 2""
    ],
    ""uyarı"": ""Eğer varsa güvenlik uyarısı""
}}

SADECE JSON çıktısını ver, başka metin ekleme.";
        }

        private static TeşhisResponse ParseTeşhisResponse(string jsonText)
        {
            try
            {
                // JSON metninden kodu çıkar (eğer markdown içine alınmışsa)
                var cleanJson = jsonText.Replace("```json", "").Replace("```", "").Trim();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var response = JsonSerializer.Deserialize<TeşhisResponse>(cleanJson, options);

                if (response != null)
                {
                    response.Success = true;
                    return response;
                }

                throw new Exception("JSON parse hatası");
            }
            catch (Exception ex)
            {
                return new TeşhisResponse
                {
                    Success = false,
                    HataMesaji = $"Yanıt parse hatası: {ex.Message}"
                };
            }
        }

        public static bool IsConfigured()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENROUTER_API_KEY"));
        }
    }
}
