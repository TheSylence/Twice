using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	/// <summary>
	/// Base class for a behavior that handles proper unloading.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BehaviorBase<T> : Behavior<T> where T : FrameworkElement
	{
		protected override void OnChanged()
		{
			var target = AssociatedObject;
			if( target != null )
			{
				HookupBehavior( target );
			}
			else
			{
				UnHookupBehavior();
			}
		}

		protected virtual void OnCleanup()
		{
		}

		protected virtual void OnSetup()
		{
		}

		private void CleanupBehavior()
		{
			if( !IsSetup ) return;
			IsSetup = false;
			OnCleanup();
		}

		private void HookupBehavior( T target )
		{
			if( IsHookedUp ) return;
			WeakTarget = new WeakReference( target );
			IsHookedUp = true;
			target.Unloaded += OnTarget_Unloaded;
			target.Loaded += OnTarget_Loaded;
			SetupBehavior();
		}

		private void OnTarget_Loaded( object sender, RoutedEventArgs e )
		{
			SetupBehavior();
		}

		private void OnTarget_Unloaded( object sender, RoutedEventArgs e )
		{
			CleanupBehavior();
		}

		private void SetupBehavior()
		{
			if( IsSetup ) return;
			IsSetup = true;
			OnSetup();
		}

		private void UnHookupBehavior()
		{
			if( !IsHookedUp ) return;
			IsHookedUp = false;
			var target = AssociatedObject ?? (T)WeakTarget.Target;
			if( target != null )
			{
				target.Unloaded -= OnTarget_Unloaded;
				target.Loaded -= OnTarget_Loaded;
			}
			CleanupBehavior();
		}

		private bool IsHookedUp;
		private bool IsSetup = true;
		private WeakReference WeakTarget;
	}
}