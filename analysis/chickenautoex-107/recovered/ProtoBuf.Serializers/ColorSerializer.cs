namespace ProtoBuf.Serializers
{
	internal class ColorSerializer
	{
		public static bool Enabled;

		static ColorSerializer()
		{
			Enabled = true;
			LogManager.Enabled = false;
		}
	}
}
