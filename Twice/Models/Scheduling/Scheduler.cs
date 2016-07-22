using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

			JobIdCounter = Jobs.Any()
				? Jobs.Max( j => j.JobId ) + 1
				: 0;
		}

		internal Scheduler( string fileName, ITwitterContextList contextList, ITwitterConfiguration twitterConfig,
			IJobProcessor testProcessor )
			: this( fileName, contextList, twitterConfig )
		{
			JobProcessors.Add( SchedulerJobType.Test, testProcessor );
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

				lock( JobListLock )
				{
					JobListUpdated?.Invoke( this, EventArgs.Empty );
				}

				Thread.Sleep( 1000 );
			}
		}

		private void UpdateJobList()
		{
			var json = JsonConvert.SerializeObject( Jobs );
			File.WriteAllText( FileName, json );
		}

		public event EventHandler JobListUpdated;

		public void AddJob( SchedulerJob job )
		{
			Debug.Assert( job.AccountIds.Any() );
			Debug.Assert( !string.IsNullOrEmpty( job.Text ) );

			lock( JobListLock )
			{
				job.JobId = JobIdCounter++;

				Jobs.Add( job );
				UpdateJobList();
			}

			lock( JobListLock )
			{
				JobListUpdated?.Invoke( this, EventArgs.Empty );
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

		public IEnumerable<SchedulerJob> JobList => Jobs;
		private readonly string FileName;
		private readonly object JobListLock = new object();

		private readonly Dictionary<SchedulerJobType, IJobProcessor> JobProcessors =
			new Dictionary<SchedulerJobType, IJobProcessor>();

		private readonly List<SchedulerJob> Jobs = new List<SchedulerJob>();
		private bool IsRunning;
		private ulong JobIdCounter;
		private Thread ThreadObject;
	}
}