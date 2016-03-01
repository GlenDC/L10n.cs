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
	
			private readonly string m_Path = null;
			private readonly int m_LastPosition;
			private String m_Buffer = null;
			private int m_Position;

			public string Path
			{
				get { return m_Path; }
			}

			public int Position
			{
				get { return m_Position; }
			}
			
			public CharStream(String buffer, string path = null)
			{
				m_Path = path;
				m_Buffer = buffer;
				m_Position = 0;
				m_LastPosition = m_Buffer.Length - 1;
			}

			public static CharStream CreateFromFile(String path)
			{
				using (var sr = new StreamReader(path, System.Text.Encoding.UTF8, false))
				{
					return new CharStream(sr.ReadToEnd(), path);
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

			public bool ReadNextRange(int len, out string s)
			{
				if(EndOfStream() || (m_Position+len) > m_LastPosition)
				{
					s = null;
					return false;
				}

				s = m_Buffer.Substring(m_Position, len);
				m_Position += len;
				return true;
			}

			public char PeekNext(int offset = 0)
			{
				if(EndOfStream())
				{
					return '\0';
				}

				int pos = m_Position + offset;
				if (pos > m_LastPosition)
					return '\0';

				return m_Buffer[pos];
			}

			public string PeekNextRange(int length, int offset = 0)
			{
				int pos = m_Position + offset;
				if(EndOfStream() || (pos+length) > m_LastPosition)
					return null;

				return m_Buffer.Substring (pos, length);
			}

			public bool PeekReg(string reg)
			{
				var re = new Regex(reg);
				var match = re.Match(m_Buffer, m_Position);

				return match.Success && (match.Index == m_Position);
			}

			public char ForceReadNext(string msg = null)
			{
				if (msg == null)
					msg = "expected to read a char, but reached EOF instead";

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
					throw CreateException(String.Format ("expected {0}, got {1}", expected, c));
			}

			public void SkipAnyCharacter(char[] expected)
			{
				char c;

				if (!ReadNext (out c))
					throw CreateEOFException();

				for (int i = 0; i < expected.Length; ++i) {
					if (expected [i] == c) {
						return;
					}
				}

				throw CreateException(String.Format ("expected {0}, got {1}", expected, c));
			}

			public void SkipString(string expected)
			{
				string s;
				if (!this.ReadNextRange(expected.Length, out s))
					throw CreateEOFException();
				if (s != expected)
					throw CreateException(String.Format ("expected {0}, got {1}", expected, s));
			}


			public int SkipWhile(CharPredicate pred)
			{
				int skipped = 0;
				while (!EndOfStream() && pred(m_Buffer[m_Position])) {
					++m_Position;
					++skipped;
				}

				return skipped;
			}

			public bool SkipIfPossible(char expected)
			{
				if (PeekNext () == expected) {
					SkipCharacter(expected);
					return true;
				}

				return false;
			}

			public bool SkipAnyIfPossible(char[] expected, out char c)
			{
				c = PeekNext();
				for (int i = 0; i < expected.Length; ++i) {
					if(c == expected[i]) {
						++m_Position;
						return true;
					}
				}

				return false;
			}

			public bool ReadReg(string reg, out string c)
			{
				var re = new Regex(reg);
				var match = re.Match(m_Buffer, m_Position);

				if (match.Success && match.Index == m_Position) {
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

			public bool EndOfStream()
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

			public L20n.Exceptions.ParseException CreateException(string msg, int offset = 0)
			{
				int pos = m_Position + offset;
				if (pos >= m_LastPosition) {
					return new L20n.Exceptions.ParseException(
						String.Format("parsing error: {0}", msg));
				}

				return new L20n.Exceptions.ParseException(
					String.Format("'{0}' at {1} is unexpected: {2}",
				              m_Buffer[pos], ComputeDetailedPosition(pos), msg));
			}

			public L20n.Exceptions.ParseException CreateEOFException()
			{
				return new L20n.Exceptions.ParseException(
					"end of stream was reached while more input was expected");
			}

			public void Dispose() {}
		}
	}
}
