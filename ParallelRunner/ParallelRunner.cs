using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelRunnerNs
{
	public static class ParallelRunner
	{
		private class TaskData
		{
			public Action<int> Task { get; private set; }
			public int Index;
			public int Count { get; private set; }

			public TaskData ( Action<int> task, int count )
			{
				Task = task;
				Count = count;
				Index = 0;
			}
		}

		private static void threadTask ( object d )
		{
			var td = ( TaskData ) d;
			while ( true )
			{
				int idx = Interlocked.Add ( ref td.Index, 1 ) - 1;

				if ( idx >= td.Count )
					return;
				td.Task ( idx );
			}
		}

		private static readonly int _processorCount = Environment.ProcessorCount;
		public static void RunInParallel ( Action<int> pt, int elementCount ) { RunInParallel ( pt, elementCount, _processorCount ); }
		public static void RunInParallel ( Action<int> pt, int elementCount, int threadCount )
		{
			if ( elementCount == 0 )
				return;

			if ( elementCount == 1 )
			{
				pt ( 0 );
				return;
			}

			if ( threadCount < 2 )
				throw new ArgumentException ( "Not enough threads to run something in parallel", "threadCount" );

			var td = new TaskData ( pt, elementCount );

			var tasks = new Task[ Math.Min ( elementCount, threadCount )-1 ];
			for ( int i = 0; i < tasks.Length; i++ )
			{
				tasks[ i ] = new Task ( threadTask, td );
				tasks[ i ].Start ();
			}

			threadTask ( td );
			//Task.WaitAll ( tasks, -1 );
			for ( int i = 0; i < tasks.Length; i++ )
				tasks[ i ].Wait ();
		}

		public static void SyncDo ( ref SpinLock sl, Action action )
		{
			bool gotLock = false;
			try
			{
				sl.Enter ( ref gotLock );
				action ();
			}
			finally
			{
				if ( gotLock )
					sl.Exit ();
			}
		}
	}
}
