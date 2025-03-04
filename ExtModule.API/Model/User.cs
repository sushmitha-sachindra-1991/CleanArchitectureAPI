using System.Collections;

namespace ExtModule.API.Model
{
    public class UserLoginDto
    {
        public string? Username { get; set; }=default;
        public string? Password { get; set; }=default;
        public string? SessionId { get; set; } = default;
        public string CompanyCode { get; set; }
    }
    public class Loginresponse
    {
        public string url { get; set; }

        public List<Data> data { get; set; }

        public int result { get; set; }

        public string message { get; set; }
    }
    public class Data
    {
        public int AltLanguageId { get; set; }
        public int EmployeeId { get; set; }

        public string fSessionId { get; set; }
    }
    public class TokenDto
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
