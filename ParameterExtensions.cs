using System;

namespace Interview
{
    /// <summary>
    /// Static class for parameter check.
    /// </summary>
    public static class ParameterExtensions
    {
        /// <summary>
        /// Checks for null or empty values in parameters.
        /// </summary>
        /// <param name="parameter">The <see cref="IComparable"/> parameter value.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        public static void CheckAndThrow(this IComparable parameter, string parameterName)
        {
            parameter.ThrowIfNull(parameterName);
            if (parameter is string && string.IsNullOrWhiteSpace((string)parameter))
            {
                throw new ArgumentException(parameterName);
            }
        }
        /// <summary>
        /// Checks for null values in parameters.
        /// </summary>
        /// <param name="parameter">The parameter value.</param>
        /// <param name="parameterName">The parameter name.</param>
        public static void ThrowIfNull(this object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
