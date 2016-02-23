using System;
using System.IO;
using System.Text.RegularExpressions;

namespace L20n
{
	namespace IO
	{

		public class CharStream
		{
			public delegate bool CharPredicate(char c);
	
			private String m_Buffer = null;
			private int m_LastPosition;
			private int m_Position;

			public int Position
			{
				get { return m_Position; }
			}

			public CharStream(String path)
			{
				using (var sr = new StreamReader(path, System.Text.Encoding.UTF8, false))
				{
					m_Buffer = sr.ReadToEnd();
					m_LastPosition = m_Buffer.Length - 1;
					m_Position = 0;
				}
			}

			public bool ReadNext(out char c)
			{
				if(EndOfStream())
				{
					c = '\0';
					return false;
				}

				c = m_Buffer[m_Position++];
				return true;
			}

			public char PeekNext()
			{
				if(EndOfStream())
				{
					return '\0';
				}

				return m_Buffer[m_Position];
			}

			public char ForceReadNext(string msg)
			{
				char c;
				if(!ReadNext(out c)) // TODO: finalize this when we have decent error handling
					throw new IOException(msg);
				return c;
			}

			public void SkipCharacter(char expected)
			{
				char c;
				if (!ReadNext (out c))
					throw new IOException ("Couldn't find char in SkipCharacter: " + expected);
				if (c != expected)
					throw new IOException ("Skip has an unexpected character");
			}

			public void SkipWhile(CharPredicate pred)
			{
				while (!EndOfStream() && pred(m_Buffer[m_Position]))
					++m_Position;
			}

			public bool SkipIfPossible(char expected)
			{
				if (PeekNext () == expected) {
					SkipCharacter(expected);
					return true;
				}

				return false;
			}

			public bool ReadReg(string reg, out string c)
			{
				var rx = new Regex("^" + reg);
				var match = rx.Match(m_Buffer, m_Position);

				if (match.Success) {
					c = match.Value;
					m_Position += c.Length;
					return true;
				}
				
				c = null;
				return false;
				
			}
			
			public string ReadUntilEnd()
			{
				string content = m_Buffer.Substring (m_Position);
				m_Position = m_LastPosition + 1;
				return content;
			}

			public bool EndOfStream(bool keepWhiteSpace = false)
			{
				return m_Position > m_LastPosition;
			}

			public bool InputLeft()
			{
				if (EndOfStream ())
					return false;

				var re = new Regex (@"[^\s]+");
				return re.IsMatch(m_Buffer, m_Position);
			}
		}
	}
}
