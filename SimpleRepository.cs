using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Interview
{
    /// <summary>
    /// Simple implementation of the <see cref="IRepository{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="IStoreable"/> type.</typeparam>
    public class SimpleRepository<T> : IRepository<T> where T : IStoreable
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dataProvider">A <see cref="IDataProvider{T}"/> instance for loading data.</param>
        public SimpleRepository(IDataProvider<T> dataProvider)
        {
            LoadData(dataProvider);
        }

        #region IRepository
        private ConcurrentDictionary<IComparable, T> data = new ConcurrentDictionary<IComparable, T>();
        /// <summary>
        /// Gets the collection of data.
        /// </summary>
        public ConcurrentDictionary<IComparable, T> Data => data;
        /// <summary>
        /// Returns all items in the repository.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> form of the underlying data.</returns>
        public IEnumerable<T> All() => Data.Values;
        /// <summary>
        /// Deletes an item from the repository.
        /// </summary>
        /// <param name="id">The <see cref="IComparable"/> id value of the required item.</param>
        public void Delete(IComparable id)
        {
            id.CheckAndThrow(nameof(id));
            if (!data.TryGetValue(id, out _))
            {
                throw new ItemNotFoundException(id.ToString());
            }
            if (!Data.TryRemove(id, out _))
            {
                throw new ItemNotDeletedException(id.ToString());
            }
        }
        /// <summary>
        /// Retrieves an item from the repository.
        /// </summary>
        /// <param name="id">The <see cref="IComparable"/> id value of the required item.</param>
        /// <returns>Returns a <see cref="T"/> value.</returns>
        public T FindById(IComparable id)
        {
            id.CheckAndThrow(nameof(id));
            data.TryGetValue(id, out T result);
            return result;
        }
        /// <summary>
        /// Creates new items or updates existing ones in the repository
        /// </summary>
        /// <param name="item">The <see cref="T"/> item to save.</param>
        public void Save(T item)
        {
            item.ThrowIfNull(nameof(item));
            if (data.TryGetValue(item.Id, out T existing))
            {
                data.TryUpdate(item.Id, item, existing);
            }
            else
            {
                data.TryAdd(item.Id, item);
            }           
        }
        #endregion

        #region private
        private void LoadData(IDataProvider<T> dataProvider)
        {
            var items = dataProvider.LoadData();
            if (items != null)
            {
                data = new ConcurrentDictionary<IComparable, T>(
                    dataProvider.LoadData().ToDictionary(c => c.Id, c => c)
                    );
            }            
        }
        #endregion
    }
}
