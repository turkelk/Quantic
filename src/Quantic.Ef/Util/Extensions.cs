using System;

namespace Quantic.Ef.Util
{
    public static class Extensions
    {
        public static long ToUnixTimeMilliseconds(this DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeMilliseconds();
        }  
    }
}