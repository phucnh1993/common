using System;

namespace PhucNH.Commons.Extensions
{
    public static class ExceptionExtension
    {
        public static void ValidateNullObject(
            this object obj,
            string methodName,
            string message)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(
                    methodName,
                    message);
            }
            return;
        }

        public static void ValidateNullObjects(
            this string methodName,
            string message,
            params object[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(
                        methodName,
                        $"{message} Please check at [{nameof(obj)}].");
                }
            }
            return;
        }
    }
}