using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Twice.ViewModels.Columns.Definitions
{
	internal class ColumnDefintionList
	{
		public ColumnDefintionList( string fileName )
		{
			FileName = fileName;
		}

		public IEnumerable<ColumnDefinition> DefaultColumns( ulong accountId )
		{
			yield return new MentionsColumnDefinition( new[] { accountId } );
			yield return new TimelineColumnDefinition( new[] { accountId } );
			yield return new UserColumnDefintion( accountId );
			yield return new MessagesColumnDefinition( accountId );
		}

		public IEnumerable<ColumnDefinition> Load()
		{
			if( !File.Exists( FileName ) )
			{
				return Enumerable.Empty<ColumnDefinition>();
			}

			var json = File.ReadAllText( FileName );
			return JsonConvert.DeserializeObject<List<ColumnDefinition>>( json );
		}

		public void Save( IEnumerable<ColumnDefinition> definitions )
		{
			var json = JsonConvert.SerializeObject( definitions.ToList(), Formatting.Indented );
			File.WriteAllText( FileName, json );
		}

		private readonly string FileName;
	}
}