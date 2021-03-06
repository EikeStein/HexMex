using System;

namespace HexMex.Helper
{
    public class NoPathFoundException<TNode> : Exception
    {
        public TNode Destination { get; }
        public TNode Start { get; }

        public NoPathFoundException(string text, TNode start, TNode destination) : base(text)
        {
            Start = start;
            Destination = destination;
        }
    }
}