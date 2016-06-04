using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Twice.Views
{
	/// <summary>
	///     Contains attached properties to activate Trigger Tracing on the specified Triggers. This file
	///     alone should be dropped into your app.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class TriggerTracing
	{
		static TriggerTracing()
		{
			// Initialise WPF Animation tracing and add a TriggerTraceListener
			PresentationTraceSources.Refresh();
			PresentationTraceSources.AnimationSource.Listeners.Clear();
			PresentationTraceSources.AnimationSource.Listeners.Add( new TriggerTraceListener() );
			PresentationTraceSources.AnimationSource.Switch.Level = SourceLevels.All;
		}

		/// <summary>
		///     Gets a value indication whether trace is enabled for the specified trigger.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		/// <returns></returns>
		public static bool GetTraceEnabled( TriggerBase trigger )
		{
			return (bool)trigger.GetValue( TraceEnabledProperty );
		}

		/// <summary>
		///     Gets the trigger name for the specified trigger. This will be used to identify the
		///     trigger in the debug output.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		/// <returns></returns>
		public static string GetTriggerName( TriggerBase trigger )
		{
			return (string)trigger.GetValue( TriggerNameProperty );
		}

		/// <summary>
		///     Sets a value specifying whether trace is enabled for the specified trigger
		/// </summary>
		/// <param name="trigger"></param>
		/// <param name="value"></param>
		public static void SetTraceEnabled( TriggerBase trigger, bool value )
		{
			trigger.SetValue( TraceEnabledProperty, value );
		}

		/// <summary>
		///     Sets the trigger name for the specified trigger. This will be used to identify the
		///     trigger in the debug output.
		/// </summary>
		/// <param name="trigger">The trigger.</param>
		/// <param name="value">Name of the trigger.</param>
		/// <returns></returns>
		public static void SetTriggerName( TriggerBase trigger, string value )
		{
			trigger.SetValue( TriggerNameProperty, value );
		}

		private static void OnTraceEnabledChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var triggerBase = d as TriggerBase;

			if( triggerBase == null )
				return;

			if( !( e.NewValue is bool ) )
				return;

			if( (bool)e.NewValue )
			{
				// insert dummy story-boards which can later be traced using WPF animation tracing

				var storyboard = new TriggerTraceStoryboard( triggerBase, TriggerTraceStoryboardType.Enter );
				triggerBase.EnterActions.Insert( 0, new BeginStoryboard {Storyboard = storyboard} );

				storyboard = new TriggerTraceStoryboard( triggerBase, TriggerTraceStoryboardType.Exit );
				triggerBase.ExitActions.Insert( 0, new BeginStoryboard {Storyboard = storyboard} );
			}
			else
			{
				// remove the dummy storyboards

				foreach( TriggerActionCollection actionCollection in new[] {triggerBase.EnterActions, triggerBase.ExitActions} )
				{
					foreach( TriggerAction triggerAction in actionCollection )
					{
						BeginStoryboard bsb = triggerAction as BeginStoryboard;

						if( bsb?.Storyboard is TriggerTraceStoryboard )
						{
							actionCollection.Remove( bsb );
							break;
						}
					}
				}
			}
		}

		public static readonly DependencyProperty TraceEnabledProperty =
			DependencyProperty.RegisterAttached(
				"TraceEnabled",
				typeof(bool),
				typeof(TriggerTracing),
				new UIPropertyMetadata( false, OnTraceEnabledChanged ) );

		public static readonly DependencyProperty TriggerNameProperty =
			DependencyProperty.RegisterAttached(
				"TriggerName",
				typeof(string),
				typeof(TriggerTracing),
				new UIPropertyMetadata( string.Empty ) );

		private enum TriggerTraceStoryboardType
		{
			Enter,
			Exit
		}

		/// <summary>
		///     A custom tracelistener.
		/// </summary>
		private class TriggerTraceListener : TraceListener
		{
			public override void TraceEvent( TraceEventCache eventCache, string source, TraceEventType eventType, int id,
				string format, params object[] args )
			{
				base.TraceEvent( eventCache, source, eventType, id, format, args );

				if( format.StartsWith( "Storyboard has begun;" ) )
				{
					TriggerTraceStoryboard storyboard = args[1] as TriggerTraceStoryboard;
					if( storyboard != null )
					{
						// add a breakpoint here to see when your trigger has been entered or exited

						// the element being acted upon
						object targetElement = args[5];

						// the namescope of the element being acted upon
						INameScope namescope = (INameScope)args[7];

						TriggerBase triggerBase = storyboard.TriggerBase;
						string triggerName = GetTriggerName( storyboard.TriggerBase );

						Debug.WriteLine(
							$"Element: {targetElement}, {triggerBase.GetType().Name}: {triggerName}: {storyboard.StoryboardType}" );
					}
				}
			}

			public override void Write( string message )
			{
			}

			public override void WriteLine( string message )
			{
			}
		}

		/// <summary>
		///     A dummy storyboard for tracing purposes
		/// </summary>
		private class TriggerTraceStoryboard : Storyboard
		{
			public TriggerTraceStoryboard( TriggerBase triggerBase, TriggerTraceStoryboardType storyboardType )
			{
				TriggerBase = triggerBase;
				StoryboardType = storyboardType;
			}

			public TriggerTraceStoryboardType StoryboardType { get; }
			public TriggerBase TriggerBase { get; }
		}
	}
}