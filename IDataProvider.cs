using System.Collections.Generic;

namespace Interview
{
    /// <summary>
    /// Simple interface for populating data.
    /// </summary>
    /// <typeparam name="T">The generic <see cref="T"/> parameter.</typeparam>
    public interface IDataProvider<T>
    {
        /// <summary>
        /// Returns a collection of data.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> collection.</returns>
        IEnumerable<T> LoadData();
    }
}