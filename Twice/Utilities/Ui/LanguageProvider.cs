using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using WPFLocalizeExtension.Engine;

namespace Twice.Utilities.Ui
{
	[ExcludeFromCodeCoverage]
	internal class LanguageProvider : ILanguageProvider
	{
		public IEnumerable<CultureInfo> AvailableLanguages => LocalizeDictionary.Instance.MergedAvailableCultures;
		public CultureInfo CurrentCulture => LocalizeDictionary.CurrentCulture;
	}
}