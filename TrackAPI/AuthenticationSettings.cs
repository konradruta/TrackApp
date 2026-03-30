namespace TrackAPI
{
    public class AuthenticationSettings
    {
        public string JWTKey { get; set; }
        public string JWTIssuer { get; set; }
        public int JWTExpiresDays { get; set; }
    }
}
