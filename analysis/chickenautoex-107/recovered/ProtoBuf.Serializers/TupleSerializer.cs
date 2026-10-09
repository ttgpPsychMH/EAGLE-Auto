using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class TupleSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		private readonly MemberInfo[] members;

		private readonly ConstructorInfo ctor;

		private IProtoSerializer[] tails;

		public Type ExpectedType => ctor.DeclaringType;

		public bool RequiresOldValue => true;

		public bool ReturnsValue => false;

		public TupleSerializer(RuntimeTypeModel model, ConstructorInfo ctor, MemberInfo[] members)
		{
			if (ctor == null)
			{
				throw new ArgumentNullException("ctor");
			}
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			this.ctor = ctor;
			this.members = members;
			tails = new IProtoSerializer[members.Length];
			ParameterInfo[] parameters = ctor.GetParameters();
			for (int i = 0; i < members.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				Type itemType = null;
				Type defaultType = null;
				MetaType.ResolveListTypes(model, parameterType, ref itemType, ref defaultType);
				Type type = ((itemType == null) ? parameterType : itemType);
				IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(model, DataFormat.Default, type, out var defaultWireType, asReference: false, dynamicType: false, overwriteList: false, allowComplexTypes: true);
				if (protoSerializer == null)
				{
					throw new InvalidOperationException("No serializer defined for type: " + type.FullName);
				}
				protoSerializer = new TagDecorator(i + 1, defaultWireType, strict: false, protoSerializer);
				IProtoSerializer protoSerializer2 = ((itemType != null) ? ((!parameterType.IsArray) ? ((ProtoDecoratorBase)new ListDecorator(model, parameterType, defaultType, protoSerializer, i + 1, writePacked: false, defaultWireType, returnList: true, overwriteList: false, supportNull: false)) : ((ProtoDecoratorBase)new ArrayDecorator(model, protoSerializer, i + 1, writePacked: false, defaultWireType, parameterType, overwriteList: false, supportNull: false))) : protoSerializer);
				tails[i] = protoSerializer2;
			}
		}

		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			return false;
		}

		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
		}

		private object GetValue(object obj, int index)
		{
			if (members[index] is PropertyInfo propertyInfo)
			{
				if (obj != null)
				{
					return propertyInfo.GetValue(obj, null);
				}
				if (!Helpers.IsValueType(propertyInfo.PropertyType))
				{
					return null;
				}
				return Activator.CreateInstance(propertyInfo.PropertyType);
			}
			if (!(members[index] is FieldInfo fieldInfo))
			{
				throw new InvalidOperationException();
			}
			if (obj != null)
			{
				return fieldInfo.GetValue(obj);
			}
			if (!Helpers.IsValueType(fieldInfo.FieldType))
			{
				return null;
			}
			return Activator.CreateInstance(fieldInfo.FieldType);
		}

		public object Read(object value, ProtoReader source)
		{
			object[] array = new object[members.Length];
			bool flag = false;
			if (value == null)
			{
				flag = true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetValue(value, i);
			}
			int num;
			while ((num = source.ReadFieldHeader()) > 0)
			{
				flag = true;
				if (num <= tails.Length)
				{
					IProtoSerializer protoSerializer = tails[num - 1];
					array[num - 1] = tails[num - 1].Read(protoSerializer.RequiresOldValue ? array[num - 1] : null, source);
				}
				else
				{
					source.SkipField();
				}
			}
			if (!flag)
			{
				return value;
			}
			return ctor.Invoke(array);
		}

		public void Write(object value, ProtoWriter dest)
		{
			for (int i = 0; i < tails.Length; i++)
			{
				object value2 = GetValue(value, i);
				if (value2 != null)
				{
					tails[i].Write(value2, dest);
				}
			}
		}

		private Type GetMemberType(int index)
		{
			return Helpers.GetMemberType(members[index]) ?? throw new InvalidOperationException();
		}
	}
}
