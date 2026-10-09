using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class SubItemSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		private readonly int key;

		private readonly Type type;

		private readonly ISerializerProxy proxy;

		private readonly bool recursionCheck;

		Type IProtoSerializer.ExpectedType => type;

		bool IProtoSerializer.RequiresOldValue => true;

		bool IProtoSerializer.ReturnsValue => true;

		bool IProtoTypeSerializer.HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return ((IProtoTypeSerializer)proxy.Serializer).HasCallbacks(callbackType);
		}

		void IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			((IProtoTypeSerializer)proxy.Serializer).Callback(value, callbackType, context);
		}

		public SubItemSerializer(Type type, int key, ISerializerProxy proxy, bool recursionCheck)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}
			this.type = type;
			this.proxy = proxy;
			this.key = key;
			this.recursionCheck = recursionCheck;
		}

		void IProtoSerializer.Write(object value, ProtoWriter dest)
		{
			if (recursionCheck)
			{
				ProtoWriter.WriteObject(value, key, dest);
			}
			else
			{
				ProtoWriter.WriteRecursionSafeObject(value, key, dest);
			}
		}

		object IProtoSerializer.Read(object value, ProtoReader source)
		{
			return ProtoReader.ReadObject(value, key, source);
		}
	}
}
