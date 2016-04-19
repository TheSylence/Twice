using System;
using System.Collections.Generic;

namespace Twice.ViewModels.Info
{
	internal interface IInfoDialogViewModel : IDialogViewModel
	{
		DateTime BuildDate { get; }
		ICollection<ChangelogItem> Changelogs { get; }
		ICollection<LicenseItem> Licenses { get; }
		string Version { get; }
	}
}