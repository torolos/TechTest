using System;

namespace Interview
{
    /// <summary>
    /// Exception thrown when an item is not removed from a collection.
    /// </summary>
    internal class ItemNotDeletedException : Exception
    {
        /// <inheritDoc/>
        public ItemNotDeletedException(string message) : base(message)
        {
        }
    }
}