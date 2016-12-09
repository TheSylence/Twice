using System;

namespace Twice.ViewModels.Dialogs.Data
{
	abstract class DialogData : IEquatable<DialogData>
	{
		protected DialogData( Type controlType, Type viewModelType )
		{
			ControlType = controlType;
			ViewModelType = viewModelType;
		}

		public Type ControlType{get; }
		public Type ViewModelType { get; }

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
	}
}