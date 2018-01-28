using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure
{
    /// <summary>
    /// A recording container with fixed size, it can provide a limited number of historical navigation.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the container.</typeparam>
    public class RecordableArray<T>
    {
        private readonly T[] _array;
        private int _startPointer;
        private int _currentPointer = -1;
        private int _endPointer;


        public int Capacity { get; }
        public bool CanForward => _array[PreviousOf(_currentPointer)] != null && _currentPointer != _startPointer;
        public bool CanBackward => _array[NextOf(_currentPointer)] != null && _currentPointer != _endPointer;


        /// <summary>
        /// Initialize a <see cref="RecordableArray{T}"/> instance with Specified capacity.
        /// </summary>
        /// <param name="capacity">The size of the container.</param>
        public RecordableArray(int capacity)
        {
            Capacity = capacity;
            _array = new T[capacity];
        }


        public void Add(T item)
        {
            _array[_currentPointer = NextOf(_currentPointer)] = item;
            if (_currentPointer == _startPointer) _startPointer = NextOf(_startPointer);
            _endPointer = _currentPointer;
        }
        public T Forward()
        {
            if (CanForward)
                return _array[_currentPointer = PreviousOf(_currentPointer)];
            else
                throw new InvalidOperationException("There is no previous data.");
        }
        public T Backward()
        {
            if (CanBackward)
                return _array[_currentPointer = NextOf(_currentPointer)];
            else
                throw new InvalidOperationException("There is no next data.");
        }


        private int PreviousOf(int pointer)
        {
            return --pointer < 0 ? pointer + Capacity : pointer;
        }
        private int NextOf(int pointer)
        {
            return (pointer + 1) % Capacity;
        }
    }
}
