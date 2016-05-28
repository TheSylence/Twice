namespace Twice.Utilities
{
	internal interface ITimerFactory
	{
		ITimer Create( int timeout );
	}
}