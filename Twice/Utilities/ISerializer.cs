namespace Twice.Utilities
{
	internal interface ISerializer
	{
		TObject Deserialize<TObject>( string data );

		string Serialize( object obj );
	}
}