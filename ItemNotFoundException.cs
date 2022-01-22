using System;

namespace Interview
{
    /// <summary>
    /// Exception thrown when an item is not found.
    /// </summary>
    internal class ItemNotFoundException : Exception
    {
        /// <inheritdoc />
        public ItemNotFoundException(string message) : base(message)
        {
        }
    }
}