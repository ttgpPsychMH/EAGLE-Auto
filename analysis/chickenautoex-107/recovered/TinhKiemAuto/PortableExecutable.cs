using System;
using System.Collections.Generic;
using System.IO;

namespace TinhKiemAuto
{
	[Serializable]
	public class PortableExecutable : MemoryIterator
	{
		public IMAGE_DOS_HEADER DOSHeader { get; private set; }

		public string FileLocation { get; private set; }

		public IMAGE_NT_HEADER32 NTHeader { get; private set; }

		public PortableExecutable(string path)
			: this(File.ReadAllBytes(path))
		{
			FileLocation = path;
		}

		public PortableExecutable(byte[] data)
			: base(data)
		{
			string text = string.Empty;
			IMAGE_NT_HEADER32 result = default(IMAGE_NT_HEADER32);
			IMAGE_DOS_HEADER result2 = default(IMAGE_DOS_HEADER);
			if (Read<IMAGE_DOS_HEADER>(out result2) && result2.e_magic == 23117)
			{
				if (Read((long)result2.e_lfanew, SeekOrigin.Begin, out result) && (long)result.Signature == 17744)
				{
					if (result.OptionalHeader.Magic == 267)
					{
						if (result.OptionalHeader.DataDirectory[14].Size != 0)
						{
							text = "Image contains a CLR runtime header. Currently only native binaries are supported; no .NET dependent libraries.";
						}
					}
					else
					{
						text = "File is of the PE32+ format. Currently support only extends to PE32 images. Either recompile the binary as x86, or choose a different target.";
					}
				}
				else
				{
					text = "Invalid NT header found in image.";
				}
			}
			else
			{
				text = "Invalid DOS Header found in image";
			}
			if (string.IsNullOrEmpty(text))
			{
				NTHeader = result;
				DOSHeader = result2;
				return;
			}
			Dispose();
			throw new ArgumentException(text);
		}

		public IEnumerable<IMAGE_IMPORT_DESCRIPTOR> EnumImports()
		{
			IMAGE_DATA_DIRECTORY iMAGE_DATA_DIRECTORY = NTHeader.OptionalHeader.DataDirectory[1];
			if (iMAGE_DATA_DIRECTORY.Size == 0)
			{
				yield break;
			}
			uint num = GetPtrFromRVA(iMAGE_DATA_DIRECTORY.VirtualAddress);
			IMAGE_IMPORT_DESCRIPTOR result;
			for (uint num2 = typeof(IMAGE_IMPORT_DESCRIPTOR).SizeOf(); Read((long)num, SeekOrigin.Begin, out result); num += num2)
			{
				if (result.OriginalFirstThunk == 0)
				{
					break;
				}
				if (result.Name == 0)
				{
					break;
				}
				yield return result;
			}
		}

		public IEnumerable<IMAGE_SECTION_HEADER> EnumSectionHeaders()
		{
			uint numberOfSections = NTHeader.FileHeader.NumberOfSections;
			long num = NTHeader.FileHeader.SizeOfOptionalHeader + typeof(IMAGE_FILE_HEADER).SizeOf() + 4 + DOSHeader.e_lfanew;
			uint num2 = typeof(IMAGE_SECTION_HEADER).SizeOf();
			for (uint num3 = 0u; num3 < numberOfSections; num3++)
			{
				if (Read(num + num3 * num2, SeekOrigin.Begin, out IMAGE_SECTION_HEADER result))
				{
					yield return result;
				}
			}
		}

		private IMAGE_SECTION_HEADER GetEnclosingSectionHeader(uint rva)
		{
			foreach (IMAGE_SECTION_HEADER item in EnumSectionHeaders())
			{
				if (rva >= item.VirtualAddress && rva < item.VirtualAddress + ((item.VirtualSize != 0) ? item.VirtualSize : item.SizeOfRawData))
				{
					return item;
				}
			}
			throw new EntryPointNotFoundException("RVA does not exist within any of the current sections.");
		}

		public uint GetPtrFromRVA(uint rva)
		{
			IMAGE_SECTION_HEADER enclosingSectionHeader = GetEnclosingSectionHeader(rva);
			return rva - (enclosingSectionHeader.VirtualAddress - enclosingSectionHeader.PointerToRawData);
		}

		public byte[] ToArray()
		{
			return GetUnderlyingData();
		}
	}
}
