# Twice
Twitter Windows Client - Follow [@TwiceApp](https://twitter.com/TwiceApp)

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
