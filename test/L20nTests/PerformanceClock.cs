/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
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
using System.Diagnostics;

namespace L20nCoreTests
{
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
