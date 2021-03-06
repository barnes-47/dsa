﻿namespace Ds.Generic.Tree
{
    using Ds.Helper;
    using System;
    using System.Collections.Generic;

    public class BinarySearchTreeNode<T>
        where T : IComparable<T>
    {
        #region Public Properties
        public bool IsLeaf => Left == null && Right == null;
        public T Item { get; internal set; }
        public BinarySearchTreeNode<T> Parent { get; internal set; }
        public BinarySearchTreeNode<T> Left { get; internal set; }
        public BinarySearchTreeNode<T> Right { get; internal set; }
        #endregion

        #region Ctors
        public BinarySearchTreeNode(
            T item
            , BinarySearchTreeNode<T> parent)
        {
            Item = item;
            Parent = parent;
            Left = Right = null;
        }
        public BinarySearchTreeNode(
            T item
            , BinarySearchTreeNode<T> parent
            , BinarySearchTreeNode<T> left
            , BinarySearchTreeNode<T> right)
        {
            Item = item;
            Parent = parent;
            Left = left;
            Right = right;
        }
        #endregion

        #region Public Methods
        internal void Clear()
        {
            Parent = null;
            Left = null;
            Right = null;
            Item = default;
        }
        #endregion

        #region Overloaded Methods
        public override bool Equals(object obj)
        {
            if (!(obj is BinarySearchTreeNode<T> node))
                return false;

            if (ReferenceEquals(Item, node.Item))
                return true;

            return Item.Equals(node.Item);
        }
        public override int GetHashCode()
            => Item == null ? 0 : Item.GetHashCode();
        public int CompareTo(T other)
            => Item.CompareTo(other);
        #endregion

        #region Overloaded Operators
        public static bool operator ==(BinarySearchTreeNode<T> left, BinarySearchTreeNode<T> right)
            => ReferenceEquals(left, right) || (left?.Equals(right) ?? false);
        public static bool operator !=(BinarySearchTreeNode<T> left, BinarySearchTreeNode<T> right)
            => !(left == right);
        public static bool operator <(BinarySearchTreeNode<T> left, BinarySearchTreeNode<T> right)
            => left.Item.CompareTo(right.Item) < 0;
        public static bool operator >(BinarySearchTreeNode<T> left, BinarySearchTreeNode<T> right)
            => left.Item.CompareTo(right.Item) > 0;
        #endregion
    }
    public class BinarySearchTree<T>
        where T : IComparable<T>
    {
        #region Private Variables
        private IComparer<T> _comparer;
        #endregion

        #region Public Properties
        public BinarySearchTreeNode<T> Root { get; private set; }
        public BinarySearchTreeNode<T> Maximum
        {
            get
            {
                if (Root == null)
                    Throw.InvalidOperationException(Message.OnBinarySearchTree.RootIsNull);

                return GetRightMost(Root);
            }
        }
        public BinarySearchTreeNode<T> Minimum
        {
            get
            {
                if (Root == null)
                    Throw.InvalidOperationException(Message.OnBinarySearchTree.RootIsNull);

                return GetLeftMost(Root);
            }
        }
        public int Count { get; private set; }
        public bool IsEmpty => Count < 1 && Root == null;
        #endregion

        #region Ctors
        public BinarySearchTree()
        {
            _comparer = Comparer<T>.Default;
        }
        public BinarySearchTree(IComparer<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
        }
        public BinarySearchTree(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {

        }
        public BinarySearchTree(IEnumerable<T> collection, IComparer<T> comparer)
            : this(comparer)
        {
            if (collection == null)
                Throw.ArgumentNullException(nameof(collection));

            using var en = collection.GetEnumerator();
            while (en.MoveNext())
            {
                Add(en.Current);
            }
        }
        #endregion

        public int CompareTo(T other)
        {
            return CompareTo(other);
        }

        #region Public Methods

        /// <summary>Adds the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            if (IsEmpty)
            {
                Root = AddParent(item);
                ++Count;

                return;
            }

            AddTo(item, Root);
            ++Count;
        }

        /// <summary>Removes the specified item.</summary>
        /// <param name="item">The item.</param>
        public void Delete(T item)
        {
            var nodeToBeDeleted = Find(item);
            if (nodeToBeDeleted == null)
                return;

            if (nodeToBeDeleted.IsLeaf)     // deletes a leaf-node.
            {
                if (nodeToBeDeleted.Parent.Left == nodeToBeDeleted)
                    nodeToBeDeleted.Parent.Left = null;
                if (nodeToBeDeleted.Parent.Right == nodeToBeDeleted)
                    nodeToBeDeleted.Parent.Right = null;

                nodeToBeDeleted.Clear();
                --Count;

                return;
            }
            if (nodeToBeDeleted.Left != null && nodeToBeDeleted.Right == null)  // deletes a node that has only left child.
            {
                if (nodeToBeDeleted.Parent.Left == nodeToBeDeleted)
                    nodeToBeDeleted.Parent.Left = nodeToBeDeleted.Left;
                else
                    nodeToBeDeleted.Parent.Right = nodeToBeDeleted.Left;

                nodeToBeDeleted.Left.Parent = nodeToBeDeleted.Parent;
                nodeToBeDeleted.Clear();
                --Count;

                return;
            }
            if (nodeToBeDeleted.Left == null && nodeToBeDeleted.Right != null)  // deletes a node that has only right child.
            {
                if (nodeToBeDeleted.Parent.Left == nodeToBeDeleted)
                    nodeToBeDeleted.Parent.Left = nodeToBeDeleted.Right;
                else
                    nodeToBeDeleted.Parent.Right = nodeToBeDeleted.Right;

                nodeToBeDeleted.Right.Parent = nodeToBeDeleted.Parent;
                nodeToBeDeleted.Clear();
                --Count;

                return;
            }

            // deletes a node that has 2 children (including root).
            var successor = InOrderSuccessor(nodeToBeDeleted);  // gets the in-order successor of the node to be deleted.
            if (successor == null)
                return;

            nodeToBeDeleted.Item = successor.Item;
            if (successor.Right == null)
            {
                if (successor.Parent.Left == successor)
                    successor.Parent.Left = null;
                else
                    successor.Parent.Right = null;
            }
            else
            {
                if (successor.Parent.Left == successor)
                    successor.Parent.Left = successor.Right;
                else
                    successor.Parent.Right = successor.Right;
                successor.Right.Parent = successor.Parent;
            }

            successor.Clear();
            --Count;
        }

        /// <summary>Determines whether this instance contains the object.</summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
            => Find(item) != null;

        /// <summary>Gets the In-Order predecessor of the node passed.</summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public BinarySearchTreeNode<T> InOrderPredecessor(BinarySearchTreeNode<T> node)
        {
            if (node == null)
                return null;

            if (node.Left != null)
                return GetRightMost(node.Left);

            var predecessor = node.Parent;
            while (predecessor != null && node == predecessor.Left)
            {
                node = predecessor;
                predecessor = predecessor.Parent;
            }

            return predecessor;
        }

        /// <summary>Gets the In-Order successor of the node passed.</summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public BinarySearchTreeNode<T> InOrderSuccessor(BinarySearchTreeNode<T> node)
        {
            if (node == null)
                return null;
            if (node.Right != null)
                return GetLeftMost(node.Right);

            var successor = node.Parent;
            while (successor != null && node == successor.Right)
            {
                node = successor;
                successor = successor.Parent;
            }

            return successor;
        }

        /// <summary>Implements the In-Order tree walk.</summary>
        /// <param name="action">The action.</param>
        /// <param name="node">The node.</param>
        public void InOrderTreeWalk(Action<T> action)
            => InOrderTreeWalk(action, Root);

        /// <summary>Implements the Post-Order tree walk.</summary>
        /// <param name="action">The action.</param>
        /// <param name="node">The node.</param>
        public void PostOrderTreeWalk(Action<T> action)
            => PostOrderTreeWalk(action, Root);

        /// <summary>Get the Pre-Order predecessor of the node passed.</summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public BinarySearchTreeNode<T> PreOrderPredecessor(BinarySearchTreeNode<T> node)
        {
            if (node == null || node.Parent == null)
                return null;
            if (node.Parent.Left == null)
                return node.Parent;

            var predecessor = node.Parent.Left;
            while (predecessor.Right != null || predecessor.Left != null)
            {
                if (predecessor.Right != null)
                {
                    predecessor = predecessor.Right;
                    continue;
                }
                if (predecessor.Left != null)
                {
                    predecessor = predecessor.Left;
                    continue;
                }
            }

            return predecessor;
        }

        /// <summary>Get the Pre-Order successor of the node passed.</summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public BinarySearchTreeNode<T> PreOrderSuccessor(BinarySearchTreeNode<T> node)
        {
            if (node == null)
                return null;
            if (node.Left != null)
                return node.Left;
            if (node.Right != null)
                return node.Right;

            var successor = node.Parent;
            while (successor != null)
            {
                if (successor.Right != null && node == successor.Left)
                    return successor.Right;

                node = successor;
                successor = successor.Parent;
            }

            return null;
        }

        /// <summary>Implements the Pre-Order tree walk.</summary>
        /// <param name="action">The action.</param>
        /// <param name="node">The node.</param>
        public void PreOrderTreeWalk(Action<T> action)
            => PreOrderTreeWalk(action, Root);
        #endregion

        #region Private Methods
        /// <summary>Adds the parent.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private BinarySearchTreeNode<T> AddParent(T item)
            => new BinarySearchTreeNode<T>(item, null);

        /// <summary>Adds the child.</summary>
        /// <param name="item">The item.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        private BinarySearchTreeNode<T> AddChild(T item, BinarySearchTreeNode<T> parent)
            => new BinarySearchTreeNode<T>(item, parent);

        /// <summary>Adds to the binary tree.</summary>
        /// <param name="item">The item.</param>
        /// <param name="node">The node.</param>
        private void AddTo(T item, BinarySearchTreeNode<T> node)
        {
            var isAdded = false;
            while (!isAdded)
            {
                if (item.CompareTo(node.Item) < 0)
                {
                    if (node.Left == null)
                    {
                        node.Left = AddChild(item, node);
                        isAdded = true;
                        continue;
                    }

                    node = node.Left;
                    continue;
                }

                if (node.Right == null)
                {
                    node.Right = AddChild(item, node);
                    isAdded = true;
                    continue;
                }

                node = node.Right;
            }
        }

        /// <summary>Finds the specified item.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private BinarySearchTreeNode<T> Find(T item)
        {
            var current = Root;
            while (current != null)
            {
                if (item.CompareTo(current.Item) < 0)
                {
                    current = current.Left;
                    continue;
                }
                if (item.CompareTo(current.Item) > 0)
                {
                    current = current.Right;
                    continue;
                }

                return current;
            }

            return null;
        }

        /// <summary>Gets the left most node.</summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private BinarySearchTreeNode<T> GetLeftMost(BinarySearchTreeNode<T> node)
        {
            while (node.Left != null)
            {
                node = node.Left;
            }

            return node;
        }

        /// <summary>Gets the right most node.</summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public BinarySearchTreeNode<T> GetRightMost(BinarySearchTreeNode<T> node)
        {
            while (node.Right != null)
            {
                node = node.Right;
            }

            return node;
        }

        /// <summary>Implements the In-Order tree walk.</summary>
        /// <param name="action">The action.</param>
        /// <param name="node">The node.</param>
        private void InOrderTreeWalk(Action<T> action, BinarySearchTreeNode<T> node)
        {
            if (node == null)
                return;

            InOrderTreeWalk(action, node.Left);
            action(node.Item);
            InOrderTreeWalk(action, node.Right);
        }

        /// <summary>Implements the Post-Order tree walk.</summary>
        /// <param name="action">The action.</param>
        /// <param name="node">The node.</param>
        private void PostOrderTreeWalk(Action<T> action, BinarySearchTreeNode<T> node)
        {
            if (node == null)
                return;

            PostOrderTreeWalk(action, node.Left);
            PostOrderTreeWalk(action, node.Right);
            action(node.Item);
        }

        /// <summary>Implements the Pre-Order tree walk.</summary>
        /// <param name="action">The action.</param>
        /// <param name="node">The node.</param>
        private void PreOrderTreeWalk(Action<T> action, BinarySearchTreeNode<T> node)
        {
            if (node == null)
                return;

            action(node.Item);
            PreOrderTreeWalk(action, node.Left);
            PreOrderTreeWalk(action, node.Right);
        }
        #endregion
    }
}
