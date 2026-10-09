using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace TinhKiemAuto
{
	internal class ASM
	{
		public int ProcessId;

		public IntPtr Id;

		public string OPCode = "";

		public byte[] ASMCode;

		public ASM(int processId)
		{
			ProcessId = processId;
			Id = OpenProcess(2035711, bInheritHandle: false, processId);
		}

		public int RunASM()
		{
			OPCode += "33C0C20400";
			ASMCode = new byte[OPCode.Length / 2];
			for (int i = 0; i < OPCode.Length / 2; i++)
			{
				ASMCode[i] = Hex2Byte(OPCode.Substring(i * 2, 2));
			}
			int num = VirtualAllocEx(Id, 0, ASMCode.Length, 4096u, 64u);
			WriteProcessMemory(Id, num, ASMCode, ASMCode.Length, 0);
			CloseHandle(CreateRemoteThread(Id, 0, 0, num, 0, 0, 0));
			return num;
		}

		public string Int2Hex(uint value, int n)
		{
			string text = value.ToString("X8");
			text = text.Substring(8 - n);
			string text2 = "";
			for (int i = 0; i < text.Length / 2; i++)
			{
				text2 += text.Substring(text.Length - i * 2 - 2, 2);
			}
			return text2;
		}

		public byte Hex2Byte(string hex)
		{
			byte result = 0;
			byte.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			return result;
		}

		public void Leave()
		{
			OPCode += "C9";
		}

		public void Pushad()
		{
			OPCode += "60";
		}

		public void Popad()
		{
			OPCode += "61";
		}

		public void Nop()
		{
			OPCode += "90";
		}

		public void Ret()
		{
			OPCode += "C3";
		}

		public void RetA(uint i)
		{
			OPCode += Int2Hex(i, 4);
		}

		public void IN_AL_DX()
		{
			OPCode += "EC";
		}

		public void TEST_EAX_EAX()
		{
			OPCode += "85C0";
		}

		public void Add_EAX_EDX()
		{
			OPCode += "03C2";
		}

		public void Add_EBX_EAX()
		{
			OPCode += "03D8";
		}

		public void Add_EAX_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "0305" + Int2Hex(i, 8);
		}

		public void Add_EBX_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "031D" + Int2Hex(i, 8);
		}

		public void Add_EBP_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "032D" + Int2Hex(i, 8);
		}

		public void Add_EAX(uint i)
		{
			OPCode = OPCode + "05" + Int2Hex(i, 8);
		}

		public void Add_EBX(uint i)
		{
			OPCode = OPCode + "83C3" + Int2Hex(i, 8);
		}

		public void Add_ECX(uint i)
		{
			OPCode = OPCode + "83C1" + Int2Hex(i, 8);
		}

		public void Add_EDX(uint i)
		{
			OPCode = OPCode + "83C2" + Int2Hex(i, 8);
		}

		public void Add_ESI(uint i)
		{
			OPCode = OPCode + "83C6" + Int2Hex(i, 8);
		}

		public void Add_ESP(uint i)
		{
			OPCode = OPCode + "83C4" + Int2Hex(i, 8);
		}

		public void Call_EAX()
		{
			OPCode += "FFD0";
		}

		public void Call_EBX()
		{
			OPCode += "FFD3";
		}

		public void Call_ECX()
		{
			OPCode += "FFD1";
		}

		public void Call_EDX()
		{
			OPCode += "FFD2";
		}

		public void Call_ESI()
		{
			OPCode += "FFD2";
		}

		public void Call_ESP()
		{
			OPCode += "FFD4";
		}

		public void Call_EBP()
		{
			OPCode += "FFD5";
		}

		public void Call_EDI()
		{
			OPCode += "FFD7";
		}

		public void Call_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "FF15" + Int2Hex(i, 8);
		}

		public void Call_DWORD_Ptr_EAX()
		{
			OPCode += "FF10";
		}

		public void Call_DWORD_Ptr_EBX()
		{
			OPCode += "FF13";
		}

		public void Call_DWORD_Ptr_EDX_ADD(uint i)
		{
			OPCode = OPCode + "FF52" + Int2Hex(i, 8);
		}

		public void Cmp_EAX(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "83F8" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "3D" + Int2Hex(i, 8);
			}
		}

		public void Cmp_EAX_EDX()
		{
			OPCode += "3BC2";
		}

		public void Cmp_EAX_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "3B05" + Int2Hex(i, 8);
		}

		public void Cmp_DWORD_Ptr_EAX(uint i)
		{
			OPCode = OPCode + "3905" + Int2Hex(i, 8);
		}

		public void Dec_EAX()
		{
			OPCode += "48";
		}

		public void Dec_EBX()
		{
			OPCode += "4B";
		}

		public void Dec_ECX()
		{
			OPCode += "49";
		}

		public void Dec_EDX()
		{
			OPCode += "4A";
		}

		public void Idiv_EAX()
		{
			OPCode += "F7F8";
		}

		public void Idiv_EBX()
		{
			OPCode += "F7FB";
		}

		public void Idiv_ECX()
		{
			OPCode += "F7F9";
		}

		public void Idiv_EDX()
		{
			OPCode += "F7FA";
		}

		public void Imul_EAX_EDX()
		{
			OPCode += "0FAFC2";
		}

		public void Imul_EAX(uint i)
		{
			OPCode = OPCode + "6BC0" + Int2Hex(i, 2);
		}

		public void ImulB_EAX(uint i)
		{
			OPCode = OPCode + "69C0" + Int2Hex(i, 8);
		}

		public void Inc_EAX()
		{
			OPCode += "40";
		}

		public void Inc_EBX()
		{
			OPCode += "43";
		}

		public void Inc_ECX()
		{
			OPCode += "41";
		}

		public void Inc_EDX()
		{
			OPCode += "42";
		}

		public void Inc_EDI()
		{
			OPCode += "47";
		}

		public void Inc_ESI()
		{
			OPCode += "46";
		}

		public void Inc_DWORD_Ptr_EAX()
		{
			OPCode += "FF00";
		}

		public void Inc_DWORD_Ptr_EBX()
		{
			OPCode += "FF03";
		}

		public void Inc_DWORD_Ptr_ECX()
		{
			OPCode += "FF01";
		}

		public void Inc_DWORD_Ptr_EDX()
		{
			OPCode += "FF02";
		}

		public void JMP_EAX()
		{
			OPCode += "FFE0";
		}

		public void Mov_DWORD_Ptr_EAX(uint i)
		{
			OPCode = OPCode + "A3" + Int2Hex(i, 8);
		}

		public void Mov_EAX(uint i)
		{
			OPCode = OPCode + "B8" + Int2Hex(i, 8);
		}

		public void Mov_EBX(uint i)
		{
			OPCode = OPCode + "BB" + Int2Hex(i, 8);
		}

		public void Mov_ECX(uint i)
		{
			OPCode = OPCode + "B9" + Int2Hex(i, 8);
		}

		public void Mov_EDX(uint i)
		{
			OPCode = OPCode + "BA" + Int2Hex(i, 8);
		}

		public void Mov_ESI(uint i)
		{
			OPCode = OPCode + "BE" + Int2Hex(i, 8);
		}

		public void Mov_ESP(uint i)
		{
			OPCode = OPCode + "BC" + Int2Hex(i, 8);
		}

		public void Mov_EBP(uint i)
		{
			OPCode = OPCode + "BD" + Int2Hex(i, 8);
		}

		public void Mov_EDI(uint i)
		{
			OPCode = OPCode + "BF" + Int2Hex(i, 8);
		}

		public void Mov_EBX_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "8B1D" + Int2Hex(i, 8);
		}

		public void Mov_ECX_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "8B0D" + Int2Hex(i, 8);
		}

		public void Mov_EAX_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "A1" + Int2Hex(i, 8);
		}

		public void Mov_EDX_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "8B15" + Int2Hex(i, 8);
		}

		public void Mov_ESI_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "8B35" + Int2Hex(i, 8);
		}

		public void Mov_ESP_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "8B25" + Int2Hex(i, 8);
		}

		public void Mov_EBP_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "8B2D" + Int2Hex(i, 8);
		}

		public void Mov_EAX_DWORD_Ptr_EAX()
		{
			OPCode += "8B00";
		}

		public void Mov_EAX_DWORD_Ptr_EBP()
		{
			OPCode += "8B4500";
		}

		public void Mov_EAX_DWORD_Ptr_EBX()
		{
			OPCode += "8B03";
		}

		public void Mov_EAX_DWORD_Ptr_ECX()
		{
			OPCode += "8B01";
		}

		public void Mov_EAX_DWORD_Ptr_EDX()
		{
			OPCode += "8B02";
		}

		public void Mov_EAX_DWORD_Ptr_EDI()
		{
			OPCode += "8B07";
		}

		public void Mov_EAX_DWORD_Ptr_ESP()
		{
			OPCode += "8B0424";
		}

		public void Mov_EAX_DWORD_Ptr_ESI()
		{
			OPCode += "8B06";
		}

		public void Mov_EAX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B40" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B80" + Int2Hex(i, 8);
			}
		}

		public void Mov_EAX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B4424" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B8424" + Int2Hex(i, 8);
			}
		}

		public void Mov_EAX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B43" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B83" + Int2Hex(i, 8);
			}
		}

		public void Mov_EAX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B41" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B81" + Int2Hex(i, 8);
			}
		}

		public void Mov_EAX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B42" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B82" + Int2Hex(i, 8);
			}
		}

		public void Mov_EAX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B47" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B87" + Int2Hex(i, 8);
			}
		}

		public void Mov_EAX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B45" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B85" + Int2Hex(i, 8);
			}
		}

		public void Mov_EAX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B46" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B86" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B58" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B98" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B5C24" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B9C24" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B5B" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B9B" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B59" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B99" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B5A" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B9A" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B5F" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B9F" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B5D" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B9D" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B5E" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B9E" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B48" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B88" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B4C24" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B8C24" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B4B" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B8B" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B49" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B89" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B4A" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B8A" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B4F" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B8F" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B4D" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B8D" + Int2Hex(i, 8);
			}
		}

		public void Mov_ECX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B4E" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B8E" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B50" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B90" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B5424" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B9424" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B53" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B93" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B51" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B91" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B52" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B92" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B57" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B97" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B55" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B95" + Int2Hex(i, 8);
			}
		}

		public void Mov_EDX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8B56" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8B96" + Int2Hex(i, 8);
			}
		}

		public void Mov_EBX_DWORD_Ptr_EAX()
		{
			OPCode += "8B18";
		}

		public void Mov_EBX_DWORD_Ptr_EBP()
		{
			OPCode += "8B5D00";
		}

		public void Mov_EBX_DWORD_Ptr_EBX()
		{
			OPCode += "8B1B";
		}

		public void Mov_EBX_DWORD_Ptr_ECX()
		{
			OPCode += "8B19";
		}

		public void Mov_EBX_DWORD_Ptr_EDX()
		{
			OPCode += "8B1A";
		}

		public void Mov_EBX_DWORD_Ptr_EDI()
		{
			OPCode += "8B1F";
		}

		public void Mov_EBX_DWORD_Ptr_ESP()
		{
			OPCode += "8B1C24";
		}

		public void Mov_EBX_DWORD_Ptr_ESI()
		{
			OPCode += "8B1E";
		}

		public void Mov_ECX_DWORD_Ptr_EAX()
		{
			OPCode += "8B08";
		}

		public void Mov_ECX_DWORD_Ptr_EBP()
		{
			OPCode += "8B4D00";
		}

		public void Mov_ECX_DWORD_Ptr_EBX()
		{
			OPCode += "8B0B";
		}

		public void Mov_ECX_DWORD_Ptr_ECX()
		{
			OPCode += "8B09";
		}

		public void Mov_ECX_DWORD_Ptr_EDX()
		{
			OPCode += "8B0A";
		}

		public void Mov_ECX_DWORD_Ptr_EDI()
		{
			OPCode += "8B0F";
		}

		public void Mov_ECX_DWORD_Ptr_ESP()
		{
			OPCode += "8B0C24";
		}

		public void Mov_ECX_DWORD_Ptr_ESI()
		{
			OPCode += "8B0E";
		}

		public void Mov_EDX_DWORD_Ptr_EAX()
		{
			OPCode += "8B10";
		}

		public void Mov_EDX_DWORD_Ptr_EBP()
		{
			OPCode += "8B5500";
		}

		public void Mov_EDX_DWORD_Ptr_EBX()
		{
			OPCode += "8B13";
		}

		public void Mov_EDX_DWORD_Ptr_ECX()
		{
			OPCode += "8B11";
		}

		public void Mov_EDX_DWORD_Ptr_EDX()
		{
			OPCode += "8B12";
		}

		public void Mov_EDX_DWORD_Ptr_EDI()
		{
			OPCode += "8B17";
		}

		public void Mov_EDX_DWORD_Ptr_ESI()
		{
			OPCode += "8B16";
		}

		public void Mov_EDX_DWORD_Ptr_ESP()
		{
			OPCode += "8B1424";
		}

		public void Mov_EAX_EBP()
		{
			OPCode += "8BC5";
		}

		public void Mov_EAX_EBX()
		{
			OPCode += "8BC3";
		}

		public void Mov_EAX_ECX()
		{
			OPCode += "8BC1";
		}

		public void Mov_EAX_EDI()
		{
			OPCode += "8BC7";
		}

		public void Mov_EAX_EDX()
		{
			OPCode += "8BC2";
		}

		public void Mov_EAX_ESI()
		{
			OPCode += "8BC6";
		}

		public void Mov_EAX_ESP()
		{
			OPCode += "8BC4";
		}

		public void Mov_EBX_EBP()
		{
			OPCode += "8BDD";
		}

		public void Mov_EBX_EAX()
		{
			OPCode += "8BD8";
		}

		public void Mov_EBX_ECX()
		{
			OPCode += "8BD9";
		}

		public void Mov_EBX_EDI()
		{
			OPCode += "8BDF";
		}

		public void Mov_EBX_EDX()
		{
			OPCode += "8BDA";
		}

		public void Mov_EBX_ESI()
		{
			OPCode += "8BDE";
		}

		public void Mov_EBX_ESP()
		{
			OPCode += "8BDC";
		}

		public void Mov_ECX_EBP()
		{
			OPCode += "8BCD";
		}

		public void Mov_ECX_EAX()
		{
			OPCode += "8BC8";
		}

		public void Mov_ECX_EBX()
		{
			OPCode += "8BCB";
		}

		public void Mov_ECX_EDI()
		{
			OPCode += "8BCF";
		}

		public void Mov_ECX_EDX()
		{
			OPCode += "8BCA";
		}

		public void Mov_ECX_ESI()
		{
			OPCode += "8BCE";
		}

		public void Mov_ECX_ESP()
		{
			OPCode += "8BCC";
		}

		public void Mov_EDX_EBP()
		{
			OPCode += "8BD5";
		}

		public void Mov_EDX_EBX()
		{
			OPCode += "8BD3";
		}

		public void Mov_EDX_ECX()
		{
			OPCode += "8BD1";
		}

		public void Mov_EDX_EDI()
		{
			OPCode += "8BD7";
		}

		public void Mov_EDX_EAX()
		{
			OPCode += "8BD0";
		}

		public void Mov_EDX_ESI()
		{
			OPCode += "8BD6";
		}

		public void Mov_EDX_ESP()
		{
			OPCode += "8BD4";
		}

		public void Mov_ESI_EBP()
		{
			OPCode += "8BF5";
		}

		public void Mov_ESI_EBX()
		{
			OPCode += "8BF3";
		}

		public void Mov_ESI_ECX()
		{
			OPCode += "8BF1";
		}

		public void Mov_ESI_EDI()
		{
			OPCode += "8BF7";
		}

		public void Mov_ESI_EAX()
		{
			OPCode += "8BF0";
		}

		public void Mov_ESI_EDX()
		{
			OPCode += "8BF2";
		}

		public void Mov_ESI_ESP()
		{
			OPCode += "8BF4";
		}

		public void Mov_ESP_EBP()
		{
			OPCode += "8BE5";
		}

		public void Mov_ESP_EBX()
		{
			OPCode += "8BE3";
		}

		public void Mov_ESP_ECX()
		{
			OPCode += "8BE1";
		}

		public void Mov_ESP_EDI()
		{
			OPCode += "8BE7";
		}

		public void Mov_ESP_EAX()
		{
			OPCode += "8BE0";
		}

		public void Mov_ESP_EDX()
		{
			OPCode += "8BE2";
		}

		public void Mov_ESP_ESI()
		{
			OPCode += "8BE6";
		}

		public void Mov_EDI_EBP()
		{
			OPCode += "8BFD";
		}

		public void Mov_EDI_EAX()
		{
			OPCode += "8BF8";
		}

		public void Mov_EDI_EBX()
		{
			OPCode += "8BFB";
		}

		public void Mov_EDI_ECX()
		{
			OPCode += "8BF9";
		}

		public void Mov_EDI_EDX()
		{
			OPCode += "8BFA";
		}

		public void Mov_EDI_ESI()
		{
			OPCode += "8BFE";
		}

		public void Mov_EDI_ESP()
		{
			OPCode += "8BFC";
		}

		public void Mov_EBP_EDI()
		{
			OPCode += "8BDF";
		}

		public void Mov_EBP_EAX()
		{
			OPCode += "8BE8";
		}

		public void Mov_EBP_EBX()
		{
			OPCode += "8BEB";
		}

		public void Mov_EBP_ECX()
		{
			OPCode += "8BE9";
		}

		public void Mov_EBP_EDX()
		{
			OPCode += "8BEA";
		}

		public void Mov_EBP_ESI()
		{
			OPCode += "8BEE";
		}

		public void Mov_EBP_ESP()
		{
			OPCode += "8BEC";
		}

		public void Push(uint i)
		{
			OPCode = OPCode + "68" + Int2Hex(i, 8);
		}

		public void Push_DWORD_Ptr(uint i)
		{
			OPCode = OPCode + "FF35" + Int2Hex(i, 8);
		}

		public void Push_EAX()
		{
			OPCode += "50";
		}

		public void Push_ECX()
		{
			OPCode += "51";
		}

		public void Push_EDX()
		{
			OPCode += "52";
		}

		public void Push_EBX()
		{
			OPCode += "53";
		}

		public void Push_ESP()
		{
			OPCode += "54";
		}

		public void Push_EBP()
		{
			OPCode += "55";
		}

		public void Push_ESI()
		{
			OPCode += "56";
		}

		public void Push_EDI()
		{
			OPCode += "57";
		}

		public void Lea_EAX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D40" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D80" + Int2Hex(i, 8);
			}
		}

		public void Lea_EAX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D43" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D83" + Int2Hex(i, 8);
			}
		}

		public void Lea_EAX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D41" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D81" + Int2Hex(i, 8);
			}
		}

		public void Lea_EAX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D42" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D82" + Int2Hex(i, 8);
			}
		}

		public void Lea_EAX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D46" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D86" + Int2Hex(i, 8);
			}
		}

		public void Lea_EAX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D40" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D80" + Int2Hex(i, 8);
			}
		}

		public void Lea_EAX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D4424" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D8424" + Int2Hex(i, 8);
			}
		}

		public void Lea_EAX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D47" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D87" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D58" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D98" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D5C24" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D9C24" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D5B" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D9B" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D59" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D99" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D5A" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D9A" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D5F" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D9F" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D5D" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D9D" + Int2Hex(i, 8);
			}
		}

		public void Lea_EBX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D5E" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D9E" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D48" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D88" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D4C24" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D8C24" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D4B" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D8B" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D49" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D89" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D4A" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D8A" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D4F" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D8F" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D4D" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D8D" + Int2Hex(i, 8);
			}
		}

		public void Lea_ECX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D4E" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D8E" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_EAX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D50" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D90" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_ESP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D5424" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D9424" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_EBX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D53" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D93" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_ECX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D51" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D91" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_EDX_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D52" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D92" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_EDI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D57" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D97" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_EBP_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D55" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D95" + Int2Hex(i, 8);
			}
		}

		public void Lea_EDX_DWORD_Ptr_ESI_Add(uint i)
		{
			if (i <= 255)
			{
				OPCode = OPCode + "8D56" + Int2Hex(i, 2);
			}
			else
			{
				OPCode = OPCode + "8D96" + Int2Hex(i, 8);
			}
		}

		public void Pop_EAX()
		{
			OPCode += "58";
		}

		public void Pop_EBX()
		{
			OPCode += "5B";
		}

		public void Pop_ECX()
		{
			OPCode += "59";
		}

		public void Pop_EDX()
		{
			OPCode += "5A";
		}

		public void Pop_ESI()
		{
			OPCode += "5E";
		}

		public void Pop_ESP()
		{
			OPCode += "5C";
		}

		public void Pop_EDI()
		{
			OPCode += "5F";
		}

		public void Pop_EBP()
		{
			OPCode += "5D";
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, int lpThreadAttributes, int dwStackSize, int lpStartAddress, int lpParameter, int dwCreationFlags, int lpThreadId);

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int processAccess, bool bInheritHandle, int processId);

		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll")]
		public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int VirtualAllocEx(IntPtr hProcess, int lpAddress, int dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll")]
		public static extern bool VirtualFreeEx(IntPtr hProcess, int lpAddress, int dwSize, int dwFreeType);
	}
}
