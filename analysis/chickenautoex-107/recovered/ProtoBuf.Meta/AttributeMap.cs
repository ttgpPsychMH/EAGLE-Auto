using System;
using System.Reflection;

namespace ProtoBuf.Meta
{
	internal abstract class AttributeMap
	{
		private class ReflectionAttributeMap : AttributeMap
		{
			private readonly Attribute attribute;

			public override object Target => attribute;

			public override Type AttributeType => attribute.GetType();

			public override bool TryGet(string key, bool publicOnly, out object value)
			{
				MemberInfo[] instanceFieldsAndProperties = Helpers.GetInstanceFieldsAndProperties(attribute.GetType(), publicOnly);
				foreach (MemberInfo memberInfo in instanceFieldsAndProperties)
				{
					if (string.Equals(memberInfo.Name, key, StringComparison.OrdinalIgnoreCase))
					{
						if (memberInfo is PropertyInfo propertyInfo)
						{
							value = propertyInfo.GetValue(attribute, null);
							return true;
						}
						if (!(memberInfo is FieldInfo fieldInfo))
						{
							throw new NotSupportedException(memberInfo.GetType().Name);
						}
						value = fieldInfo.GetValue(attribute);
						return true;
					}
				}
				value = null;
				return false;
			}

			public ReflectionAttributeMap(Attribute attribute)
			{
				this.attribute = attribute;
			}
		}

		public abstract Type AttributeType { get; }

		public abstract object Target { get; }

		[Obsolete("Please use AttributeType instead")]
		public new Type GetType()
		{
			return AttributeType;
		}

		public abstract bool TryGet(string key, bool publicOnly, out object value);

		public bool TryGet(string key, out object value)
		{
			return TryGet(key, publicOnly: true, out value);
		}

		public static AttributeMap[] Create(TypeModel model, Type type, bool inherit)
		{
			object[] customAttributes = type.GetCustomAttributes(inherit);
			AttributeMap[] array = new AttributeMap[customAttributes.Length];
			for (int i = 0; i < customAttributes.Length; i++)
			{
				array[i] = new ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return array;
		}

		public static AttributeMap[] Create(TypeModel model, MemberInfo member, bool inherit)
		{
			object[] customAttributes = member.GetCustomAttributes(inherit);
			AttributeMap[] array = new AttributeMap[customAttributes.Length];
			for (int i = 0; i < customAttributes.Length; i++)
			{
				array[i] = new ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return array;
		}

		public static AttributeMap[] Create(TypeModel model, Assembly assembly)
		{
			object[] customAttributes = assembly.GetCustomAttributes(inherit: false);
			AttributeMap[] array = new AttributeMap[customAttributes.Length];
			for (int i = 0; i < customAttributes.Length; i++)
			{
				array[i] = new ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return array;
		}
	}
}
