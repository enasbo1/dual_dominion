using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shared
{
    public abstract class Manager<TDealer, TEnum, TVariant> : MonoBehaviour where TDealer : Dealer<TEnum, TVariant>
        where TEnum : Enum
        where TVariant : Enum
    {
        protected readonly TableList<TDealer> Elements = new(0);

        protected TableArray<bool> Active = new(0);
        protected int Size { get; private set; }

        public void InitializeChunk(int size = 50)
        {
            Elements.AddChunk(size);
            Active.AddChunk(size);
            AddChunk(size);
        }

        protected abstract void AddChunk(int size);


        public void AddElement(TDealer element)
        {
            int i = Elements.FindIndex(d => d == element);
            if (i != -1)
            {
                Active[i] = true;
                RestoreElement(i, element);
                return;
            }


            if (Active.Count > Size)
            {
                Elements[Size] = element;
                Active[Size] = true;
                try
                {
                    AddElementInChunk(element);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new Exception(
                        $"Element add in Chunk Failed: {element},\nThe issue is probably from the AddChunk method one of the managers");
                }
            }
            else
            {
                Elements.Add(element);
                Active.Add(true);
                AddElementInNew(element);
            }

            InitElement(element);
            ++Size;
        }

        protected virtual void RestoreElement(int i, TDealer element)
        {
        }

        protected virtual void InitElement(TDealer element)
        {
        }

        protected abstract void AddElementInChunk(TDealer element);
        protected abstract void AddElementInNew(TDealer element);


        protected virtual void DisableElement(int index)
        {
        }

        public void DisableElement(TDealer element)
        {
            int i = Elements.FindIndex(d => d == element);
            if (i != -1)
                Active[i] = false;
        }
    }

    public struct TableList<TValues> : ITable<List<TValues>, TValues>
    {
        public List<TValues> Values { get; set; }
        public int Count => Values.Count;

        public TableList(int size = 0)
        {
            Values = new List<TValues>(size);
        }

        public readonly void Add(TValues value = default)
        {
            Values.Add(value);
        }


        public readonly void AddChunk(int size)
        {
            Values.AddRange(new TValues[size]);
        }

        public readonly int FindIndex(Predicate<TValues> predicate)
        {
            return Values.FindIndex(predicate);
        }

        public readonly TValues this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }
    }

    public struct TableArray<TValues> : ITable<TValues[], TValues>
    {
        private readonly bool _keepValues;
        public int Count => Values.Length;

        public TValues[] Values { get; set; }

        public TableArray(int size = 0, bool keepValues = true)
        {
            _keepValues = keepValues;
            Values = size == 0 ? Array.Empty<TValues>() : new TValues[size];
        }

        public void Add(TValues value = default)
        {
            Values = Values.Append(value).ToArray();
        }

        public void AddChunk(int size)
        {
            Values = _keepValues ? Values.Concat(new TValues[size]).ToArray() : new TValues[Values.Length + size];
        }


        public TValues this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }
    }

    public interface ITable<T, TV> : ITable where T : IList<TV>
    {
        public TV this[int index] { get; set; }
        public T Values { get; set; }
        public void Add(TV value = default);
    }

    public interface ITable
    {
        public void AddChunk(int size);
    }
}