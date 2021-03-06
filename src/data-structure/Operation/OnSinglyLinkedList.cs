﻿namespace Ds.Operation
{
    using Ds.Generic.LinkedList;
    using Ds.Helper;
    using System.Collections.Generic;
    using DsGeneric = Generic;

    public static class OnSinglyLinkedList
    {
        #region Internal Extension Validations
        internal static void InternalValidation<T>(this Singly<T> list)
        {
            if (list.IsEmpty)
                Throw.ArgumentNullException(nameof(list));
        }
        #endregion

        #region Public Helper Extension Methods for Singly Linked List
        public static SinglyNode<T> GetNode<T>(this Singly<T> list, T item)
        {
            if (list.IsEmpty)
                return null;

            var comparer = EqualityComparer<T>.Default;
            var current = list.Head;
            while (current != null)
            {
                if (comparer.Equals(current.Item, item))
                    return current;
                current = current.Next;
            }

            return null;
        }
        public static void SetupLoop<T>(this Singly<T> list, T item)
        {
            var node = list.GetNode(item);
            if (node == null)
                return;
            list.Tail.Next = node;
        }
        #endregion

        #region Geeks For Geeks Extension Methods
        public static bool IsPalindrome<T>(this Singly<T> list)
        {
            if (list.IsEmpty)
                return false;

            var comparer = EqualityComparer<T>.Default;
            if (!comparer.Equals(list.Head.Item, list.Tail.Item))
                return false;
            if (list.Count == 1)
                return true;
            if (list.Count == 2 || list.Count == 3)
                return comparer.Equals(list.Head.Item, list.Tail.Item);

            // First get the middle node of the list and then add all the item of the second half of the list to stack.
            var middle = list.InternalMiddleNode();
            var secondHalfStart = (list.Count & 1) == 1 ? middle.Next : middle;
            var items = new DsGeneric.Stack<T>();
            while (secondHalfStart != null)
            {
                items.Push(secondHalfStart.Item);
                secondHalfStart = secondHalfStart.Next;
            }

            // Compare first half of the list with stack elements by poping them.
            var current = list.Head;
            while (current != middle)
            {
                if (!comparer.Equals(current.Item, items.Pop()))
                    return false;
                current = current.Next;
            }
            items.Clear();

            return true;
        }
        public static bool LoopExists<T>(this Singly<T> list)
            => list.InternalHareTortoiseMeetNode() != null;
        public static int LoopLength<T>(this Singly<T> list)
        {
            var node = list.InternalHareTortoiseMeetNode();
            if (node == null)
                return 0;

            var length = 1;
            var temp = node.Next;
            while (temp != node)
            {
                temp = temp.Next;
                ++length;
            }

            return length;
        }
        public static SinglyNode<T> LoopStartNode<T>(this Singly<T> list)
        {
            var meetingNode = list.InternalHareTortoiseMeetNode();
            if (meetingNode == null)
                return null;

            var current = list.Head;
            meetingNode = meetingNode.Next;
            while (current != meetingNode)
            {
                current = current.Next;
                meetingNode = meetingNode.Next;
            }

            return current;
        }
        public static void MergeSort(this Singly<int> unsortedList)
        {
            if (unsortedList.Count < 2)
                return;

            unsortedList.Head = unsortedList.Head.InternalMergeSort();
            unsortedList.InternalSetupSinglyAfterMergeSort();
        }
        /// <summary>
        /// Merges 2 sorted lists.
        /// NOTE: Both lists must be sorted in order for this method to work correctly.
        /// </summary>
        /// <param name="sortedList1">The sorted list one.</param>
        /// <param name="sortedList2">The sorted list two.s</param>
        /// <returns>A new list which is a combination of both the sorted lists.</returns>
        public static Singly<int> MergeSort(this Singly<int> sortedList1, Singly<int> sortedList2)
        {
            if (sortedList1.IsEmpty && sortedList2.IsEmpty)
                return null;
            if (sortedList1.IsEmpty && !sortedList2.IsEmpty)
                return sortedList2;
            if (!sortedList1.IsEmpty && sortedList2.IsEmpty)
                return sortedList1;

            var singly = new Singly<int>
            {
                Count = sortedList1.Count + sortedList2.Count,
                Head = InternalMerge(sortedList1.Head, sortedList2.Head)
            };
            if (sortedList1.Tail.Item > sortedList2.Tail.Item)
                singly.Tail = sortedList1.Tail;
            else
                singly.Tail = sortedList2.Tail;

            return singly;
        }
        public static SinglyNode<T> MiddleNode<T>(this Singly<T> list)
        {
            if (list.IsEmpty)
                return null;

            return list.InternalMiddleNode();
        }
        public static SinglyNode<T> NthNode<T>(this Singly<T> list, int nTh)
        {
            InternalValidationForListAndIndex(list, nTh);

            var current = list.Head;
            while(--nTh != 0)
            {
                current = current.Next;
            }

            return current;
        }
        public static SinglyNode<T> NthNodeFromEnd<T>(this Singly<T> list, int nTh)
        {
            nTh = list.Count + 1 - nTh;

            return list.NthNode(nTh);
        }
        public static void Reverse<T>(this Singly<T> list)
        {
            if (list.IsEmpty)
                return;
            if (list.Head.Next == null)
                return;

            SinglyNode<T> prev = null, current = list.Head, next = list.Head.Next;
            list.Tail = list.Head;
            while (next != null)
            {
                current.Next = prev;
                prev = current;
                current = next;
                next = current.Next;
            }
            current.Next = prev;
            prev = current;
            list.Head = prev;
        }
        #endregion

        #region Private Extension Methods
        private static SinglyNode<T> InternalHareTortoiseMeetNode<T>(this Singly<T> list)
        {
            if (list.IsEmpty)
                return null;
            if (list.Head.Next == null)
                return null;
            if (list.Head.Next == list.Head)
                return list.Head;

            var tortoise = list.Head;
            var hare = list.Head.Next;
            while (hare != null)
            {
                if (hare == tortoise)
                    return hare;

                hare = hare.Next;
                if (hare == null)
                    break;
                
                tortoise = tortoise.Next;
                hare = hare.Next;
            }

            return null;
        }
        private static SinglyNode<int> InternalMergeSort(this SinglyNode<int> node)
        {
            if (node == null)
                Throw.ArgumentNullException(nameof(node));
            if (node.Next == null)
                return node;

            var middle = node.InternalMiddleNode();
            var secondHalf = middle.Next;
            middle.Next = null;

            var left = InternalMergeSort(node);
            var right = InternalMergeSort(secondHalf);

            return InternalMerge(left, right);
        }
        private static SinglyNode<T> InternalMiddleNode<T>(this Singly<T> list)
        {
            if (list.IsEmpty)
                Throw.ArgumentNullException(nameof(list));
            if (list.Head.Next == null || list.Head.Next.Next == null)
                return list.Head;

            var hare = list.Head;
            var tortoise = list.Head;
            while (hare.Next != null)
            {
                hare = hare.Next;
                if (hare.Next == null)
                    break;

                hare = hare.Next;
                tortoise = tortoise.Next;
            }

            return tortoise;
        }
        private static SinglyNode<T> InternalMiddleNode<T>(this SinglyNode<T> head)
        {
            if (head == null)
                Throw.ArgumentNullException(nameof(head));
            if (head.Next == null || head.Next.Next == null)    // when list has 1 or 2 nodes only.
                return head;

            var hare = head;
            var tortoise = head;
            while (hare != null)
            {
                hare = hare.Next;
                if (hare == null)
                    break;

                hare = hare.Next;
                tortoise = tortoise.Next;
            }

            return tortoise;
        }
        private static void InternalSetupSinglyAfterMergeSort(this Singly<int> list)
        {
            var current = list.Head;
            list.Count = 1;
            while(current.Next != null)
            {
                current = current.Next;
                ++list.Count;
            }
            list.Tail = current;
        }
        #endregion

        #region Private Methods
        private static SinglyNode<int> InternalMerge(SinglyNode<int> left, SinglyNode<int> right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;

            SinglyNode<int> node;
            if (left.Item > right.Item)
            {
                node = right;
                node.Next = InternalMerge(left, right.Next);
            }
            else
            {
                node = left;
                node.Next = InternalMerge(left.Next, right);
            }

            return node;
        }
        private static void InternalValidationForListAndIndex<T>(Singly<T> list, int nTh)
        {
            if (list == null)
                Throw.ArgumentNullException(nameof(list));
            if (nTh < 1)
                Throw.ArgumentOutOfRangeException(nameof(nTh), Message.OnSinglyLinkedList.NthMustBePositiveNonZero);
            if (nTh > list.Count)
                Throw.ArgumentOutOfRangeException(nameof(nTh), Message.OnSinglyLinkedList.NthCannotBeGreaterThanListCount);
        }
        #endregion
    }
}
