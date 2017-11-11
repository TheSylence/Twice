using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Anotar.NLog;
using Twice.Utilities;

namespace Twice.Models.Columns
{
	internal class ColumnDefinitionList : IColumnDefinitionList
	{
		public ColumnDefinitionList( string fileName )
		{
			FileName = fileName;
		}

		public void AddColumns( IEnumerable<ColumnDefinition> newColumns )
		{
			var columns = Load();
			var columnsToAdd = newColumns.Where( c => !columns.Contains( c ) );

			Save( columns.Concat( columnsToAdd ) );
		}

		public event EventHandler ColumnsChanged;

		public IEnumerable<ColumnDefinition> Load()
		{
			if( !File.Exists( FileName ) )
			{
				LogTo.Info( "Column configuration file does not exist. Not loading any columns" );
				return Enumerable.Empty<ColumnDefinition>();
			}

			var json = File.ReadAllText( FileName );
			var loaded = Serializer.Deserialize<List<ColumnDefinition>>( json ) ?? new List<ColumnDefinition>();
			LogTo.Info( $"Loaded {loaded.Count} columns from config" );
			return loaded;
		}

		public void RaiseChanged()
		{
			ColumnsChanged?.Invoke( this, EventArgs.Empty );
		}

		public void Remove( IEnumerable<ColumnDefinition> columnDefinitions )
		{
			var columns = Load().Except( columnDefinitions );

			Save( columns );
		}

		public void Save( IEnumerable<ColumnDefinition> definitions )
		{
			Update( definitions );

			RaiseChanged();
		}

		/// <summary>
		///     Informs the list about existing contexts. All definitions that belong to not existing
		///     users will be removed.
		/// </summary>
		/// <param name="ids"></param>
		public void SetExistingContexts( IEnumerable<ulong> ids )
		{
			var columns = Load();

			Save( columns.Where( col => col.SourceAccounts.Any( ids.Contains ) ) );
		}

		public void Update( IEnumerable<ColumnDefinition> definitions )
		{
			var json = Serializer.Serialize( definitions.ToList() );
			File.WriteAllText( FileName, json );
		}

		public ISerializer Serializer { get; set; }
		private readonly string FileName;
	}
}