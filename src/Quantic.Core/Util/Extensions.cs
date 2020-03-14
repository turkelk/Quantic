using System;

namespace Quantic.Core.Util
{
    public static class Extensions
    {
        public static void Guard(this object obj, string message, string paramName) {
			if (obj == null) {
				throw new ArgumentNullException(paramName, message);
			}
		}

        public static void Guard(this object obj, string paramName) {
			if (obj == null) {
				throw new ArgumentNullException(paramName);
			}
		}        

		public static void Guard(this string str, string message, string paramName) {
			if (str == null) {
				throw new ArgumentNullException(paramName, message);
			}

			if (string.IsNullOrEmpty(str)) {
				throw new ArgumentException(message, paramName);
			}
		}

		public static void Guard(this string str, string paramName) {
			if (str == null) {
				throw new ArgumentNullException(paramName);
			}

			if (string.IsNullOrEmpty(str)) {
				throw new ArgumentException(paramName);
			}
		}   
    }
}