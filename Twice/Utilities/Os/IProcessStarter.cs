namespace Twice.Utilities.Os
{
	internal interface IProcessStarter
	{
		void Restart();
		void Start( string proc );
	}
}