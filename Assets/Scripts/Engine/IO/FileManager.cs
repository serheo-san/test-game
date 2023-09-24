using System;
using System.IO;
using UnityEngine;

namespace Engine.IO
{
	public static class FileEx
	{
		private const int sCopyBufferSize = 1024 * 512;

		//! OpenRead \author Serheo \date 2018/11/22 14:58:38
		public static Stream OpenRead(string thePath)
		{
			string aPath = PathEx.Combine(Application.persistentDataPath, thePath);
			if (File.Exists(aPath))
			{
				return File.OpenRead(aPath);
			}
			aPath = PathEx.Combine(Application.streamingAssetsPath, thePath);
			if (File.Exists(aPath))
			{
				return File.OpenRead(aPath);
			}
			aPath = PathEx.Combine2Fs(Path.GetDirectoryName(thePath), Path.GetFileNameWithoutExtension(thePath));
			//Debug.Log(string.Format("Resource thePath '{0}'", thePath));
			var textAsset = Resources.Load(aPath, typeof(TextAsset)) as TextAsset;
			if (textAsset != null)
			{
				return new MemoryStream(textAsset.bytes);
			}
#if(UNITY_EDITOR)
			aPath = PathEx.Combine(Application.dataPath, "Resources", thePath);
			if(File.Exists(aPath))
			{
				//Debug.Log(string.Format("Editor thePath '{0}'", thePath));
				return File.OpenRead(aPath);
			}
#endif
			return null;
		}

		//! OpenWrite \author Serheo \date 2018/12/04 18:06:44
		public static Stream OpenWrite(string thePath)
		{
			string aPath = PathEx.GetPath(thePath);
			Stream aStream = null;
			try
			{
				aStream = File.Open(aPath, FileMode.Create, FileAccess.Write, FileShare.Read);
			}
			catch (Exception)
			{
				aStream = null;
			}
			return aStream;
		}

		//! CopyTo \author Serheo \date 2019/01/31 15:52:04
		public static bool Copy(string theSrcPath, string theDstPath)
		{
			var aSrcStream = OpenRead(theSrcPath);
			if (aSrcStream == null)
			{
				return false;
			}
			var aDstStream = OpenWrite(theDstPath);
			if (aDstStream == null)
			{
				aSrcStream.Dispose();
				return false;
			}
			long aSrcLength = aSrcStream.Length;
			int aBufferSize = Math.Min((int)aSrcLength, sCopyBufferSize);
			var aBuffer = new byte[aBufferSize];
			for (long anIndex = 0; anIndex < aSrcLength; anIndex += aBufferSize)
			{
				int aSize = aSrcStream.Read(aBuffer, 0, aBufferSize);
				aDstStream.Write(aBuffer, 0, aSize);
			}
			aSrcStream.Dispose();
			aDstStream.Dispose();
			return true;
		}
	}
}
