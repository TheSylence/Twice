namespace Twice.ViewModels.Validation
{
	internal abstract class PropertyValidatorBase
	{
		protected PropertyValidatorBase( bool isManual = false )
		{

			IsManual = isManual;
		}
		public bool IsManual { get; }

		public void Clear()
		{
			Error = null;
			HasError = false;
		}

		public abstract void Update( object value );

		public string Error { get; protected set; }
		public bool HasError { get; protected set; }
	}
}