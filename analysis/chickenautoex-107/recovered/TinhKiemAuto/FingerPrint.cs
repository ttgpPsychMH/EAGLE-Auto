using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace TinhKiemAuto
{
	internal class FingerPrint
	{
		private static string fingerPrint = string.Empty;

		private static string serial = "";

		public static string Serial
		{
			get
			{
				serial = Setting.Read("User", "HWID");
				if (serial.Length == 32)
				{
					return serial;
				}
				if (serial == "")
				{
					serial = GetHash(Value() + TINHKIEM.GetMACAddress());
					Setting.Write("User", "HWID", serial);
				}
				return serial;
			}
		}

		public static string TrueSerial
		{
			get
			{
				Setting.Write("User", "HWID", GetHash(Value() + TINHKIEM.GetMACAddress()));
				return GetHash(Value() + TINHKIEM.GetMACAddress());
			}
		}

		public static string CpuId
		{
			get
			{
				string text = identifier("Win32_Processor", "UniqueId");
				if (text == "")
				{
					text = identifier("Win32_Processor", "ProcessorId");
					if (text == "")
					{
						text = identifier("Win32_Processor", "Name");
						if (text == "")
						{
							text = identifier("Win32_Processor", "Manufacturer");
						}
						text += identifier("Win32_Processor", "MaxClockSpeed");
					}
				}
				return text;
			}
		}

		public static string Value()
		{
			if (string.IsNullOrEmpty(fingerPrint))
			{
				fingerPrint = GetHash("CPU >> " + CpuId + "\nBIOS >> " + biosId() + "\nBASE >> " + baseId());
			}
			return fingerPrint;
		}

		private static string Md5(string input)
		{
			byte[] array = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public static string macId()
		{
			return identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
		}

		private static string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
		{
			string text = "";
			foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
			{
				if (instance[wmiMustBeTrue].ToString() == "True" && text == "")
				{
					try
					{
						text = instance[wmiProperty].ToString();
					}
					catch
					{
						continue;
					}
					break;
				}
			}
			return text;
		}

		private static string identifier(string wmiClass, string wmiProperty)
		{
			string text = "";
			foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
			{
				if (text == "")
				{
					try
					{
						text = instance[wmiProperty].ToString();
					}
					catch
					{
						continue;
					}
					break;
				}
			}
			return text;
		}

		private static string biosId()
		{
			return identifier("Win32_BIOS", "Manufacturer") + identifier("Win32_BIOS", "SMBIOSBIOSVersion") + identifier("Win32_BIOS", "IdentificationCode") + identifier("Win32_BIOS", "SerialNumber") + identifier("Win32_BIOS", "ReleaseDate") + identifier("Win32_BIOS", "Version");
		}

		public static string diskId()
		{
			return identifier("Win32_DiskDrive", "Model") + identifier("Win32_DiskDrive", "Manufacturer") + identifier("Win32_DiskDrive", "Signature") + identifier("Win32_DiskDrive", "TotalHeads");
		}

		private static string baseId()
		{
			return identifier("Win32_BaseBoard", "Model") + identifier("Win32_BaseBoard", "Manufacturer") + identifier("Win32_BaseBoard", "Name") + identifier("Win32_BaseBoard", "SerialNumber");
		}

		private static string videoId()
		{
			return identifier("Win32_VideoController", "DriverVersion") + identifier("Win32_VideoController", "Name");
		}

		private static string GetHash(string s)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = new ASCIIEncoding().GetBytes(s);
			return GetHexString(mD5CryptoServiceProvider.ComputeHash(bytes));
		}

		public static string GetHexString(byte[] bt)
		{
			string text = string.Empty;
			foreach (byte num in bt)
			{
				int num2 = num & 0xF;
				int num3 = (num >> 4) & 0xF;
				text = ((num3 <= 9) ? (text + num3) : (text + (char)(num3 - 10 + 65)));
				text = ((num2 <= 9) ? (text + num2) : (text + (char)(num2 - 10 + 65)));
			}
			return text;
		}
	}
}
