using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Zestaw_6
{
    public interface TreeVisitor
    {
        int VisitNode(Node node);
        int VisitLeaf(Leaf leaf);
    }

    public abstract class BinaryTree
    {
        public abstract int Accept(TreeVisitor visitor);
    }

    public class Leaf : BinaryTree
    {
        public override int Accept(TreeVisitor visitor)
        {
            return visitor.VisitLeaf(this);
        }
    }

    public class Node : BinaryTree
    {
        public BinaryTree Left, Right;
        public Node(BinaryTree left, BinaryTree right)
        {
            Left = left; Right = right;
        }
        public override int Accept(TreeVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
    }

    public class DepthVisitor : TreeVisitor
    {
        public int VisitNode(Node node)
        {
            int leftDepth = node.Left.Accept(this);
            int rightDepth = node.Right.Accept(this);
            return Max(leftDepth, rightDepth) + 1;
        }
        public int VisitLeaf(Leaf leaf)
        {
            return 0;
        }
        int Max(int a, int b)
        {
            return b > a ? b : a;
        }
    }




    [TestFixture]
    public class VisitorTest
    {
        [Test]
        public void Depth()
        {
            var visitor = new DepthVisitor();
            var root = new Node(
                           new Node(
                               new Node(
                                   new Leaf(),
                                   new Leaf()
                                ),
                                new Leaf()),
                           new Node(
                               new Leaf(),
                               new Leaf()
                               )
                          );
            int depth = root.Accept(visitor);
            Assert.AreEqual(3, depth);
        }
    }





}
