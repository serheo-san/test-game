namespace Engine.Common
{
	public interface ICopyable<TValue>
	{
		TValue Copy(TValue theOther);
	}
}
