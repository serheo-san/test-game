namespace Engine.Common
{
	//! IChild \author Serheo \date 2016/11/21 16:29:10
	public interface IOwnerId<TValue>
	{
		TValue Id
		{
			get;
			set;
		}
	}
}
