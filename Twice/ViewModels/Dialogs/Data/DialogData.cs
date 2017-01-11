using System;
using System.Diagnostics;

namespace Twice.ViewModels.Dialogs.Data
{
	internal abstract class DialogData : IEquatable<DialogData>
	{
		protected DialogData( Type controlType, Type viewModelType )
		{
			ControlType = controlType;
			ViewModelType = viewModelType;
		}

		public abstract bool Equals( DialogData obj );

		public override bool Equals( object obj )
		{
			var data = obj as DialogData;
			if( data == null )
			{
				return false;
			}

			return Equals( data );
		}

		public override int GetHashCode()
		{
			int hash = 397;
			unchecked
			{
				hash = hash * 23 + ControlType.GetHashCode();
				hash = hash * 23 + ViewModelType.GetHashCode();
				//? Do we need to ensure that inherited classes offer a hash code?
			}
			return hash;
		}
		
		public abstract object GetResult( object viewModel );

		public abstract void Setup( object viewModel );

		protected TViewModel CastViewModel<TViewModel>( object viewModel ) where TViewModel : class, IDialogViewModel
		{
			Debug.Assert( typeof( TViewModel ).IsAssignableFrom( ViewModelType ) );

			return viewModel as TViewModel;
		}

		public Type ControlType { get; }
		public Type ViewModelType { get; }
	}
}