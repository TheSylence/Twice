using System;
using Twice.ViewModels.Profile;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{

	internal class ProfileDialogData : DialogData
	{
		public ProfileDialogData( string screeName )
			: base( typeof( ProfileDialog ), typeof( IProfileDialogViewModel ) )
		{
			ScreenName = screeName;
		}

		public ProfileDialogData( ulong userId )
			: base( typeof( ProfileDialog ), typeof( IProfileDialogViewModel ) )
		{
			UserId = userId;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as ProfileDialogData;
			if( other == null )
			{
				return false;
			}

			if( UserId != 0 )
			{
				return UserId == other.UserId;
			}

			return ScreenName.Equals( other.ScreenName );
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<IProfileDialogViewModel>( viewModel );

			if( UserId != 0 )
			{
				vm.Setup( UserId );
			}
			else
			{
				vm.Setup( ScreenName );
			}
		}

		public string ScreenName { get; }
		public ulong UserId { get; }
	}
}