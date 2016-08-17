namespace Twice.Models.Proxy
{
	internal interface IHttpRequest
	{
		string GetQueryParameter( string param );
	}
}