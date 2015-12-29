namespace Twice.Models.Cache
{
	internal interface IDataCache
	{
		IUserCache Users { get; }
	}
}