/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 - 2017 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace L20n
{
	namespace IO
	{

		public class CharStream : IDisposable
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
				if (!ReadNext (out c))
					throw new IOException(msg, CreateEOFException());
				return c;
			}

			public void SkipCharacter(char expected)
			{
				char c;
				if (!ReadNext (out c))
					throw CreateEOFException();
				if (c != expected)
					throw CreateException(String.Format ("expected {0}", expected));
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

			public string ComputeDetailedPosition(int pos) {
				var re = new Regex(@"(\r\n|\n|\r)");
				var matches = re.Matches(m_Buffer.Substring (0, pos));
				int lineNumber = 1;
				int linePosition = pos;
				if (matches.Count > 0) {
					lineNumber = matches.Count + 1;
					var match = matches [matches.Count - 1];
					linePosition -= match.Index + match.Length - 1;
				}

				return String.Format("L{0}:{1}", lineNumber, linePosition);
			}

			public IOException CreateException(string msg, int offset = 0)
			{
				int pos = m_Position + offset;
				return new IOException(
					String.Format("'{0}' at {1} is unexpected: {2}",
				              m_Buffer[pos], ComputeDetailedPosition(pos), msg));
			}

			public IOException CreateEOFException()
			{
				return new IOException("end of stream was reached while more input was expected");
			}

			public void Dispose() {}
		}
	}
}
