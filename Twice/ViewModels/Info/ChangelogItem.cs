using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace Twice.ViewModels.Info
{
	internal class ChangelogItem : ObservableObject
	{
		public List<string> Changes { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public List<string> Fixes { get; set; }
		public List<string> Issues { get; set; }
		public List<string> NewFeatures { get; set; }
		public string Version { get; set; }
	}
}