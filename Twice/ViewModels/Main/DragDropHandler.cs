using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Twice.Messages;
using Twice.Models.Columns;
using Twice.ViewModels.Columns;

namespace Twice.ViewModels.Main
{
	[ExcludeFromCodeCoverage]
	internal class DragDropHandler : IDragDropHandler
	{
		public DragDropHandler( IColumnDefinitionList columnList, IMessenger messenger )
		{
			ColumnList = columnList;

			messenger.Register<DragMessage>( this, OnDragMessage );
		}

		bool IDragSource.CanStartDrag( IDragInfo dragInfo )
		{
			return !ResizeInProgress && DragDrop.DefaultDragHandler.CanStartDrag( dragInfo );
		}

		void IDragSource.DragCancelled()
		{
			DragDrop.DefaultDragHandler.DragCancelled();
		}

		void IDragSource.Dropped( IDropInfo dropInfo )
		{
			DragDrop.DefaultDragHandler.Dropped( dropInfo );
		}

		void IDragSource.StartDrag( IDragInfo dragInfo )
		{
			DragDrop.DefaultDragHandler.StartDrag( dragInfo );
		}

		bool IDragSource.TryCatchOccurredException( Exception exception )
		{
			return DragDrop.DefaultDragHandler.TryCatchOccurredException( exception );
		}

		void IDropTarget.DragOver( IDropInfo dropInfo )
		{
			DragDrop.DefaultDropHandler.DragOver( dropInfo );
		}

		void IDropTarget.Drop( IDropInfo dropInfo )
		{
			DragDrop.DefaultDropHandler.Drop( dropInfo );

			if( !dropInfo.IsSameDragDropContextAsSource )
			{
				return;
			}

			var columns = dropInfo.TargetCollection as IEnumerable<IColumnViewModel>;
			Debug.Assert( columns != null );
			ColumnList.Save( columns.Select( c => c.Definition ) );
		}

		private void OnDragMessage( DragMessage msg )
		{
			ResizeInProgress = msg.Start;
		}

		private readonly IColumnDefinitionList ColumnList;
		private bool ResizeInProgress;
	}
}