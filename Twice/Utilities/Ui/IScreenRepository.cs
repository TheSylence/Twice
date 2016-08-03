namespace Twice.Utilities.Ui
{
	internal interface IScreenRepository
	{
		IVirtualScreen GetScreenFromPosition( double x, double y );
	}
}