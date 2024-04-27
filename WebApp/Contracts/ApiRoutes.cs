namespace WebApp.Contracts;

public static class ApiRoutes
{
    public const string Root = "api";

    public const string Version = "v1";

    public const string Base = Root + "/" + Version;

    public static class AuthenticationRoutes
    {
        public const string Login = "login";

        public const string Register = "register";

        public const string Profile = "profile";

        public const string Refresh = "refresh";

        public const string PublicKey = "public-key";
    }
}
