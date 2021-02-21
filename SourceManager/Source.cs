using System;
using System.IO;
using System.Text;

namespace SourceManager
{
    public class Source
    {
        private TextReader Reader;

        public Source(string s) : this(new StringReader(s ?? ""))
        {
            
        }

        public Source(TextReader textReader)
        {
            Reader = textReader ?? throw new ArgumentNullException(nameof(textReader), $"A {nameof(Source)} cannot have a null {nameof(TextReader)}");
        }
    }
}
