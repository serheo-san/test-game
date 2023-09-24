
namespace Engine.IO
{
	public class PathEx
	{
		//! GetPath \author serheo 
		public static string GetPath(string thePath)
		{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_ANDROID || UNITY_IOS 
			return Path2Fs(thePath);
#elif UNITY_STANDALONE_WIN
			return Path2Bs(thePath);
#else
			return thePath.Replace('/', '\\');
#endif
		}

		//! Path to back slash \author Serheo 
		public static string Path2Bs(string thePath)
		{
			return thePath.Replace('/', '\\');
		}

		//! Path to forward slash \author Serheo 
		public static string Path2Fs(string thePath)
		{
			return thePath.Replace('\\', '/');
		}

		//! Combine \author serheo 
		public static string Combine(string thePath1, string thePath2)
		{
			return GetPath(System.IO.Path.Combine(thePath1, thePath2));
		}

		//! Combine \author serheo 
		public static string Combine(string thePath1, string thePath2, string thePath3)
		{
			return GetPath(System.IO.Path.Combine(System.IO.Path.Combine(thePath1, thePath2), thePath3));
		}

		//! Для файлов из папки Resources. Там всегда используется прямой слеш 
		public static string Combine2Fs(string thePath1, string thePath2)
		{
			return Path2Fs(System.IO.Path.Combine(thePath1, thePath2));
		}

	}
}
