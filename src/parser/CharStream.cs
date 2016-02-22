using System;
using System.IO;

namespace L20n
{
	public class CharStream : IDisposable
	{
		private String m_Buffer = null;
		private int m_LastPosition;
		private int m_Position;

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
			if(m_Position > m_LastPosition)
			{
				c = '\0';
				return false;
			}

			bool lastResult;
			do {
				c = m_Buffer [m_Position++];
			} while(!(lastResult = IsValidChar(c)) && m_Position <= m_LastPosition);

			return lastResult;
		}

		public void Dispose() {}

		private bool IsValidChar(char c)
		{
			return !Char.IsWhiteSpace(c);
		}
	}
}
