namespace JsonMGITester
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    public class JsonStreamReader
    {
        private readonly Stack<byte> nesting = new Stack<byte>();

        private readonly Stream input;

        private bool inQuotes;

        private bool escaped;

        public JsonStreamReader(Stream input)
        {
            this.input = input;
        }

        public object ReadObject()
        {
            var text = this.ReadNextJson();
            return JsonConvert.DeserializeObject(text);
        }

        public T ReadObject<T>()
        {
            var text = this.ReadNextJson();
            return JsonConvert.DeserializeObject<T>(text);
        }

        private string ReadNextJson()
        {
            var memory = new MemoryStream();
            var ready = false;
            while (!ready)
            {
                var b = this.input.ReadByte();
                if (b == -1)
                {
                    throw new EndOfStreamException();
                }

                memory.WriteByte((byte)b);
                if (b == '\\')
                {
                    if (this.inQuotes)
                    {
                        this.escaped = !this.escaped;
                    }

                    continue;
                }

                if (b == '"')
                {
                    if (!this.escaped)
                    {
                        this.inQuotes = !this.inQuotes;
                    }

                    this.escaped = false;
                    continue;
                }

                this.escaped = false;
                if (this.inQuotes)
                {
                    continue;
                }

                if (b == '{' || b == '[')
                {
                    this.nesting.Push((byte)b);
                }
                else if (b == '}' || b == ']')
                {
                    var nested = this.nesting.Pop();
                    if (nested != b - 2)
                    {
                        throw new FormatException(
                            string.Format("Invalid nesting, got '{0}' but expected '{1}'", (char)b, (char)(nested + 2)));
                    }

                    ready = this.nesting.Count == 0;
                }
            }

            var text = Encoding.ASCII.GetString(memory.ToArray());
            return text;
        }
    }
}