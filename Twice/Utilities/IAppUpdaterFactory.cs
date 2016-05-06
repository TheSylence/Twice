namespace Twice.Utilities
{
	internal interface IAppUpdaterFactory
	{
		IAppUpdater Construct( string url );
	}
}