namespace StrausTech.CommonLib.DataStructures
{
    public class LinkedList<T>
    {
        public Node<T>? Cursor { get; private set; }
        public Node<T>? Head { get; private set; }
        public Node<T>? Tail { get; private set; }

        public void AddToHead(T value)
        {
            var node = new Node<T>(value);

            if (Head == null || Head == default)
            {
                Head = node;
                Tail = node;
                return;
            }

            node.Next = Head;
            Head = node;
        }

        public void AddToTail(T value)
        {
            var node = new Node<T>(value);
            
            if (Tail == null || Tail == default)
            {
                Head = node;
                Tail = node;
                return;
            }

            Tail.Next = node;
            Tail = node;
        }

        public override string ToString()
        {
            var result = "";

            if (!Head.HasValue<Node<T>?>())
                return "No Items in Linked List";

            var node = Head;

            while (node.HasValue<Node<T>?>())
            {
                if (node.Value != null)
                {
                    result += $"[{node.Value.ToString()}]";
                    node = node.Next;
                    continue;
                }

                result += "[NO VALUE]";
                node = node.Next;
            }

            return result;
        }
    }

    public class Node<T>
    {
        public Node(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
        public Node<T>? Next { get; set; }

        public override string ToString()
        {
            if (Value == null)
                return "[NO VALUE]";

            return $"[{Value.ToString()}]";
        }
    }
}