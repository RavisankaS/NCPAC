namespace NCPAC_LambdaX.Utilities
{
    public static class CookieHelper
    {
        public static void CookieSet(HttpContext _context, string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            _context.Response.Cookies.Append(key, value, option);
        }
    }
}
