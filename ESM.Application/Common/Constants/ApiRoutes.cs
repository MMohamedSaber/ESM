namespace ESM.Application.Common.Constants;

public static class ApiRoutes
{
    public const string Root = "api";

    public static class Auth
    {
        public const string Base = Root + "/auth";
        
        public const string Login = Base + "/login";
        public const string Refresh = Base + "/refresh";
        public const string Me = Base + "/me";
        public const string ChangePassword = Base + "/change-password";
    }

    public static class Organization
    {
        public const string Base = Root + "/organizations";

        public const string Register = Base + "/register";
    }
}
