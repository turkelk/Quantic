namespace Quantic.Trace.Elastic.Apm
{
    internal static class Extension
    {        
        public static string ToTransactionName(this string requestName)
        {
            return $"{requestName}-Txn";
        }
        public static string ToTransactionType(this string requestName)
        {
            return $"{requestName}-Txn";
        } 

        public static string ToSpanName(this string requestName)
        {
            return requestName;
        }
        public static string ToSpanType(this string requestName)
        {
            return $"{requestName} Handling";
        }               
    }
}