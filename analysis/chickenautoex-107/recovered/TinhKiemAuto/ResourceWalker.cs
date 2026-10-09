using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TinhKiemAuto
{
	public class ResourceWalker
	{
		public class ResourceDirectory : ResourceObject
		{
			private IMAGE_RESOURCE_DIRECTORY _base;

			private ResourceDirectory[] _dirs;

			private ResourceFile[] _files;

			private const uint SZ_DIRECTORY = 16u;

			private const uint SZ_ENTRY = 8u;

			public ResourceDirectory[] Directories
			{
				get
				{
					if (_dirs == null)
					{
						Initialize();
					}
					return _dirs;
				}
			}

			public ResourceFile[] Files
			{
				get
				{
					if (_files == null)
					{
						Initialize();
					}
					return _files;
				}
			}

			public ResourceDirectory(PortableExecutable owner, IMAGE_RESOURCE_DIRECTORY_ENTRY entry, bool named, uint root)
				: base(owner, entry, named, root)
			{
				if (!owner.Read((long)(root + (entry.SubdirectoryRva ^ 0x80000000u)), SeekOrigin.Begin, out _base))
				{
					throw owner.GetLastError();
				}
			}

			private void Initialize()
			{
				List<ResourceDirectory> list = new List<ResourceDirectory>();
				List<ResourceFile> list2 = new List<ResourceFile>();
				int numberOfNamedEntries = _base.NumberOfNamedEntries;
				for (int i = 0; i < numberOfNamedEntries + _base.NumberOfIdEntries; i++)
				{
					if (_owner.Read(_root + 16 + (_entry.SubdirectoryRva ^ 0x80000000u) + (long)i * 8L, SeekOrigin.Begin, out IMAGE_RESOURCE_DIRECTORY_ENTRY result))
					{
						if ((result.SubdirectoryRva & 0x80000000u) != 0)
						{
							list.Add(new ResourceDirectory(_owner, result, i < numberOfNamedEntries, _root));
						}
						else
						{
							list2.Add(new ResourceFile(_owner, result, i < numberOfNamedEntries, _root));
						}
					}
				}
				_files = list2.ToArray();
				_dirs = list.ToArray();
			}
		}

		public class ResourceFile : ResourceObject
		{
			private IMAGE_RESOURCE_DATA_ENTRY _base;

			public ResourceFile(PortableExecutable owner, IMAGE_RESOURCE_DIRECTORY_ENTRY entry, bool named, uint root)
				: base(owner, entry, named, root)
			{
				if (!owner.Read((long)(_root + entry.DataEntryRva), SeekOrigin.Begin, out _base))
				{
					throw owner.GetLastError();
				}
			}

			public byte[] GetData()
			{
				byte[] array = new byte[_base.Size];
				if (!_owner.Read(_owner.GetPtrFromRVA(_base.OffsetToData), SeekOrigin.Begin, array))
				{
					throw _owner.GetLastError();
				}
				return array;
			}
		}

		public abstract class ResourceObject
		{
			protected IMAGE_RESOURCE_DIRECTORY_ENTRY _entry;

			private string _name;

			protected PortableExecutable _owner;

			protected uint _root;

			public int Id
			{
				get
				{
					if (!IsNamedResource)
					{
						return (int)_entry.IntegerId;
					}
					return -1;
				}
			}

			public bool IsNamedResource { get; protected set; }

			public string Name => _name;

			public ResourceObject(PortableExecutable owner, IMAGE_RESOURCE_DIRECTORY_ENTRY entry, bool named, uint root)
			{
				_owner = owner;
				_entry = entry;
				IsNamedResource = named;
				if (named)
				{
					ushort result = 0;
					if (owner.Read((long)(root + (entry.NameRva & 0x7FFFFFFF)), SeekOrigin.Begin, out result))
					{
						byte[] array = new byte[result << 1];
						if (owner.Read(0L, SeekOrigin.Current, array))
						{
							_name = Encoding.Unicode.GetString(array);
						}
					}
					if (_name == null)
					{
						throw owner.GetLastError();
					}
				}
				_root = root;
			}
		}

		public ResourceDirectory Root { get; private set; }

		public ResourceWalker(PortableExecutable image)
		{
			IMAGE_DATA_DIRECTORY iMAGE_DATA_DIRECTORY = image.NTHeader.OptionalHeader.DataDirectory[2];
			if (iMAGE_DATA_DIRECTORY.VirtualAddress != 0 && iMAGE_DATA_DIRECTORY.Size != 0)
			{
				uint ptrFromRVA;
				if (!image.Read((long)(ptrFromRVA = image.GetPtrFromRVA(iMAGE_DATA_DIRECTORY.VirtualAddress)), SeekOrigin.Begin, out IMAGE_RESOURCE_DIRECTORY _))
				{
					throw image.GetLastError();
				}
				IMAGE_RESOURCE_DIRECTORY_ENTRY entry = new IMAGE_RESOURCE_DIRECTORY_ENTRY
				{
					SubdirectoryRva = 2147483648u
				};
				Root = new ResourceDirectory(image, entry, named: false, ptrFromRVA);
			}
		}
	}
}
