namespace Twice.Utilities.Os
{
	internal interface IProcessStarter
	{
		void Start( string proc );
		void Restart();
	}
}