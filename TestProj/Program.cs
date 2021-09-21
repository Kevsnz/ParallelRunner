using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using ParallelRunnerNs;

namespace TestProj
{
	internal class Program
	{
		internal static void Main ( string[] args )
		{
			Console.WriteLine ( "Welcome!" );

			var timer = new Stopwatch ();
			var rnd = new Random ( 1 );

			var data = new double[ 5000 ];
			for ( int i = 0; i < data.Length; i++ )
				data[ i ] = rnd.NextDouble () * 6 - 3;

			var res = new double[ data.Length ];

			timer.Start ();

			/*for ( int i = 0; i < data.Length; i++ ) // ~ 125k ticks
				res[ i ] = func ( data[ i ] ); //*/

			//ParallelRunner.RunInParallel ( i => res[ i ] = func ( data[ i ] ), data.Length ); // ~90k ticks


			/*for ( int k = 0; k < 10000; k++ ) // ~11.9M ticks
				for ( int i = 0; i < data.Length; i++ )
					res[ i ] = func ( data[ i ] ); //*/

			/*for ( int k = 0; k < 10000; k++ ) // ~5.0M ticks
				ParallelRunner.RunInParallel ( i => res[ i ] = func ( data[ i ] ), data.Length ); //*/

			ParallelRunner.RunInParallel ( i => res[ i / 10000 ] = func ( data[ i / 10000 ] ), data.Length * 10000 ); // ~4.5M ticks ??????????????????????? */

			timer.Stop ();

			Console.WriteLine ( "Result: " + res.Sum () );
			Console.WriteLine ( "Time elapsed: " + ( timer.ElapsedTicks * 1000.0 / Stopwatch.Frequency ).ToString ( "0.000000" ) + " ms (" + timer.ElapsedTicks + " ticks)." );

			Console.WriteLine ( "Done!" );
			Console.ReadKey ( true );
		}

		private static double func ( double val ) { return Math.Sqrt ( Math.Cos ( Math.Tanh ( val ) ) ); }
	}
}
