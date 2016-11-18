// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Diagnostics;

namespace L20nCoreTests
{
	namespace Common
	{
		/// <summary>
		/// A simple clock class to measure how much time it takes.
		/// </summary>
		/// <remarks>
		/// Doesn't seem very accurate.
		/// </remarks>
		public class PerformanceClock
		{
			private Stopwatch m_StopWatch;
			private String m_Tag;

			public PerformanceClock(string tag)
			{
				m_StopWatch = new Stopwatch();
				m_StopWatch.Start();
				m_Tag = tag;

				Clock("start performance timer");
			}

			public void Pause()
			{
				m_StopWatch.Stop();
			}

			public void Continue()
			{
				m_StopWatch.Start();
			}

			public void Clock(string description)
			{
				Console.WriteLine(
				String.Format("Clock: {0}: {1}ms: {2}",
			              m_Tag,
			              m_StopWatch.Elapsed.TotalMilliseconds,
			              description));
			}

			public void Stop()
			{
				Clock("stop performance timer");
				m_StopWatch.Stop();
			}
		}
	}
}
