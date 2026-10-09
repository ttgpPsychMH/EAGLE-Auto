using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class ListDecorator : ProtoDecoratorBase
	{
		private const byte OPTIONS_IsList = 1;

		private const byte OPTIONS_SuppressIList = 2;

		private const byte OPTIONS_WritePacked = 4;

		private const byte OPTIONS_ReturnList = 8;

		private const byte OPTIONS_OverwriteList = 16;

		private const byte OPTIONS_SupportNull = 32;

		private readonly byte options;

		private readonly Type declaredType;

		private readonly Type concreteType;

		private readonly MethodInfo add;

		private readonly int fieldNumber;

		private readonly WireType packedWireType;

		private static readonly Type ienumeratorType;

		private static readonly Type ienumerableType;

		private bool IsList => (options & 1) != 0;

		private bool SuppressIList => (options & 2) != 0;

		private bool WritePacked => (options & 4) != 0;

		private bool SupportNull => (options & 0x20) != 0;

		private bool ReturnList => (options & 8) != 0;

		public override Type ExpectedType => declaredType;

		public override bool RequiresOldValue => AppendToCollection;

		public override bool ReturnsValue => ReturnList;

		private bool AppendToCollection => (options & 0x10) == 0;

		static ListDecorator()
		{
			ienumeratorType = typeof(IEnumerator);
			ienumerableType = typeof(IEnumerable);
			ColorSerializer.Enabled = false;
		}

		internal static bool CanPack(WireType wireType)
		{
			if ((uint)wireType > 1u && wireType != WireType.Fixed32 && wireType != WireType.SignedVariant)
			{
				return false;
			}
			return true;
		}

		public ListDecorator(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull)
			: base(tail)
		{
			if (returnList)
			{
				options |= 8;
			}
			if (overwriteList)
			{
				options |= 16;
			}
			if (supportNull)
			{
				options |= 32;
			}
			if ((writePacked || packedWireType != WireType.None) && fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (!CanPack(packedWireType))
			{
				if (writePacked)
				{
					throw new InvalidOperationException("Only simple data-types can use packed encoding");
				}
				packedWireType = WireType.None;
			}
			this.fieldNumber = fieldNumber;
			if (writePacked)
			{
				options |= 4;
			}
			this.packedWireType = packedWireType;
			if (declaredType == null)
			{
				throw new ArgumentNullException("declaredType");
			}
			if (declaredType.IsArray)
			{
				throw new ArgumentException("Cannot treat arrays as lists", "declaredType");
			}
			this.declaredType = declaredType;
			this.concreteType = concreteType;
			add = TypeModel.ResolveListAdd(model, declaredType, tail.ExpectedType, out var isList);
			if (isList)
			{
				options |= 1;
				if (declaredType.FullName.StartsWith("System.Data.Linq.EntitySet`1[["))
				{
					options |= 2;
				}
			}
			if (add == null)
			{
				throw new InvalidOperationException("Unable to resolve a suitable Add method for " + declaredType.FullName);
			}
		}

		private MethodInfo GetEnumeratorInfo(TypeModel model, out MethodInfo moveNext, out MethodInfo current)
		{
			Type type = null;
			Type expectedType = ExpectedType;
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(expectedType, "GetEnumerator", null);
			Type expectedType2 = Tail.ExpectedType;
			Type returnType;
			if (instanceMethod != null)
			{
				returnType = instanceMethod.ReturnType;
				moveNext = Helpers.GetInstanceMethod(returnType, "MoveNext", null);
				PropertyInfo property = Helpers.GetProperty(returnType, "Current");
				current = ((property == null) ? null : Helpers.GetGetMethod(property, nonPublic: false));
				if (moveNext == null && model.MapType(ienumeratorType).IsAssignableFrom(returnType))
				{
					moveNext = Helpers.GetInstanceMethod(model.MapType(ienumeratorType), "MoveNext", null);
				}
				if (moveNext != null && moveNext.ReturnType == model.MapType(typeof(bool)) && current != null && current.ReturnType == expectedType2)
				{
					return instanceMethod;
				}
				moveNext = (current = null);
			}
			Type type2 = model.MapType(typeof(IEnumerable<>), demand: false);
			if (type2 != null)
			{
				type2 = type2.MakeGenericType(expectedType2);
				type = type2;
			}
			if (type != null && type.IsAssignableFrom(expectedType))
			{
				instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
				returnType = instanceMethod.ReturnType;
				moveNext = Helpers.GetInstanceMethod(model.MapType(ienumeratorType), "MoveNext");
				current = Helpers.GetGetMethod(Helpers.GetProperty(returnType, "Current"), nonPublic: false);
				return instanceMethod;
			}
			type = model.MapType(ienumerableType);
			instanceMethod = Helpers.GetInstanceMethod(type, "GetEnumerator");
			returnType = instanceMethod.ReturnType;
			moveNext = Helpers.GetInstanceMethod(returnType, "MoveNext");
			current = Helpers.GetGetMethod(Helpers.GetProperty(returnType, "Current"), nonPublic: false);
			return instanceMethod;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			bool writePacked = WritePacked;
			SubItemToken token;
			if (writePacked)
			{
				ProtoWriter.WriteFieldHeader(fieldNumber, WireType.String, dest);
				token = ProtoWriter.StartSubItem(value, dest);
				ProtoWriter.SetPackedField(fieldNumber, dest);
			}
			else
			{
				token = default(SubItemToken);
			}
			bool flag = !SupportNull;
			foreach (object item in (IEnumerable)value)
			{
				if (flag && item == null)
				{
					throw new NullReferenceException();
				}
				Tail.Write(item, dest);
			}
			if (writePacked)
			{
				ProtoWriter.EndSubItem(token, dest);
			}
		}

		public override object Read(object value, ProtoReader source)
		{
			int field = source.FieldNumber;
			object obj = value;
			if (value == null)
			{
				value = Activator.CreateInstance(concreteType);
			}
			bool flag = IsList && !SuppressIList;
			if (packedWireType != WireType.None && source.WireType == WireType.String)
			{
				SubItemToken token = ProtoReader.StartSubItem(source);
				if (flag)
				{
					IList list = (IList)value;
					while (ProtoReader.HasSubValue(packedWireType, source))
					{
						list.Add(Tail.Read(null, source));
					}
				}
				else
				{
					object[] array = new object[1];
					while (ProtoReader.HasSubValue(packedWireType, source))
					{
						array[0] = Tail.Read(null, source);
						add.Invoke(value, array);
					}
				}
				ProtoReader.EndSubItem(token, source);
			}
			else if (flag)
			{
				IList list2 = (IList)value;
				do
				{
					list2.Add(Tail.Read(null, source));
				}
				while (source.TryReadFieldHeader(field));
			}
			else
			{
				object[] array2 = new object[1];
				do
				{
					array2[0] = Tail.Read(null, source);
					add.Invoke(value, array2);
				}
				while (source.TryReadFieldHeader(field));
			}
			if (obj != value)
			{
				return value;
			}
			return null;
		}
	}
}
