using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Core.Interfaces;

namespace Core
{
    public static class Channel
    {
        /// <summary>
        /// Returns a new channel given optional size
        /// </summary>
        /// <param name="size">optional size</param>
        /// <typeparam name="T">channel type parameter</typeparam>
        /// <returns>new channel with given size and parameter type</returns>
        public static IChannel<T> New<T>(int size = 1)
        {
            return new Channel<T>(size);
        }
    }
    
    internal class Channel<T> : IChannel<T>
    {
        private readonly BlockingCollection<T> _buffer;

        public Channel(int size)
        {
            _buffer = new BlockingCollection<T>(new ConcurrentQueue<T>(), size);
        }

        public bool Send(T t)
        {
            try
            {
                _buffer.Add(t);
            }
            catch (InvalidOperationException)
            {
                // Will be thrown when the collection gets closed
                return false;
            }

            return true;
        }

        public bool Receive(out T val)
        {
            try
            {
                val = _buffer.Take();
            }
            catch (InvalidOperationException)
            {
                // will be thrown when the collection is empty and got closed
                val = default;

                return false;
            }

            return true;
        }

        public void Close()
        {
            _buffer.CompleteAdding();
        }

        public IEnumerable<T> Range()
        {
            while (Receive(out var val))
            {
                yield return val;
            }
        }

        public void Dispose()
        {
            _buffer?.Dispose();
        }
    }
}