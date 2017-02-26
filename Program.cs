using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BinarySerialization;

namespace BinarySerializerFloatRepro
{
    class Program
    {
        static void Main(string[] args)
        {
            var container =
                new FloatContainer
                {
                    Value = -48.651363f
                };

            Console.WriteLine("source value: {0}", container.Value);

            var sourceBytes =
                BitConverter.GetBytes(container.Value)
                    .Reverse()
                    .ToArray();

            var serializer = new BinarySerializer() {Endianness = Endianness.Big};
            var stream = new MemoryStream();
            serializer.Serialize(stream, container);
            var serializedBytes = stream.ToArray();

            //compare at byte level
            if (sourceBytes.Length != serializedBytes.Length) throw new AmbiguousMatchException();
            for (var i = 0; i < sourceBytes.Length; i++)
                if (sourceBytes[i] != serializedBytes[i]) throw new AmbiguousMatchException($"byte {i}"); //fails here, i = 2
            
            var roundtrip = serializer.Deserialize<FloatContainer>(serializedBytes);

            Console.WriteLine("roundtrip value: {0}", roundtrip.Value);

            //compare at object level - warning complains about precision
            if (container.Value != roundtrip.Value) throw new AmbiguousMatchException();
        }
    }

    public class FloatContainer
    {
        [FieldOrder(0)]
        public float Value;
    }
}
