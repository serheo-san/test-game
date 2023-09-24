using System;
using System.IO;
using System.Text;

namespace Engine.Storage
{
	//! DataBlock \author Serheo
	public class DataBlock
	{
		#region Static Members

		public const char Separator = '\t';
		public const char ArraySeparator = ',';
		public const string DateTimeFormat = "yyyy.MM.dd_hh:mm";

		#endregion // Static Members

		#region Virtual Functions

		//! Save \author serheo 
		public virtual void Save(StreamWriter theWriter)
		{
			WriteData(theWriter);
		}

		//! Load \author serheo 
		public virtual void Load(StreamReader theReader)
		{
			ReadData(theReader);
		}

		//! Save \author Serheo 
		public virtual void Save(StreamWriter theWriter, char theSeparator)
		{
			WriteData(theWriter, theSeparator);
		}

		//! Load \author Serheo 
		public virtual void Load(StreamReader theReader, char theSeparator)
		{
			ReadData(theReader, theSeparator);
		}

		//! WriteData \author serheo 
		public virtual void WriteData(BlockWriter theWriter)
		{

		}

		//! ReadData \author serheo 
		public virtual void ReadData(BlockReader theReader)
		{

		}

		//! GetData \author serheo 
		public virtual string GetData()
		{
			var aStream = new MemoryStream();
			var theWriter = new StreamWriter(aStream);
			Save(theWriter);
			theWriter.Close();
			return new UTF8Encoding().GetString(aStream.ToArray());
		}

		//! SetData \author serheo 
		public virtual void SetData(string theBuffer)
		{
			var aStream = new MemoryStream(new UTF8Encoding().GetBytes(theBuffer));
			var aReader = new StreamReader(aStream);
			Load(aReader);
			aReader.Close();
		}

		#endregion // Virtual Functions


		#region Functions

		//! SaveData \author Serheo 
		protected void WriteData(StreamWriter theWriter, char theSeparator = Separator)
		{
			var aBlockWriter = new BlockWriter(theWriter, theSeparator);
			WriteData(aBlockWriter);
			aBlockWriter.WriteBlock();
			aBlockWriter.Dispose();
		}

		//! ReadData \author Serheo 
		protected void ReadData(StreamReader theReader, char theSeparator = Separator)
		{
			var aBlockReader = new BlockReader(theReader, theSeparator);
			if (aBlockReader.ReadBlock())
			{
				ReadData(aBlockReader);
			}
			aBlockReader.Dispose();
		}

		#endregion Functions
	}
}