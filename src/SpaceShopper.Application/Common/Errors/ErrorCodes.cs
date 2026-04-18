namespace SpaceShopper.Application.Common.Errors
{
    public static class ErrorCodes
    {
        public static class Auth
        {
            public const string InvalidCredentials = "AUTH-400-001";
            public const string InvalidLoginCode = "AUTH-400-002";
            public const string Unauthorized = "AUTH-401-001";
            public const string RefreshTokenInvalid = "AUTH-401-002";
        }

        public static class User
        {
            public const string UserNotFound = "USR-404-001";
            public const string EmailAlreadyExists = "USR-409-001";
            public const string AlreadyVerified = "USR-409-002";
            public const string CodeInvalidOrExpired = "USR-401-001";
            public const string PasswordIncorrect = "USR-401-002";
            public const string PasswordPolicyViolated = "USR-400-001";
            public const string ChildEntityNotFound = "USR-404-002";
            public const string DefaultEntityDeleteForbidden = "USR-409-003";
            public const string EmailInvalid = "USR-400-002";
        }

        public static class Wishlist
        {
            public const string ItemNotFound = "WIS-404-001";
            public const string Duplicate = "WIS-409-001";
        }

        public static class Application
        {
            public const string Validation = "APP-400-001";
            public const string NotFound = "APP-404-001";
            public const string Domain = "APP-409-001";
        }

        public static class Infrastructure
        {
            public const string Unknown = "INF-500-001";
        }
    }
}
