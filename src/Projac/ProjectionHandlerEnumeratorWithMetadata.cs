using System;
using System.Collections;
using System.Collections.Generic;

namespace Projac
{
    /// <summary>
    /// Represents a <see cref="ProjectionHandler{TConnection}"/> array enumerator.
    /// </summary>
    public class ProjectionHandlerEnumerator<TConnection, TMetadata> : IEnumerator<ProjectionHandler<TConnection, TMetadata>>
    {
        private readonly ProjectionHandler<TConnection, TMetadata>[] _handlers;
        private int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionHandlerEnumerator{TConnection}"/> class.
        /// </summary>
        /// <param name="handlers">The handlers to enumerate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="handlers"/> are <c>null</c>.</exception>
        public ProjectionHandlerEnumerator(ProjectionHandler<TConnection, TMetadata>[] handlers)
        {
            _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
            _index = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            return _index < _handlers.Length &&
                   ++_index < _handlers.Length;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _index = -1;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Enumeration has not started. Call MoveNext.
        /// or
        /// Enumeration has already ended. Call Reset.
        /// </exception>
        public ProjectionHandler<TConnection, TMetadata> Current
        {
            get
            {
                if (_index == -1)
                    throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                if (_index == _handlers.Length)
                    throw new InvalidOperationException("Enumeration has already ended. Call Reset.");

                return _handlers[_index];
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current => Current;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}