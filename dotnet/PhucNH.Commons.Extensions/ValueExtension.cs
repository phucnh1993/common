namespace PhucNH.Commons.Extensions
{
    public static class ValueExtension
    {
        /// <summary>
        /// Return a object to string.
        /// </summary>
        /// <param name="obj">Object value.</param>
        /// <param name="defaultValue">Default string value.</param>
        /// <returns>A string value.</returns>
        public static string ToString(
            this object obj,
            string defaultValue = ConstantExtension.Empty)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            return obj.ToString() ?? defaultValue;
        }
    }
}