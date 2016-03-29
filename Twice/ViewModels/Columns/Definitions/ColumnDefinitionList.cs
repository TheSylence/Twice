using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Twice.ViewModels.Columns.Definitions
{
	internal class ColumnDefinitionList : IColumnDefinitionList
	{
		public ColumnDefinitionList( string fileName = Constants.IO.ColumnDefintionFileName )
		{
			FileName = fileName;
		}

		public event EventHandler ColumnsChanged;

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

			ColumnsChanged?.Invoke( this, EventArgs.Empty );
		}

		private readonly string FileName;
	}
}