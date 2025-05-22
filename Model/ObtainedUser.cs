using Newtonsoft.Json;

namespace AppSalas.Model
{
    public class ObtainedUser
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

    }

}

