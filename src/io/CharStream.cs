using System;
using System.IO;

namespace L20n
{
	namespace IO
	{
		public class CharStream : IDisposable
		{
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

			public bool ReadNext(out char c, bool keepWhiteSpace = false)
			{
				if(EndOfStream(keepWhiteSpace))
				{
					c = '\0';
					return false;
				}

				bool lastResult;
				do {
					c = m_Buffer [m_Position++];
				} while(!(lastResult = IsValidChar(c, keepWhiteSpace))
				        && m_Position <= m_LastPosition);

				return lastResult;
			}

			public char PeekNext()
			{
				if(EndOfStream())
				{
					return '\0';
				}

				return m_Buffer [m_Position];
			}

			public char ForceReadNext(string msg)
			{
				char c;
				if(!ReadNext(out c))
					throw new IOException(msg);
				return c;
			}

			public bool Skip(char expected)
			{
				char c;
				if(!ReadNext(out c))
					return false;
				return c == expected;
			}

			public bool SkipIfPossible(char expected)
			{
				if (PeekNext () == expected) {
					Skip(expected);
					return true;
				}

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
				SkipInvalidChars(keepWhiteSpace);
				return m_Position > m_LastPosition;
			}

			public void Dispose()
			{
				if (!EndOfStream())
				{
					throw new IOException("Forgot to read: " + ReadUntilEnd());
				}
			}

			private void SkipInvalidChars(bool keepWhiteSpace = false)
			{
				while (m_Position <= m_LastPosition
				      && !IsValidChar(m_Buffer[m_Position], keepWhiteSpace)) {
					m_Position++;
				}
			}

			private bool IsValidChar(char c, bool keepWhiteSpace = false)
			{
				return (keepWhiteSpace || !Char.IsWhiteSpace (c));
			}
		}
	}
}
