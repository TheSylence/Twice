using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Anotar.NLog;
using Newtonsoft.Json;
using Twice.Models.Scheduling.Processors;
using Twice.Models.Twitter;

namespace Twice.Models.Scheduling
{
	internal class Scheduler : IScheduler
	{
		public Scheduler( string fileName, ITwitterContextList contextList, ITwitterConfiguration twitterConfig )
		{
			FileName = fileName;
			JobProcessors.Add( SchedulerJobType.DeleteStatus, new DeleteStatusProcessor( contextList ) );
			JobProcessors.Add( SchedulerJobType.CreateStatus, new CreateStatusProcessor( contextList, twitterConfig ) );

			if( File.Exists( FileName ) )
			{
				var json = File.ReadAllText( FileName );
				try
				{
					Jobs.AddRange( JsonConvert.DeserializeObject<List<SchedulerJob>>( json ) );
				}
				catch( Exception ex )
				{
					LogTo.WarnException( "Failed to load joblist from file", ex );
				}
			}
		}

		internal Scheduler( string fileName, ITwitterContextList contextList, ITwitterConfiguration twitterConfig, IJobProcessor testProcessor )
			: this( fileName, contextList, twitterConfig )
		{
			JobProcessors.Add( SchedulerJobType.Test, testProcessor );
		}

		public void AddJob( SchedulerJob job )
		{
			lock( JobListLock )
			{
				Jobs.Add( job );
				UpdateJobList();
			}
		}

		public void Start()
		{
			LogTo.Info( "Starting scheduler thread" );

			IsRunning = true;
			ThreadObject = new Thread( RunThreaded );
			ThreadObject.Start();
		}

		public void Stop()
		{
			LogTo.Info( "Stopping scheduler thread" );

			IsRunning = false;
			ThreadObject?.Join();
		}

		internal bool ProcessJob( SchedulerJob job )
		{
			if( job.TargetTime <= DateTime.Now )
			{
				IJobProcessor processor;
				if( JobProcessors.TryGetValue( job.JobType, out processor ) )
				{
					processor.Process( job );
					return true;
				}
				LogTo.Warn( $"Unknown job type was scheduled ({job.JobType})" );
			}

			return false;
		}

		private void RunThreaded()
		{
			while( IsRunning )
			{
				lock( JobListLock )
				{
					for( int i = Jobs.Count - 1; i >= 0; --i )
					{
						var job = Jobs[i];

						if( ProcessJob( job ) )
						{
							Jobs.RemoveAt( i );
							UpdateJobList();
						}
					}
				}

				Thread.Sleep( 1000 );
			}
		}

		private void UpdateJobList()
		{
			var json = JsonConvert.SerializeObject( Jobs );
			File.WriteAllText( FileName, json );
		}

		internal IEnumerable<SchedulerJob> JobList => Jobs;
		private readonly string FileName;

		private readonly object JobListLock = new object();
		private readonly Dictionary<SchedulerJobType, IJobProcessor> JobProcessors = new Dictionary<SchedulerJobType, IJobProcessor>();
		private readonly List<SchedulerJob> Jobs = new List<SchedulerJob>();

		private bool IsRunning;
		private Thread ThreadObject;
	}
}