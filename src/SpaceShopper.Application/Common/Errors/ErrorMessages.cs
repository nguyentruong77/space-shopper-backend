namespace SpaceShopper.Application.Common.Errors
{
    public static class ErrorMessages
    {
        public static class Product
        {
            public const string ProductNotFound = "Product not found.";
        }

        public static class Wishlist
        {
            public const string ItemNotFound = "Product is not in the wishlist.";
            public const string Duplicate = "Product is already in the wishlist.";
        }

        public static class Category
        {
            public const string CategoryNotFound = "Category not found.";
        }

        public static class User
        {
            public const string UserNotFound = "User not found.";
            public const string EmailAlreadyExists = "Email already exists.";
            public const string AlreadyVerified = "User is already verified.";
            public const string CodeInvalidOrExpired = "Code is invalid or expired.";
            public const string PasswordIncorrect = "Current password is incorrect.";
            public const string PasswordPolicyViolated = "Password does not satisfy policy.";
            public const string ChildEntityNotFound = "Child entity was not found.";
            public const string DefaultEntityDeleteForbidden = "Default entity cannot be deleted.";
            public const string EmailInvalid = "Email format is invalid.";
        }

        public static class Auth
        {
            public const string InvalidCredentials = "Invalid username or password.";
            public const string InvalidLoginCode = "Login code is invalid or expired.";
            public const string EmailNotConfirmed = "Email has not been confirmed.";
            public const string Unauthorized = "You are not authorized to perform this action.";
            public const string RefreshTokenInvalid = "Refresh token is invalid or expired.";
        }

        public static class Order
        {
            public const string OrderNotFound = "Order not found.";
        }

        public static class Cart
        {
            public const string CartEmpty = "Cart is empty.";
            public const string ItemNotFound = "Product is not in the cart.";
            public const string InsufficientStock = "Insufficient stock for the requested quantity.";
        }

        public static class Common
        {
            public const string ValidationFailed = "One or more validation errors occurred.";
        }
    }
}
