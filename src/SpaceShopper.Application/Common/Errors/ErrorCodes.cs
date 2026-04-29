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

        public static class Cart
        {
            public const string ItemNotFound = "CRT-404-001";
            public const string InsufficientStock = "CRT-400-001";
        }

        public static class Review
        {
            public const string InvalidSortBy = "REV-400-001";
            public const string InvalidPaging = "REV-400-002";
            public const string ProductNotPurchased = "REV-422-001";
            public const string DuplicateReview = "REV-409-001";
            public const string RatingOutOfRange = "REV-400-003";
            public const string CommentRequired = "REV-400-004";
        }

        public static class Checkout
        {
            public const string ShippingMethodNotFound = "CHK-404-001";
            public const string ShippingAddressRequired = "CHK-400-001";
            public const string ShippingAddressNotFound = "CHK-404-002";
            public const string InvalidOrderPromotionCode = "CHK-400-002";
            public const string InvalidShippingPromotionCode = "CHK-400-003";
            public const string DuplicatePromotionType = "CHK-400-004";
            public const string EmptySelection = "CHK-400-005";
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
