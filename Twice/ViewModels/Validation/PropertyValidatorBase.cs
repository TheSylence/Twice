namespace Twice.ViewModels.Validation
{
	internal abstract class PropertyValidatorBase
	{
		protected PropertyValidatorBase( bool isManual = false )
		{
			IsManual = isManual;
		}

		public void Clear()
		{
			Error = null;
			HasError = false;
		}

		public abstract void Update( object value );

		public string Error { get; protected set; }
		public bool HasError { get; protected set; }
		public bool IsManual { get; }
	}
}