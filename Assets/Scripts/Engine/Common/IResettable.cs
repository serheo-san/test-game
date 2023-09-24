using System;

namespace Engine.Common
{
	//! IChild \author Serheo 
	public interface IResettable: IDisposable
	{
		void Expose();
	}
}
