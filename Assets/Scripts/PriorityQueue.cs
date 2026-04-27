using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PriorityQueue<TElement, TPriority>
{

    /*
     * - **두 제네릭 타입**: 저장할 원소 타입 `TElement`와 우선순위 비교용 `TPriority`. 분리해 두면 원소 자체가 비교 가능할 필요가 없다 (예: `GraphNode`를 우선순위 `int`로 정렬).
        - 비교는 `Comparer<TPriority>.Default`로 수행. `int`, `string`, `float` 등 표준 타입은 자동으로 오름차순 비교자가 적용돼 **Min-Heap**이 된다.
        - 내부 저장소는 `List<(TElement Element, TPriority Priority)>` — **튜플**로 두 값을 엮어 저장.
        - `Dequeue`는 원소만 반환 — 우선순위는 내부 동작에 쓸 뿐 외부 반환값 아님.
     */

    private readonly IComparer<TPriority> comparer = Comparer<TPriority>.Default;
    public int Count => heap.Count; 


    private readonly List<(TElement Element, TPriority Priority)> heap =
        new List<(TElement Element, TPriority Priority)>();

    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority));
        int currentIndex = heap.Count - 1;

        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;
            if (comparer.Compare(heap[currentIndex].Priority, heap[parentIndex].Priority) >= 0) break;

            Swap(currentIndex, parentIndex);

            currentIndex = parentIndex;

        }

    }
    public TElement Dequeue()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Queue is empty.");
        }
        var element = heap[0].Element;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);

        int currentIndex = 0;
        while (currentIndex < heap.Count)
        {
            /*
             * 부모	(i - 1) / 2
                왼쪽 자식	2 * i + 1
                오른쪽 자식	2 * i + 2
             */
            int leftIndex = 2 * currentIndex + 1;
            int rightIndex = 2 * currentIndex + 2;

            if (leftIndex >= heap.Count) break; // 자식이 없으면 종료

            int minIndex = (rightIndex < heap.Count &&
                comparer.Compare(heap[rightIndex].Priority, heap[leftIndex].Priority) < 0) ?
                rightIndex : leftIndex;

            if (comparer.Compare(heap[currentIndex].Priority, heap[minIndex].Priority) <= 0) break; // 부모가 더 작으면 종료

            Swap(currentIndex, minIndex);
            currentIndex = minIndex;
        }
        return element;
    }
    public TElement Peek()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Queue is empty.");
        }
        return heap[0].Element;
    }
    public void Clear()
    {
        heap.Clear();
    }

    public bool IsEmpty()
    {
        return heap.Count == 0;
    }

    private void Swap(int indexA, int indexB)
    {
        var temp = heap[indexA];
        heap[indexA] = heap[indexB];
        heap[indexB] = temp;
    }


}