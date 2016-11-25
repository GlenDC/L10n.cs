// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Text.RegularExpressions;

using L20nCore.Common.Exceptions;

namespace L20nCore
{
	namespace Common
	{
		namespace IO
		{
			/// <summary>
			/// <see cref="L20nCore.Common.IO.CharStream"/> is a utility class to make the
			/// parsing logic easier and keep the streams-specific logic centralized
			/// and seperated from the actual parser logic.
			/// </summary>
			public class CharStream : IDisposable
			{
				/// <summary>
				/// Returns the path to the actual resources this instance is streaming.
				/// </summary>
				public string Path
				{
					get { return m_Path; }
				}

				/// <summary>
				/// Returns the current position in the stream.
				/// </summary>
				public int Position
				{
					get { return m_Position; }
				}

				/// <summary>
				/// Creates a <see cref="L20nCore.Common.IO.CharStream"/> instance based on a given string buffer.
				/// </summary>
				public CharStream(String buffer, string path = null)
				{
					m_Path = path;
					m_Buffer = buffer;
					m_Position = 0;
					m_LastPosition = m_Buffer.Length - 1;
				}

				/// <summary>
				/// Creates a <see cref="L20nCore.Common.IO.CharStream"/> instance with the buffered content
				/// read from the resource found at the given path.
				/// </summary>
				public static CharStream CreateFromFile(String path)
				{
					using (var sr = IO.StreamReaderFactory.Create(path))
					{
						return new CharStream(sr.ReadToEnd(), path);
					}
				}

				/// <summary>
				/// Outputs the current character, and moves the stream to the next position.
				/// Returns false in case no more characters were left.
				/// </summary>
				public bool ReadNext(out char c)
				{
					if (EndOfStream())
					{
						c = '\0';
						return false;
					}

					c = m_Buffer [m_Position++];
					return true;
				}

				/// <summary>
				/// Outputs the next <c>length</c> amount of characters and skips the stream that much ahead.
				/// Returns false in case more characters were requested than were available.
				/// </summary>
				public bool ReadNextRange(int length, out string s)
				{
					if (EndOfStream() || (m_Position + length) > m_LastPosition + 1)
					{
						s = null;
						return false;
					}

					s = m_Buffer.Substring(m_Position, length);
					m_Position += length;
					return true;
				}

				/// <summary>
				/// Returns the current or offsetted character without mutating the stream position.
				/// Returns the null `\0` in case that isn't possible.
				/// </summary>
				public char PeekNext(int offset = 0)
				{
					if (EndOfStream())
					{
						return '\0';
					}

					int pos = m_Position + offset;
					if (pos > m_LastPosition)
						return '\0';

					return m_Buffer [pos];
				}

				/// <summary>
				/// Returns the <c>length</c> amount of characters on the current or offset position.
				/// Returns <c>null</c> in case that isn't possible.
				/// </summary>
				public string PeekNextRange(int length, int offset = 0)
				{
					int pos = m_Position + offset;
					if (EndOfStream() || (pos + length) > (m_LastPosition + 1))
						return null;

					return m_Buffer.Substring(pos, length);
				}

				/// <summary>
				/// Returns <c>true</c> if the given regular expression
				/// matches the stream at the current position.
				/// </summary>
				public bool PeekReg(Regex reg)
				{
					var match = reg.Match(m_Buffer, m_Position);

					return match.Success && (match.Index == m_Position);
				}

				/// <summary>
				/// Read the current character in the stream, and move its position afterwards.
				/// Throws a <see cref="L20nCore.Common.Exceptions.IOException"/>
				/// in case no characters were left in the stream.
				/// </summary>
				public char ForceReadNext(string msg = null)
				{
					char c;
					if (!ReadNext(out c))
					{
						if (msg == null)
							msg = "expected to read a char, but reached EOF instead";
						throw new IOException(msg, CreateEOFException());
					}

					return c;
				}

				/// <summary>
				/// Skip <c>n</c> amount of characters, without caring what characters you skip
				/// Throws an exception in case not enough characters are left in the stream.
				/// </summary>
				public void Skip(int n = 1)
				{
					if (n + Position > m_LastPosition)
					{
						throw CreateException(String.Format(
			"Tried to skip {0} characters, but only {1} are left",
			n, m_LastPosition - Position));
					}

					m_Position += n;
				}

				/// <summary>
				/// Skip the current position, and go to the next position of the stream.
				/// Throws an exception in case no character was left in the stream or
				/// in case it doesn't match the <c>expected</c> character.
				/// </summary>
				public void SkipCharacter(char expected)
				{
					char c;
					if (!ReadNext(out c))
						throw CreateEOFException();
					if (c != expected)
						throw CreateException(String.Format("expected {0}, got {1}", expected, c));
				}

				/// <summary>
				/// Skip the current position, and go to the next position of the stream.
				/// Throws an exception in case no character was left in the stream or
				/// in case it doesn't match one of the <c>expected</c> characters.
				/// </summary>
				public void SkipAnyCharacter(char[] expected)
				{
					char c;

					if (!ReadNext(out c))
						throw CreateEOFException();

					for (int i = 0; i < expected.Length; ++i)
					{
						if (expected [i] == c)
						{
							return;
						}
					}

					throw CreateException(String.Format("expected {0}, got {1}", expected, c));
				}

				/// <summary>
				/// Skips the /x/ next characters, where /x/ is equal to the length of the given <c>expected</c> string.
				/// Throws an exception in case no characters were left in the stream or
				/// in case the skipped string doesn't match the <c>expected</c> string.
				/// </summary>
				public void SkipString(string expected)
				{
					string s;
					if (!this.ReadNextRange(expected.Length, out s))
						throw CreateEOFException();
					if (s != expected)
						throw CreateException(String.Format("expected {0}, got {1}", expected, s));
				}

				/// <summary>
				/// Skips characters in the stream, starting at the current position in the stream,
				/// and stops at the first position, for which the character at that position,
				/// returns <c>false</c> for the given <c>predicate</c>.
				/// Returns the amount of characters that have been skipped.
				/// </summary>
				public int SkipWhile(CharPredicate predicate)
				{
					int skipped = 0;
					while (!EndOfStream() && predicate(m_Buffer[m_Position]))
					{
						++m_Position;
						++skipped;
					}

					return skipped;
				}

				/// <summary>
				/// Checks if the current character in the stream is equal to the <c>expected</c> character.
				/// If that's the case it will move the position of the stream by 1, and return <c>true</c>.
				/// <c>false</c> will be returned otherwise.
				/// </summary>
				public bool SkipIfPossible(char expected)
				{
					if (PeekNext() == expected)
					{
						SkipCharacter(expected);
						return true;
					}

					return false;
				}

				/// <summary>
				/// Checks if the current character in the stream is equal to any of the <c>expected</c> characters.
				/// If that's the case it will move the position of the stream by 1, and return <c>true</c>.
				/// <c>false</c> will be returned otherwise.
				/// </summary>
				public bool SkipAnyIfPossible(char[] expected, out char c)
				{
					c = PeekNext();
					for (int i = 0; i < expected.Length; ++i)
					{
						if (c == expected [i])
						{
							++m_Position;
							return true;
						}
					}

					return false;
				}

				/// <summary>
				/// Outputs a string, starting at the current position and based on a given <c>reg</c> expression.
				/// Offsets the stream afterwards by the length of the output string,
				/// and return <c>true</c> in case the regex was mathed.
				/// Returns <c>false</c> otherwise.
				/// </summary>
				public bool ReadReg(Regex reg, out string c)
				{
					var match = reg.Match(m_Buffer, m_Position);

					if (match.Success && match.Index == m_Position)
					{
						c = match.Value;
						m_Position += c.Length;
						return true;
					}
	
					c = null;
					return false;
				}

				/// <summary>
				/// Outputs a string, starting at the current position and based on a given <c>reg</c> expression.
				/// Offsets the stream afterwards by the length of the output string,
				/// and return the matched value in case the regex was mathed.
				/// Throws a <see cref="L20nCore.Common.Exceptions.ParseException"/> otherwise.
				/// </summary>
				public string ForceReadReg(Regex reg)
				{
					string value;
					if (!ReadReg(reg, out value))
						throw CreateException(string.Format("regex `{0}` could not be matched", reg));
					return value;
				}

				/// <summary>
				/// Return all characters left in the stream in the form of a string,
				/// and offset the position of the stream 1 position beyond the last position of the stream.
				/// </summary>
				public string ReadUntilEnd()
				{
					string content = m_Buffer.Substring(m_Position);
					m_Position = m_LastPosition + 1;
					return content;
				}

				/// <summary>
				/// Keeps reading the stream until EOF was reached,
				/// or until the given predicate returns <c>false</c>.
				/// </summary>
				public string ReadWhile(CharPredicate predicate)
				{
					int startPosition = m_Position;
					while (!EndOfStream() && predicate(m_Buffer[m_Position]))
					{
						++m_Position;
					}

					return m_Buffer.Substring(startPosition, m_Position - startPosition);
				}

				/// <summary>
				/// Returns <c>true</c> if the stream has no more characters left,
				/// <c>false</c> otherwise.
				/// </summary>
				public bool EndOfStream()
				{
					return m_Position > m_LastPosition;
				}

				/// <summary>
				/// Similar to the <c>EndOfStream</c> method but also checks
				/// if there are also non-whitespace characters left in the stream.
				/// </summary>
				public bool InputLeft()
				{
					if (EndOfStream())
						return false;

					return s_ReNoWhitespace.IsMatch(m_Buffer, m_Position);
				}

				/// <summary>
				/// Computes a user-friendly position that gives both the Line and Column number,
				/// based on the given linear stream position.
				/// Result gets returned in a formatted string.
				/// </summary>
				public string ComputeDetailedPosition(int pos)
				{
					var matches = s_ReNewline.Matches(m_Buffer.Substring(0, pos));
					int lineNumber = 1;
					int linePosition = pos;
					if (matches.Count > 0)
					{
						lineNumber = matches.Count + 1;
						var match = matches [matches.Count - 1];
						linePosition -= match.Index + match.Length - 1;
					}

					return String.Format("L{0}:{1}", lineNumber, linePosition);
				}

				/// <summary>
				/// Returns an exception with a given or default message,
				/// for either the current or offset position.
				/// </summary>
				public ParseException CreateException(string msg, int offset = 0)
				{
					int pos = m_Position + offset;
					if (pos >= m_LastPosition)
					{
						return new ParseException(
			String.Format("parsing error: {0}", msg));
					}

					return new ParseException(
		String.Format("'{0}' at {1} is unexpected: {2}",
	              m_Buffer [pos], ComputeDetailedPosition(pos), msg));
				}

				/// <summary>
				/// Returns a <see cref="L20nCore.Common.Exceptions.ParseException"/> notifying the user
				/// that the end of the stream has been reached, while more input was expected.
				/// </summary>
				public ParseException CreateEOFException()
				{
					return new ParseException(
		"end of stream was reached while more input was expected");
				}

				/// <summary>
				/// Doesn't do anything, but implemented to satisfy the <c>IDisposable</c> interface.
				/// </summary>
				public void Dispose()
				{
				}

				// used to allow the user of this class to define its own predicate given a char.
				public delegate bool CharPredicate(char c);

				// the path to the resource to be streamed
				private readonly string m_Path = null;
				// the index of the last position
				private readonly int m_LastPosition;
				// the buffer object containing all the chars to be "streamed"
				private String m_Buffer = null;
				// the current position in the buffer
				private int m_Position;
				// regex used to check if any input is left;
				private static readonly Regex s_ReNoWhitespace = new Regex(@"[^\s]+");
				private static readonly Regex s_ReNewline = new Regex(@"(\r\n|\n|\r)");
			}
		}
	}
}
