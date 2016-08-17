using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Twice.Utilities
{
	public class SmartCollection<T> : ObservableCollection<T>
	{
		public SmartCollection()
		{
		}

		public SmartCollection( IEnumerable<T> collection )
			: base( collection )
		{
		}

		public SmartCollection( List<T> list )
			: base( list )
		{
		}

		public void AddRange( IEnumerable<T> range )
		{
			foreach( var item in range )
			{
				Items.Add( item );
			}

			OnPropertyChanged( new PropertyChangedEventArgs( "Count" ) );
			OnPropertyChanged( new PropertyChangedEventArgs( "Item[]" ) );
			OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
		}

		public void Reset( IEnumerable<T> range )
		{
			Items.Clear();

			AddRange( range );
		}
	}
}