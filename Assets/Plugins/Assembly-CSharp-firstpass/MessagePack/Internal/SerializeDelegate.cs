namespace MessagePack.Internal
{
	internal delegate int SerializeDelegate<T>(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver);
}
