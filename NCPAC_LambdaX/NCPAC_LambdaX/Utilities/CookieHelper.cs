<<<<<<< HEAD
﻿namespace MVC_Music.Utilities
{
    public static class CookieHelper
    {
=======
﻿namespace NCPAC_LambdaX.Utilities
{
    public static class CookieHelper
    {
        /// <summary>  
        /// set the cookie  
        /// </summary>  
        /// <param name="_context">the HttpContext</param>  
        /// <param name="key">key (unique indentifier)</param>  
        /// <param name="value">value to store in cookie object</param>  
        /// <param name="expireTime">expiration time</param>  
>>>>>>> 0ca81c02b436ae351968fa01d3811f31f2045163
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