namespace Bookify.Web.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user) =>
            user.GetUserId();
    }
}