using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Twice.Models.Columns
{
	internal class ColumnDefinitionList : IColumnDefinitionList
	{
		public ColumnDefinitionList( string fileName )
		{
			FileName = fileName;
		}

		public event EventHandler ColumnsChanged;

		public void AddColumns( IEnumerable<ColumnDefinition> newColumns )
		{
			var columns = Load();
			var columnsToAdd = newColumns.Where( c => !columns.Contains( c ) );

			Save( columns.Concat( columnsToAdd ) );
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

		public void Update( IEnumerable<ColumnDefinition> definitions )
		{
			var json = JsonConvert.SerializeObject( definitions.ToList(), Formatting.Indented );
			File.WriteAllText( FileName, json );
		}

		private readonly string FileName;
	}
}