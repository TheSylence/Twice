namespace Twice.Utilities
{
	interface ITimerFactory
	{
		ITimer Create( int timeout );
	}
}