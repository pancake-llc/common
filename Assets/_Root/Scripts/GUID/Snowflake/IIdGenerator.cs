using System.Collections.Generic;

namespace Pancake.Common
{
    /// <summary>
    /// This interface represents ID generator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIdGenerator<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Generates new identifier every time the method is called
        /// </summary>
        /// <returns>new identifier</returns>
        T GenerateId();
    }
}