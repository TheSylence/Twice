using System.Collections.Generic;
using System.Globalization;

namespace Twice.Utilities
{
	internal interface ILanguageProvider
	{
		IEnumerable<CultureInfo> AvailableLanguages { get; }
		CultureInfo CurrentCulture { get; }
	}
}