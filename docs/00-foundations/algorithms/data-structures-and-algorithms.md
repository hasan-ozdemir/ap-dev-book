---
title: Data Structures & Algorithms Playbook
description: Practical reference for foundational and advanced data structures & algorithms with .NET 9 ready C# examples.
last_reviewed: 2025-11-03
owners:
  - @prodyum/language-guild
---

# Data Structures & Algorithms Playbook

Modern Prodyum projects—from MAUI mobile apps to cloud services—depend on a core toolkit of data structures and algorithms. Industry refresher guides published for 2025 consistently rank arrays, hash maps, heaps, graphs, and dynamic programming among the must-know topics for software engineers, especially when tackling interviews or production performance bottlenecks.citeturn1search1turn1search4turn3search5

## Table of contents

1. [Complexity primer](#1-complexity-primer)
2. [Core collections](#2-core-collections)
3. [Tree-based structures](#3-tree-based-structures)
4. [Heap, priority, and disjoint sets](#4-heap-priority-and-disjoint-sets)
5. [Graph representations](#5-graph-representations)
6. [String and trie-based structures](#6-string-and-trie-based-structures)
7. [Sorting algorithms](#7-sorting-algorithms)
8. [Searching and selection](#8-searching-and-selection)
9. [Graph algorithms](#9-graph-algorithms)
10. [Dynamic programming and greedy patterns](#10-dynamic-programming-and-greedy-patterns)
11. [Advanced and emerging topics](#11-advanced-and-emerging-topics)

---

## 1. Complexity primer

Big-O notation expresses how runtime or memory grow as input size increases. Internalise the common classes below and track both time and space in code reviews so refactors do not regress performance.citeturn2search8

| Complexity | Typical example | Notes |
|------------|-----------------|-------|
| O(1) | Hash lookup | Constant work regardless of n. |
| O(log n) | Binary search | Divide-and-conquer halving the problem. |
| O(n) | Linear scan | Work grows proportionally with input. |
| O(n log n) | Efficient sorting | Optimal for comparison-based sorts. |
| O(n²) | Nested loops (naive DP) | Acceptable only for small inputs. |
| O(2ⁿ) | Brute-force subsets | Avoid unless n is tiny. |

Keep the Big-O cheat sheet handy when comparing implementations; it lists amortised costs for .NET collections such as `Dictionary<TKey,TValue>` and `List<T>`.citeturn2search0

---

## 2. Core collections

Arrays (`Span<T>`/`Memory<T>`), dynamic arrays (`List<T>`), linked lists, stacks, queues, and hash tables underpin nearly every business feature—from MAUI list rendering to task scheduling.citeturn1search1turn2search0turn3search0

- **Dynamic arrays (`List<T>`)**: amortised O(1) append and O(1) index access; trim capacity after heavy insert/remove cycles to release memory.  
- **Linked lists (`LinkedList<T>`)**: O(1) insert/remove when you already have a node reference, but O(n) to locate nodes.  
- **Stacks/queues**: O(1) pushes/pops; use `ConcurrentQueue<T>` for producer/consumer workloads.  
- **Hash maps/sets (`Dictionary`, `HashSet`)**: expected O(1) lookup/update; collisions degrade to O(n), so choose good hash functions.

```csharp
static IReadOnlyDictionary<char, int> BuildHistogram(ReadOnlySpan<char> text)
{
    var counts = new Dictionary<char, int>();
    foreach (var ch in text)
    {
        if (!char.IsLetter(ch)) continue;
        counts.TryGetValue(ch, out var current);
        counts[ch] = current + 1;
    }
    return counts;
}
```

Document complexity in XML comments so future teams know why these structures were chosen over alternatives.citeturn3search0

---

## 3. Tree-based structures

Balanced trees maintain O(log n) operations even as datasets grow, making them ideal for ordered lookup, range queries, and indexing.citeturn1search1turn3search5

- **Red-Black / AVL trees**: Self-balancing BSTs powering .NET’s `SortedDictionary` and `SortedSet`.  
- **B-Trees / B+ Trees**: Optimised for disk and page cache workloads (databases, file systems).  
- **Segment/Fenwick trees**: Support O(log n) range queries and point updates in analytics and game leaderboards.

```csharp
var availability = new SortedDictionary<DateTime, int>();
availability[DateTime.UtcNow] = 42;
if (availability.TryGetValue(DateTime.UtcNow, out var seats))
{
    Console.WriteLine($"Seats available: {seats}");
}
```

Always write invariants (e.g., height difference ≤ 1 for AVL) into unit tests if you implement custom trees.citeturn3search5

---

## 4. Heap, priority, and disjoint sets

Priority queues drive schedulers, top-k dashboards, and Dijkstra’s algorithm. .NET’s `PriorityQueue<TElement,TPriority>` (updated in .NET 9) lets you enqueue, update, and remove arbitrary items while maintaining a binary heap internally.citeturn1search5turn1search4

```csharp
var tasks = new PriorityQueue<string, int>();
tasks.Enqueue("ship hotfix", 0);
tasks.Enqueue("cleanup temp files", 10);
while (tasks.TryDequeue(out var job, out _))
{
    Console.WriteLine(job);
}
```

Union-Find (Disjoint Set Union) delivers almost O(1) connectivity checks with path compression and union by rank—critical for Kruskal’s MST and incremental graph clustering.citeturn3search5turn1search1

---

## 5. Graph representations

Most production graphs are sparse, so prefer adjacency lists (O(V + E) storage) over matrices (O(V²)). Edge lists are useful when streaming data straight into Kruskal’s algorithm.citeturn3search5

```csharp
var graph = new Dictionary<string, List<(string neighbor, int weight)>>
{
    ["SEA"] = new() { ("SFO", 2), ("LAX", 4) },
    ["SFO"] = new() { ("LAX", 1) }
};
```

Normalize IDs (map strings to integers) to shrink memory footprints before running intensive algorithms.citeturn3search5

---

## 6. String and trie-based structures

Tries provide O(k) insert/search by character, making them perfect for autocomplete, dictionary apps, and command palettes. Advanced string algorithms—KMP, Rabin-Karp, suffix arrays—power log analysis and DNA search.citeturn3search5turn0search5

```csharp
sealed class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; } = new();
    public bool IsWord { get; set; }
}

static void Insert(TrieNode root, ReadOnlySpan<char> word)
{
    var node = root;
    foreach (var ch in word)
    {
        node = node.Children.TryGetValue(ch, out var next)
            ? next
            : node.Children[ch] = new TrieNode();
    }
    node.IsWord = true;
}
```

Precompute rolling hashes for large text comparisons to detect plagiarism or duplicates in linear time.citeturn0search5

---

## 7. Sorting algorithms

Know how .NET’s hybrid Timsort works and still practice implementing quicksort, mergesort, heapsort, counting sort, and radix sort. Each has trade-offs around stability, memory, and worst-case behaviour.citeturn0search5turn1search1turn2search0

```csharp
static void QuickSort(Span<int> span)
{
    if (span.Length <= 1) return;
    int pivot = span[span.Length / 2];
    int left = 0, right = span.Length - 1;
    while (left <= right)
    {
        while (span[left] < pivot) left++;
        while (span[right] > pivot) right--;
        if (left <= right)
        {
            (span[left], span[right]) = (span[right], span[left]);
            left++; right--;
        }
    }
    QuickSort(span[..right + 1]);
    QuickSort(span[left..]);
}
```

Benchmark sorts against realistic distributions; bad pivot strategies push quicksort toward O(n²).citeturn0search5

---

## 8. Searching and selection

Binary search offers O(log n) lookups on sorted data, while quickselect finds the k-th element in average O(n) time. Expand your toolkit with ternary search for unimodal functions and exponential search for unbounded ranges.citeturn3search5turn0search5

```csharp
static int BinarySearch(ReadOnlySpan<int> data, int target)
{
    int lo = 0, hi = data.Length - 1;
    while (lo <= hi)
    {
        int mid = lo + ((hi - lo) / 2);
        if (data[mid] == target) return mid;
        if (data[mid] < target) lo = mid + 1;
        else hi = mid - 1;
    }
    return -1;
}
```

Guard binary searches with assertions that inputs are sorted; mishandling invariants is a common production bug.citeturn3search5

---

## 9. Graph algorithms

Prioritise BFS/DFS, Dijkstra, Bellman-Ford, Kruskal, Prim, and topological sort—these power routing, dependency resolution, and analytics features.citeturn3search5turn0search5

```csharp
static int[] Dijkstra(List<(int To, int Weight)>[] graph, int source)
{
    var dist = Enumerable.Repeat(int.MaxValue, graph.Length).ToArray();
    var pq = new PriorityQueue<int, int>();
    dist[source] = 0;
    pq.Enqueue(source, 0);
    while (pq.TryDequeue(out var u, out _))
    {
        foreach (var (v, weight) in graph[u])
        {
            var candidate = dist[u] + weight;
            if (candidate < dist[v])
            {
                dist[v] = candidate;
                pq.Enqueue(v, candidate);
            }
        }
    }
    return dist;
}
```

Use adjacency lists and pooled buffers to reduce allocations during heavy graph traversals.citeturn1search5turn3search5

---

## 10. Dynamic programming and greedy patterns

Dynamic programming caches subproblems (LIS, knapsack, edit distance) to avoid exponential blowups, while greedy techniques (activity selection, Huffman coding) exploit ordering or monotonicity to stay O(n log n) or better.citeturn0search5turn3search5

```csharp
static int LongestIncreasingSubsequence(ReadOnlySpan<int> nums)
{
    Span<int> tails = stackalloc int[nums.Length];
    var size = 0;
    foreach (var value in nums)
    {
        var position = Array.BinarySearch(tails[..size].ToArray(), value);
        if (position < 0) position = ~position;
        tails[position] = value;
        if (position == size) size++;
    }
    return size;
}
```

Comment recurrence relations and base cases directly above DP code so future maintainers understand the state transition logic.citeturn3search5

---

## 11. Advanced and emerging topics

Stay curious—researchers are using AI to discover new heuristics (AlphaDev/AlphaEvolve) and to design structures like Linked Array Trees that promise sub-linear lookups in massive datasets.citeturn0search13turn2academia12

When you adopt novel algorithms, benchmark under production workloads, provide fallbacks, and document dependencies (runtime version, third-party packages) to maintain reliability.citeturn0search13turn2academia12
