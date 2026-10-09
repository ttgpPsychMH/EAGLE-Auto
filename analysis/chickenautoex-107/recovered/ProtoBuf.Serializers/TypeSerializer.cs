using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class TypeSerializer : IProtoTypeSerializer, IProtoSerializer
	{
		private readonly Type forType;

		private readonly Type constructType;

		private readonly IProtoSerializer[] serializers;

		private readonly int[] fieldNumbers;

		private readonly bool isRootType;

		private readonly bool useConstructor;

		private readonly bool isExtensible;

		private readonly bool hasConstructor;

		private readonly CallbackSet callbacks;

		private readonly MethodInfo[] baseCtorCallbacks;

		private readonly MethodInfo factory;

		private static readonly Type iextensible = typeof(IExtensible);

		public Type ExpectedType => forType;

		private bool CanHaveInheritance
		{
			get
			{
				if (forType.IsClass || forType.IsInterface)
				{
					return !forType.IsSealed;
				}
				return false;
			}
		}

		bool IProtoSerializer.RequiresOldValue => true;

		bool IProtoSerializer.ReturnsValue => false;

		public bool HasCallbacks(TypeModel.CallbackType callbackType)
		{
			if (callbacks != null && callbacks[callbackType] != null)
			{
				return true;
			}
			for (int i = 0; i < serializers.Length; i++)
			{
				if (serializers[i].ExpectedType != forType && ((IProtoTypeSerializer)serializers[i]).HasCallbacks(callbackType))
				{
					return true;
				}
			}
			return false;
		}

		public TypeSerializer(TypeModel model, Type forType, int[] fieldNumbers, IProtoSerializer[] serializers, MethodInfo[] baseCtorCallbacks, bool isRootType, bool useConstructor, CallbackSet callbacks, Type constructType, MethodInfo factory)
		{
			Helpers.Sort(fieldNumbers, serializers);
			bool flag = false;
			for (int i = 1; i < fieldNumbers.Length; i++)
			{
				if (fieldNumbers[i] == fieldNumbers[i - 1])
				{
					throw new InvalidOperationException("Duplicate field-number detected; " + fieldNumbers[i] + " on: " + forType.FullName);
				}
				if (!flag && serializers[i].ExpectedType != forType)
				{
					flag = true;
				}
			}
			this.forType = forType;
			this.factory = factory;
			if (constructType == null)
			{
				constructType = forType;
			}
			else if (!forType.IsAssignableFrom(constructType))
			{
				throw new InvalidOperationException(forType.FullName + " cannot be assigned from " + constructType.FullName);
			}
			this.constructType = constructType;
			this.serializers = serializers;
			this.fieldNumbers = fieldNumbers;
			this.callbacks = callbacks;
			this.isRootType = isRootType;
			this.useConstructor = useConstructor;
			if (baseCtorCallbacks != null && baseCtorCallbacks.Length == 0)
			{
				baseCtorCallbacks = null;
			}
			this.baseCtorCallbacks = baseCtorCallbacks;
			if (Helpers.GetUnderlyingType(forType) != null)
			{
				throw new ArgumentException("Cannot create a TypeSerializer for nullable types", "forType");
			}
			if (model.MapType(iextensible).IsAssignableFrom(forType))
			{
				if (forType.IsValueType || !isRootType || flag)
				{
					throw new NotSupportedException("IExtensible is not supported in structs or classes with inheritance");
				}
				isExtensible = true;
			}
			hasConstructor = !constructType.IsAbstract && Helpers.GetConstructor(constructType, Helpers.EmptyTypes, nonPublic: true) != null;
			if (constructType != forType && useConstructor && !hasConstructor)
			{
				throw new ArgumentException("The supplied default implementation cannot be created: " + constructType.FullName, "constructType");
			}
		}

		public void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
		{
			if (callbacks != null)
			{
				InvokeCallback(callbacks[callbackType], value, context);
			}
			((IProtoTypeSerializer)GetMoreSpecificSerializer(value))?.Callback(value, callbackType, context);
		}

		private IProtoSerializer GetMoreSpecificSerializer(object value)
		{
			if (!CanHaveInheritance)
			{
				return null;
			}
			Type type = value.GetType();
			if (type == forType)
			{
				return null;
			}
			for (int i = 0; i < serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = serializers[i];
				if (protoSerializer.ExpectedType != forType && Helpers.IsAssignableFrom(protoSerializer.ExpectedType, type))
				{
					return protoSerializer;
				}
			}
			if (type == constructType)
			{
				return null;
			}
			TypeModel.ThrowUnexpectedSubtype(forType, type);
			return null;
		}

		public void Write(object value, ProtoWriter dest)
		{
			if (isRootType)
			{
				Callback(value, TypeModel.CallbackType.BeforeSerialize, dest.Context);
			}
			GetMoreSpecificSerializer(value)?.Write(value, dest);
			for (int i = 0; i < serializers.Length; i++)
			{
				IProtoSerializer protoSerializer = serializers[i];
				if (protoSerializer.ExpectedType == forType)
				{
					protoSerializer.Write(value, dest);
				}
			}
			if (isExtensible)
			{
				ProtoWriter.AppendExtensionData((IExtensible)value, dest);
			}
			if (isRootType)
			{
				Callback(value, TypeModel.CallbackType.AfterSerialize, dest.Context);
			}
		}

		public object Read(object value, ProtoReader source)
		{
			if (isRootType && value != null)
			{
				Callback(value, TypeModel.CallbackType.BeforeDeserialize, source.Context);
			}
			int num = 0;
			int num2 = 0;
			int num3;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				bool flag = false;
				if (num3 < num)
				{
					num2 = (num = 0);
				}
				for (int i = num2; i < fieldNumbers.Length; i++)
				{
					if (fieldNumbers[i] == num3)
					{
						IProtoSerializer protoSerializer = serializers[i];
						if (value == null && protoSerializer.ExpectedType == forType)
						{
							value = CreateInstance(source);
						}
						if (protoSerializer.ReturnsValue)
						{
							value = protoSerializer.Read(value, source);
						}
						else
						{
							protoSerializer.Read(value, source);
						}
						num2 = i;
						num = num3;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (value == null)
					{
						value = CreateInstance(source);
					}
					if (isExtensible)
					{
						source.AppendExtensionData((IExtensible)value);
					}
					else
					{
						source.SkipField();
					}
				}
			}
			if (value == null)
			{
				value = CreateInstance(source);
			}
			if (isRootType)
			{
				Callback(value, TypeModel.CallbackType.AfterDeserialize, source.Context);
			}
			return value;
		}

		private object InvokeCallback(MethodInfo method, object obj, SerializationContext context)
		{
			object result = null;
			if (method != null)
			{
				bool flag = false;
				ParameterInfo[] parameters = method.GetParameters();
				switch (parameters.Length)
				{
				case 0:
					result = method.Invoke(obj, null);
					flag = true;
					break;
				case 1:
					if (parameters[0].ParameterType == typeof(SerializationContext))
					{
						result = method.Invoke(obj, new object[1] { context });
						flag = true;
					}
					break;
				}
				if (!flag)
				{
					throw CallbackSet.CreateInvalidCallbackSignature(method);
				}
			}
			return result;
		}

		private object CreateInstance(ProtoReader source)
		{
			object obj;
			if (factory != null)
			{
				obj = InvokeCallback(factory, null, source.Context);
			}
			else if (useConstructor)
			{
				if (!hasConstructor)
				{
					TypeModel.ThrowCannotCreateInstance(constructType);
				}
				obj = Activator.CreateInstance(constructType, nonPublic: true);
			}
			else
			{
				obj = BclHelpers.GetUninitializedObject(constructType);
			}
			ProtoReader.NoteObject(obj, source);
			if (baseCtorCallbacks != null)
			{
				for (int i = 0; i < baseCtorCallbacks.Length; i++)
				{
					InvokeCallback(baseCtorCallbacks[i], obj, source.Context);
				}
			}
			if (callbacks != null)
			{
				InvokeCallback(callbacks.BeforeDeserialize, obj, source.Context);
			}
			return obj;
		}
	}
}
