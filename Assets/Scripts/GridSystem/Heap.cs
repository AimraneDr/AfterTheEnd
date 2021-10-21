using System.Collections;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int CurrentItemsCount;
    public int Count {  get => CurrentItemsCount; }
    public Heap(int maxItemsCount) { items = new T[maxItemsCount]; }
    public bool Contains(T item) { return Equals(items[item.HeapIndex], item); }
    public void Add(T item)
    {
        item.HeapIndex = CurrentItemsCount;
        items[CurrentItemsCount] = item;
        SortUp(item);
        CurrentItemsCount++;
    }
    public T RemoveFirst()
    {
        T first_item = items[0];
        CurrentItemsCount--;
        items[0] = items[CurrentItemsCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return first_item;
    }
    public void UbdateItem(T item) { SortUp(item); }
    void SortDown(T item)
    {
        while (true)
        {
            int child_index_left = item.HeapIndex * 2 + 1;
            int child_index_right = item.HeapIndex * 2 + 2;
            int swap_index = 0;

            if (child_index_left < CurrentItemsCount)
            {
                swap_index = child_index_left;
                if (child_index_right < CurrentItemsCount)
                {
                    if (items[child_index_left].CompareTo(items[child_index_right]) < 0)
                    {
                        swap_index = child_index_right;
                    }
                }
                if (item.CompareTo(items[swap_index]) < 0)
                {
                    Swap(item, items[swap_index]);
                }
                else return;
            }
            else return;
        }
    }
    void SortUp(T item)
    {
        int parent_index = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parent_item = items[parent_index];
            if (item.CompareTo(parent_item) > 0)
            {
                Swap(item, parent_item);
            }
            else break;
            parent_index = (item.HeapIndex - 1) / 2;
        }
    }
    void Swap(T a,T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        int item_a_index = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = item_a_index;
    }
}