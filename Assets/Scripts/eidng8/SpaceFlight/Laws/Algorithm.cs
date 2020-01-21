// ---------------------------------------------------------------------------
// <copyright file="Maths.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Managers;
using Unity.Collections;
using Unity.Entities;

namespace eidng8.SpaceFlight.Laws
{
    public static class NativeArrayExtensions
    {
        public static int BinarySearch<T>(this NativeArray<T> array, T search)
            where T : struct, IComparable<T> {
            int lower = 0;
            int upper = array.Length - 1;
            if (0 == upper) {
                if (array[0].Equals(search)) { return 0; }

                return -1;
            }

            // Rider suggests to use loop instead of a tail recursion,
            // which is reasonably well. Here we go.
            while (true) {
                if (upper < lower) {
                    // We reach here when element is not present in array 
                    return -1;
                }

                int mid = lower + (upper - lower) / 2;

                // If the element is present at the 
                // middle itself 
                if (array[mid].Equals(search)) { return mid; }

                // If element is smaller than mid, then 
                // it can only be present in lower sub-array 
                if (array[mid].CompareTo(search) < 0) {
                    upper = mid - 1;
                    continue;
                }

                // Else the element can only be present 
                // in upper sub-array 
                lower = mid + 1;
            }
        }

        public static int BinarySearch(
            this NativeArray<PrefabComponent> array,
            int type
        ) {
            return array.BinarySearch(
                new PrefabComponent((PrefabTypes)type, Entity.Null)
            );
        }
    }
}
