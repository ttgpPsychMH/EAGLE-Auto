using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	public sealed class RuntimeTypeModel : TypeModel
	{
		private class Singleton
		{
			internal static readonly RuntimeTypeModel Value = new RuntimeTypeModel(isDefault: true);

			private Singleton()
			{
			}
		}

		private sealed class TypeFinder : BasicList.IPredicate
		{
			private readonly Type type;

			public TypeFinder(Type type)
			{
				this.type = type;
			}

			public bool IsMatch(object obj)
			{
				return ((MetaType)obj).Type == type;
			}
		}

		private const byte OPTIONS_InferTagFromNameDefault = 1;

		private const byte OPTIONS_IsDefaultModel = 2;

		private const byte OPTIONS_Frozen = 4;

		private const byte OPTIONS_AutoAddMissingTypes = 8;

		private const byte OPTIONS_UseImplicitZeroDefaults = 32;

		private const byte OPTIONS_AllowParseableTypes = 64;

		private const byte OPTIONS_AutoAddProtoContractTypesOnly = 128;

		private byte options;

		private readonly BasicList types = new BasicList();

		private int metadataTimeoutMilliseconds = 5000;

		private int contentionCounter = 1;

		public bool InferTagFromNameDefault
		{
			get
			{
				return GetOption(1);
			}
			set
			{
				SetOption(1, value);
			}
		}

		public bool AutoAddProtoContractTypesOnly
		{
			get
			{
				return GetOption(128);
			}
			set
			{
				SetOption(128, value);
			}
		}

		public bool UseImplicitZeroDefaults
		{
			get
			{
				return GetOption(32);
			}
			set
			{
				if (!value && GetOption(2))
				{
					throw new InvalidOperationException("UseImplicitZeroDefaults cannot be disabled on the default model");
				}
				SetOption(32, value);
			}
		}

		public bool AllowParseableTypes
		{
			get
			{
				return GetOption(64);
			}
			set
			{
				if (value && GetOption(2))
				{
					throw new InvalidOperationException("AllowParseableTypes cannot be enabled on the default model");
				}
				SetOption(64, value);
			}
		}

		public static RuntimeTypeModel Default => Singleton.Value;

		public MetaType this[Type type] => (MetaType)types[FindOrAddAuto(type, demand: true, addWithContractOnly: false, addEvenIfAutoDisabled: false)];

		public bool AutoAddMissingTypes
		{
			get
			{
				return GetOption(8);
			}
			set
			{
				if (!value && GetOption(2))
				{
					throw new InvalidOperationException("The default model must allow missing types");
				}
				ThrowIfFrozen();
				SetOption(8, value);
			}
		}

		public int MetadataTimeoutMilliseconds
		{
			get
			{
				return metadataTimeoutMilliseconds;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MetadataTimeoutMilliseconds");
				}
				metadataTimeoutMilliseconds = value;
			}
		}

		public event LockContentedEventHandler LockContended;

		private bool GetOption(byte option)
		{
			return (options & option) == option;
		}

		private void SetOption(byte option, bool value)
		{
			if (value)
			{
				options |= option;
			}
			else
			{
				options &= (byte)(~option);
			}
		}

		public IEnumerable GetTypes()
		{
			return types;
		}

		public override string GetSchema(Type type)
		{
			BasicList basicList = new BasicList();
			MetaType metaType = null;
			bool flag = false;
			if (type == null)
			{
				IEnumerator enumerator = types.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MetaType surrogateOrBaseOrSelf = ((MetaType)enumerator.Current).GetSurrogateOrBaseOrSelf();
					if (!basicList.Contains(surrogateOrBaseOrSelf))
					{
						basicList.Add(surrogateOrBaseOrSelf);
						CascadeDependents(basicList, surrogateOrBaseOrSelf);
					}
				}
			}
			else
			{
				Type underlyingType = Helpers.GetUnderlyingType(type);
				if (underlyingType != null)
				{
					type = underlyingType;
				}
				flag = ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out var _, asReference: false, dynamicType: false, overwriteList: false, allowComplexTypes: false) != null;
				if (!flag)
				{
					int num = FindOrAddAuto(type, demand: false, addWithContractOnly: false, addEvenIfAutoDisabled: false);
					if (num < 0)
					{
						throw new ArgumentException("The type specified is not a contract-type", "type");
					}
					metaType = ((MetaType)types[num]).GetSurrogateOrBaseOrSelf();
					basicList.Add(metaType);
					CascadeDependents(basicList, metaType);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			if (!flag)
			{
				foreach (MetaType item in (IEnumerable)((metaType == null) ? types : basicList))
				{
					if (item.IsList)
					{
						continue;
					}
					string text2 = item.Type.Namespace;
					if (!Helpers.IsNullOrEmpty(text2) && !text2.StartsWith("System."))
					{
						if (text == null)
						{
							text = text2;
						}
						else if (!(text == text2))
						{
							text = null;
							break;
						}
					}
				}
			}
			if (!Helpers.IsNullOrEmpty(text))
			{
				stringBuilder.Append("package ").Append(text).Append(';');
				Helpers.AppendLine(stringBuilder);
			}
			bool requiresBclImport = false;
			StringBuilder stringBuilder2 = new StringBuilder();
			MetaType[] array = new MetaType[basicList.Count];
			basicList.CopyTo(array, 0);
			Array.Sort(array, MetaType.Comparer.Default);
			if (flag)
			{
				Helpers.AppendLine(stringBuilder2).Append("message ").Append(type.Name)
					.Append(" {");
				MetaType.NewLine(stringBuilder2, 1).Append("optional ").Append(GetSchemaTypeName(type, DataFormat.Default, asReference: false, dynamicType: false, ref requiresBclImport))
					.Append(" value = 1;");
				Helpers.AppendLine(stringBuilder2).Append('}');
			}
			else
			{
				foreach (MetaType metaType3 in array)
				{
					if (!metaType3.IsList || metaType3 == metaType)
					{
						metaType3.WriteSchema(stringBuilder2, 0, ref requiresBclImport);
					}
				}
			}
			if (requiresBclImport)
			{
				stringBuilder.Append("import \"bcl.proto\" // schema for protobuf-net's handling of core .NET types");
				Helpers.AppendLine(stringBuilder);
			}
			return Helpers.AppendLine(stringBuilder.Append(stringBuilder2)).ToString();
		}

		private void CascadeDependents(BasicList list, MetaType metaType)
		{
			if (metaType.IsList)
			{
				Type listItemType = TypeModel.GetListItemType(this, metaType.Type);
				if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, listItemType, out var _, asReference: false, dynamicType: false, overwriteList: false, allowComplexTypes: false) != null)
				{
					return;
				}
				int num = FindOrAddAuto(listItemType, demand: false, addWithContractOnly: false, addEvenIfAutoDisabled: false);
				if (num >= 0)
				{
					MetaType surrogateOrBaseOrSelf = ((MetaType)types[num]).GetSurrogateOrBaseOrSelf();
					if (!list.Contains(surrogateOrBaseOrSelf))
					{
						list.Add(surrogateOrBaseOrSelf);
						CascadeDependents(list, surrogateOrBaseOrSelf);
					}
				}
				return;
			}
			MetaType surrogateOrBaseOrSelf2;
			if (metaType.IsAutoTuple)
			{
				if (MetaType.ResolveTupleConstructor(metaType.Type, out var mappedMembers) != null)
				{
					for (int i = 0; i < mappedMembers.Length; i++)
					{
						Type type = null;
						if (mappedMembers[i] is PropertyInfo)
						{
							type = ((PropertyInfo)mappedMembers[i]).PropertyType;
						}
						else if (mappedMembers[i] is FieldInfo)
						{
							type = ((FieldInfo)mappedMembers[i]).FieldType;
						}
						if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type, out var _, asReference: false, dynamicType: false, overwriteList: false, allowComplexTypes: false) != null)
						{
							continue;
						}
						int num2 = FindOrAddAuto(type, demand: false, addWithContractOnly: false, addEvenIfAutoDisabled: false);
						if (num2 >= 0)
						{
							surrogateOrBaseOrSelf2 = ((MetaType)types[num2]).GetSurrogateOrBaseOrSelf();
							if (!list.Contains(surrogateOrBaseOrSelf2))
							{
								list.Add(surrogateOrBaseOrSelf2);
								CascadeDependents(list, surrogateOrBaseOrSelf2);
							}
						}
					}
				}
			}
			else
			{
				foreach (ValueMember field in metaType.Fields)
				{
					Type type2 = field.ItemType;
					if (type2 == null)
					{
						type2 = field.MemberType;
					}
					if (ValueMember.TryGetCoreSerializer(this, DataFormat.Default, type2, out var _, asReference: false, dynamicType: false, overwriteList: false, allowComplexTypes: false) != null)
					{
						continue;
					}
					int num3 = FindOrAddAuto(type2, demand: false, addWithContractOnly: false, addEvenIfAutoDisabled: false);
					if (num3 >= 0)
					{
						surrogateOrBaseOrSelf2 = ((MetaType)types[num3]).GetSurrogateOrBaseOrSelf();
						if (!list.Contains(surrogateOrBaseOrSelf2))
						{
							list.Add(surrogateOrBaseOrSelf2);
							CascadeDependents(list, surrogateOrBaseOrSelf2);
						}
					}
				}
			}
			if (metaType.HasSubtypes)
			{
				SubType[] subtypes = metaType.GetSubtypes();
				for (int j = 0; j < subtypes.Length; j++)
				{
					surrogateOrBaseOrSelf2 = subtypes[j].DerivedType.GetSurrogateOrSelf();
					if (!list.Contains(surrogateOrBaseOrSelf2))
					{
						list.Add(surrogateOrBaseOrSelf2);
						CascadeDependents(list, surrogateOrBaseOrSelf2);
					}
				}
			}
			surrogateOrBaseOrSelf2 = metaType.BaseType;
			if (surrogateOrBaseOrSelf2 != null)
			{
				surrogateOrBaseOrSelf2 = surrogateOrBaseOrSelf2.GetSurrogateOrSelf();
			}
			if (surrogateOrBaseOrSelf2 != null && !list.Contains(surrogateOrBaseOrSelf2))
			{
				list.Add(surrogateOrBaseOrSelf2);
				CascadeDependents(list, surrogateOrBaseOrSelf2);
			}
		}

		internal RuntimeTypeModel(bool isDefault)
		{
			AutoAddMissingTypes = true;
			UseImplicitZeroDefaults = true;
			SetOption(2, isDefault);
		}

		internal MetaType FindWithoutAdd(Type type)
		{
			foreach (MetaType type3 in types)
			{
				if (type3.Type == type)
				{
					if (type3.Pending)
					{
						WaitOnLock(type3);
					}
					return type3;
				}
			}
			Type type2 = TypeModel.ResolveProxies(type);
			if (type2 != null)
			{
				return FindWithoutAdd(type2);
			}
			return null;
		}

		private void WaitOnLock(MetaType type)
		{
			int opaqueToken = 0;
			try
			{
				TakeLock(ref opaqueToken);
			}
			finally
			{
				ReleaseLock(opaqueToken);
			}
		}

		internal int FindOrAddAuto(Type type, bool demand, bool addWithContractOnly, bool addEvenIfAutoDisabled)
		{
			TypeFinder predicate = new TypeFinder(type);
			int num = types.IndexOf(predicate);
			MetaType type2;
			if (num >= 0 && (type2 = (MetaType)types[num]).Pending)
			{
				WaitOnLock(type2);
			}
			if (num < 0)
			{
				Type type3 = TypeModel.ResolveProxies(type);
				if (type3 != null)
				{
					predicate = new TypeFinder(type3);
					num = types.IndexOf(predicate);
					type = type3;
				}
			}
			if (num < 0)
			{
				int opaqueToken = 0;
				try
				{
					TakeLock(ref opaqueToken);
					if ((type2 = RecogniseCommonTypes(type)) == null)
					{
						MetaType.AttributeFamily contractFamily = MetaType.GetContractFamily(this, type, null);
						if (contractFamily == MetaType.AttributeFamily.AutoTuple)
						{
							addEvenIfAutoDisabled = true;
						}
						if (!(AutoAddMissingTypes || addEvenIfAutoDisabled) || (!Helpers.IsEnum(type) && addWithContractOnly && contractFamily == MetaType.AttributeFamily.None))
						{
							if (demand)
							{
								TypeModel.ThrowUnexpectedType(type);
							}
							return num;
						}
						type2 = Create(type);
					}
					type2.Pending = true;
					bool flag = false;
					int num2 = types.IndexOf(predicate);
					if (num2 < 0)
					{
						ThrowIfFrozen();
						num = types.Add(type2);
						flag = true;
					}
					else
					{
						num = num2;
					}
					if (flag)
					{
						type2.ApplyDefaultBehaviour();
						type2.Pending = false;
					}
				}
				finally
				{
					ReleaseLock(opaqueToken);
				}
				return num;
			}
			return num;
		}

		private MetaType RecogniseCommonTypes(Type type)
		{
			return null;
		}

		private MetaType Create(Type type)
		{
			ThrowIfFrozen();
			return new MetaType(this, type);
		}

		public MetaType Add(Type type, bool applyDefaultBehaviour)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			MetaType metaType = FindWithoutAdd(type);
			if (metaType != null)
			{
				return metaType;
			}
			int opaqueToken = 0;
			if (type.IsInterface && MapType(MetaType.ienumerable).IsAssignableFrom(type) && TypeModel.GetListItemType(this, type) == null)
			{
				throw new ArgumentException("IEnumerable[<T>] data cannot be used as a meta-type unless an Add method can be resolved");
			}
			try
			{
				metaType = RecogniseCommonTypes(type);
				if (metaType != null)
				{
					if (!applyDefaultBehaviour)
					{
						throw new ArgumentException("Default behaviour must be observed for certain types with special handling; " + type.FullName, "applyDefaultBehaviour");
					}
					applyDefaultBehaviour = false;
				}
				if (metaType == null)
				{
					metaType = Create(type);
				}
				metaType.Pending = true;
				TakeLock(ref opaqueToken);
				if (FindWithoutAdd(type) != null)
				{
					throw new ArgumentException("Duplicate type", "type");
				}
				ThrowIfFrozen();
				types.Add(metaType);
				if (applyDefaultBehaviour)
				{
					metaType.ApplyDefaultBehaviour();
				}
				metaType.Pending = false;
				return metaType;
			}
			finally
			{
				ReleaseLock(opaqueToken);
			}
		}

		private void ThrowIfFrozen()
		{
			if (GetOption(4))
			{
				throw new InvalidOperationException("The model cannot be changed once frozen");
			}
		}

		public void Freeze()
		{
			if (GetOption(2))
			{
				throw new InvalidOperationException("The default model cannot be frozen");
			}
			SetOption(4, value: true);
		}

		protected override int GetKeyImpl(Type type)
		{
			return GetKey(type, demand: false, getBaseKey: true);
		}

		internal int GetKey(Type type, bool demand, bool getBaseKey)
		{
			try
			{
				int num = FindOrAddAuto(type, demand, addWithContractOnly: true, addEvenIfAutoDisabled: false);
				if (num >= 0)
				{
					MetaType source = (MetaType)types[num];
					if (getBaseKey)
					{
						source = MetaType.GetRootType(source);
						num = FindOrAddAuto(source.Type, demand: true, addWithContractOnly: true, addEvenIfAutoDisabled: false);
					}
				}
				return num;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (ex2.Message.IndexOf(type.FullName) >= 0)
				{
					throw;
				}
				throw new ProtoException(ex2.Message + " (" + type.FullName + ")", ex2);
			}
		}

		protected internal override void Serialize(int key, object value, ProtoWriter dest)
		{
			((MetaType)types[key]).Serializer.Write(value, dest);
		}

		protected internal override object Deserialize(int key, object value, ProtoReader source)
		{
			IProtoSerializer serializer = ((MetaType)types[key]).Serializer;
			if (value == null && Helpers.IsValueType(serializer.ExpectedType))
			{
				if (serializer.RequiresOldValue)
				{
					value = Activator.CreateInstance(serializer.ExpectedType);
				}
				return serializer.Read(value, source);
			}
			return serializer.Read(value, source);
		}

		internal bool IsDefined(Type type, int fieldNumber)
		{
			return FindWithoutAdd(type).IsDefined(fieldNumber);
		}

		internal bool IsPrepared(Type type)
		{
			return FindWithoutAdd(type)?.IsPrepared() ?? false;
		}

		internal EnumSerializer.EnumPair[] GetEnumMap(Type type)
		{
			int num = FindOrAddAuto(type, demand: false, addWithContractOnly: false, addEvenIfAutoDisabled: false);
			if (num >= 0)
			{
				return ((MetaType)types[num]).GetEnumMap();
			}
			return null;
		}

		internal void TakeLock(ref int opaqueToken)
		{
			opaqueToken = 0;
			if (Monitor.TryEnter(types, metadataTimeoutMilliseconds))
			{
				opaqueToken = GetContention();
				return;
			}
			AddContention();
			throw new TimeoutException("Timeout while inspecting metadata; this may indicate a deadlock. This can often be avoided by preparing necessary serializers during application initialization, rather than allowing multiple threads to perform the initial metadata inspection; please also see the LockContended event");
		}

		private int GetContention()
		{
			return Interlocked.CompareExchange(ref contentionCounter, 0, 0);
		}

		private void AddContention()
		{
			Interlocked.Increment(ref contentionCounter);
		}

		internal void ReleaseLock(int opaqueToken)
		{
			if (opaqueToken == 0)
			{
				return;
			}
			Monitor.Exit(types);
			if (opaqueToken == GetContention())
			{
				return;
			}
			LockContentedEventHandler lockContentedEventHandler = this.LockContended;
			if (lockContentedEventHandler != null)
			{
				string stackTrace;
				try
				{
					throw new Exception();
				}
				catch (Exception ex)
				{
					stackTrace = ex.StackTrace;
				}
				lockContentedEventHandler(this, new LockContentedEventArgs(stackTrace));
			}
		}

		internal void ResolveListTypes(Type type, ref Type itemType, ref Type defaultType)
		{
			if (type == null || Helpers.GetTypeCode(type) != ProtoTypeCode.Unknown || this[type].IgnoreListHandling)
			{
				return;
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() != 1)
				{
					throw new NotSupportedException("Multi-dimension arrays are supported");
				}
				itemType = type.GetElementType();
				if (itemType == MapType(typeof(byte)))
				{
					defaultType = (itemType = null);
				}
				else
				{
					defaultType = type;
				}
			}
			if (itemType == null)
			{
				itemType = TypeModel.GetListItemType(this, type);
			}
			if (itemType != null)
			{
				Type itemType2 = null;
				Type defaultType2 = null;
				ResolveListTypes(itemType, ref itemType2, ref defaultType2);
				if (itemType2 != null)
				{
					throw TypeModel.CreateNestedListsNotSupported();
				}
			}
			if (itemType == null || defaultType != null)
			{
				return;
			}
			if (type.IsClass && !type.IsAbstract && Helpers.GetConstructor(type, Helpers.EmptyTypes, nonPublic: true) != null)
			{
				defaultType = type;
			}
			if (defaultType == null && type.IsInterface)
			{
				Type[] genericArguments;
				if (type.IsGenericType && type.GetGenericTypeDefinition() == MapType(typeof(IDictionary<, >)) && itemType == MapType(typeof(KeyValuePair<, >)).MakeGenericType(genericArguments = type.GetGenericArguments()))
				{
					defaultType = MapType(typeof(Dictionary<, >)).MakeGenericType(genericArguments);
				}
				else
				{
					defaultType = MapType(typeof(List<>)).MakeGenericType(itemType);
				}
			}
			if (defaultType != null && !Helpers.IsAssignableFrom(type, defaultType))
			{
				defaultType = null;
			}
		}

		internal string GetSchemaTypeName(Type effectiveType, DataFormat dataFormat, bool asReference, bool dynamicType, ref bool requiresBclImport)
		{
			Type underlyingType = Helpers.GetUnderlyingType(effectiveType);
			if (underlyingType != null)
			{
				effectiveType = underlyingType;
			}
			if (effectiveType == MapType(typeof(byte[])))
			{
				return "bytes";
			}
			WireType defaultWireType;
			IProtoSerializer protoSerializer = ValueMember.TryGetCoreSerializer(this, dataFormat, effectiveType, out defaultWireType, asReference: false, dynamicType: false, overwriteList: false, allowComplexTypes: false);
			if (protoSerializer == null)
			{
				if (asReference || dynamicType)
				{
					requiresBclImport = true;
					return "bcl.NetObjectProxy";
				}
				return this[effectiveType].GetSurrogateOrBaseOrSelf().GetSchemaTypeName();
			}
			if (!(protoSerializer is ParseableSerializer))
			{
				switch (Helpers.GetTypeCode(effectiveType))
				{
				case ProtoTypeCode.Boolean:
					return "bool";
				case ProtoTypeCode.Char:
				case ProtoTypeCode.Byte:
				case ProtoTypeCode.UInt16:
				case ProtoTypeCode.UInt32:
					if (dataFormat == DataFormat.FixedSize)
					{
						return "fixed32";
					}
					return "uint32";
				case ProtoTypeCode.SByte:
				case ProtoTypeCode.Int16:
				case ProtoTypeCode.Int32:
					switch (dataFormat)
					{
					case DataFormat.ZigZag:
						return "sint32";
					case DataFormat.FixedSize:
						return "sfixed32";
					default:
						return "int32";
					}
				case ProtoTypeCode.Int64:
					switch (dataFormat)
					{
					case DataFormat.ZigZag:
						return "sint64";
					case DataFormat.FixedSize:
						return "sfixed64";
					default:
						return "int64";
					}
				case ProtoTypeCode.UInt64:
					if (dataFormat == DataFormat.FixedSize)
					{
						return "fixed64";
					}
					return "uint64";
				case ProtoTypeCode.Single:
					return "float";
				case ProtoTypeCode.Double:
					return "double";
				case ProtoTypeCode.Decimal:
					requiresBclImport = true;
					return "bcl.Decimal";
				case ProtoTypeCode.DateTime:
					requiresBclImport = true;
					return "bcl.DateTime";
				case ProtoTypeCode.String:
					if (asReference)
					{
						requiresBclImport = true;
					}
					if (!asReference)
					{
						return "string";
					}
					return "bcl.NetObjectProxy";
				case ProtoTypeCode.TimeSpan:
					requiresBclImport = true;
					return "bcl.TimeSpan";
				case ProtoTypeCode.Guid:
					requiresBclImport = true;
					return "bcl.Guid";
				default:
					throw new NotSupportedException("No .proto map found for: " + effectiveType.FullName);
				}
			}
			if (asReference)
			{
				requiresBclImport = true;
			}
			if (!asReference)
			{
				return "string";
			}
			return "bcl.NetObjectProxy";
		}
	}
}
