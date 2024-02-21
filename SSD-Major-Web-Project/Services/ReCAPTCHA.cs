using Newtonsoft.Json;

namespace SSD_Major_Web_Project.Services
{
    public class ReCAPTCHA
    {
        public class ReCaptchaValidationResult
        {
            public bool Success { get; set; }
            public string HostName { get; set; }
            [JsonProperty("challenge_ts")]
            public string TimeStamp { get; set; }
            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

        public class ReCaptchaValidator
        {
            public static ReCaptchaValidationResult IsValid(string secret,
                string captchaResponse)
            {
                if (string.IsNullOrWhiteSpace(captchaResponse))
                {
                    return new ReCaptchaValidationResult()
                    { Success = false };
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.google.com");

                var values = new List<KeyValuePair<string, string>>();

                values.Add(new KeyValuePair<string, string>
                ("secret", secret));
                values.Add(new KeyValuePair<string, string>
                    ("response", captchaResponse));
                FormUrlEncodedContent content =
                                new FormUrlEncodedContent(values);

                HttpResponseMessage response = client.PostAsync
                ("/recaptcha/api/siteverify", content).Result;

                string verificationResponse = response.Content.
                ReadAsStringAsync().Result;

                var verificationResult = JsonConvert.DeserializeObject
                <ReCaptchaValidationResult>(verificationResponse);

                return verificationResult;
            }
        }
    }

}
