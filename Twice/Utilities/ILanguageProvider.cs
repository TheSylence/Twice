using System.Collections.Generic;
using System.Globalization;

namespace Twice.Utilities
{
	internal interface ILanguageProvider
	{
		CultureInfo CurrentCulture { get; }
		IEnumerable<CultureInfo> AvailableLanguages { get; }
	}
}