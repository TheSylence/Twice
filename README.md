# Twice
Twitter Windows Client - Follow [@TwiceApp](https://twitter.com/TwiceApp)

# Mentions
* [LINQ to Twitter](https://github.com/JoeMayo/LinqToTwitter)
* [Material Design In XAML Toolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit)
* [MahApps.Metro](https://github.com/MahApps/MahApps.Metro)
* [Ninject](http://www.ninject.org/)
* [WPF Localization Extension](https://github.com/SeriousM/WPFLocalizationExtension)
* [Squirrel](https://github.com/Squirrel/Squirrel.Windows)
* [Fody](https://github.com/Fody/Fody)
* [NBug](https://github.com/soygul/NBug)
* [GongSolutions.WPF.DragDrop](https://github.com/punker76/gong-wpf-dragdrop)
* And all the things I forgot

# Building
- **Visual Studio 2015 required (I guess)**
- Create a new file *[REPO]/Twice/Constants.Auth.cs* with the following content
```csharp
namespace Twice
{
	internal static partial class Constants
	{
		public static class Auth
		{
			internal static readonly string ConsumerKey = "YOUR-CONSUMER-KEY";

			internal static readonly string ConsumerSecret = "YOUR-CONSUMER-SECRET";
		}
	}
}
```
where you replace *YOUR-CONSUMER-KEY* and *YOUR-CONSUMER-SECRET* with your access tokens.
- Open solution
- Build
