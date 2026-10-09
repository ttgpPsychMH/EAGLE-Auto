using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	internal static class ExtensibleUtil
	{
		internal static IEnumerable<TValue> GetExtendedValues<TValue>(IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			IEnumerator enumerator = GetExtendedValues(RuntimeTypeModel.Default, typeof(TValue), instance, tag, format, singleton, allowDefinedTag).GetEnumerator();
			while (enumerator.MoveNext())
			{
				yield return (TValue)enumerator.Current;
			}
		}

		internal static IEnumerable GetExtendedValues(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (tag <= 0)
			{
				throw new ArgumentOutOfRangeException("tag");
			}
			IExtension extensionObject = instance.GetExtensionObject(createIfMissing: false);
			if (extensionObject == null)
			{
				yield break;
			}
			Stream stream = extensionObject.BeginQuery();
			object value = null;
			try
			{
				SerializationContext context = new SerializationContext();
				using (ProtoReader protoReader = new ProtoReader(stream, model, context))
				{
					while (model.TryDeserializeAuxiliaryType(protoReader, format, tag, type, ref value, skipOtherFields: true, asListItem: false, autoCreate: false, insideList: false) && value != null)
					{
						if (!singleton)
						{
							yield return value;
							value = null;
						}
					}
				}
				if (singleton && value != null)
				{
					yield return value;
				}
			}
			finally
			{
				extensionObject.EndQuery(stream);
			}
		}

		internal static void AppendExtendValue(TypeModel model, IExtensible instance, int tag, DataFormat format, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			IExtension extensionObject = instance.GetExtensionObject(createIfMissing: true);
			if (extensionObject == null)
			{
				throw new InvalidOperationException("No extension object available; appended data would be lost.");
			}
			bool commit = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, model, null))
				{
					model.TrySerializeAuxiliaryType(protoWriter, null, format, tag, value, isInsideList: false);
					protoWriter.Close();
				}
				commit = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, commit);
			}
		}

		public static void AppendExtendValueTyped<TSource, TValue>(TypeModel model, TSource instance, int tag, DataFormat format, TValue value) where TSource : class, IExtensible
		{
			AppendExtendValue(model, instance, tag, format, value);
		}
	}
}
