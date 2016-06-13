using Newtonsoft.Json;

namespace Twice.Utilities
{
	internal class Serializer : ISerializer
	{
		public TObject Deserialize<TObject>( string data )
		{
			return JsonConvert.DeserializeObject<TObject>( data );
		}

		public string Serialize( object obj )
		{
			return JsonConvert.SerializeObject( obj );
		}
	}
}