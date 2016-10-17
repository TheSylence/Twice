using System.Threading.Tasks;
using Twice.Models.Configuration;
using Twice.Resources;
using Twice.Utilities.Ui;

namespace Twice.ViewModels.Settings
{
	internal class SettingsDialogViewModel : DialogViewModel, ISettingsDialogViewModel
	{
		public SettingsDialogViewModel( IConfig config, IVisualSettings visual, IGeneralSettings general,
			IMuteSettings mute, INotificationSettings notifications )
		{
			Config = config;
			Visual = visual;
			General = general;
			Mute = mute;
			Notifications = notifications;

			Title = Strings.Settings;
		}

		public async Task OnLoad( object data )
		{
			await Task.WhenAll(
				General.OnLoad( data ),
				Visual.OnLoad( data ),
				Mute.OnLoad( data ),
				Notifications.OnLoad( data )
			);
		}

		protected override Task<bool> OnOk()
		{
			using( new WaitOperation() )
			{
				General.SaveTo( Config );
				Visual.SaveTo( Config );
				Mute.SaveTo( Config );
				Notifications.SaveTo( Config );
				Config.Save();
			}

			return base.OnOk();
		}

		public IGeneralSettings General { get; }
		public IMuteSettings Mute { get; }
		public INotificationSettings Notifications { get; }
		public IVisualSettings Visual { get; }
		private readonly IConfig Config;
	}
}