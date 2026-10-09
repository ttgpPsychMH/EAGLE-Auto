using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace TinhKiemAuto
{
	internal class Class95
	{
		private static string string_0 = string.Empty;

		private static string string_1 = "";

		public static string String_0
		{
			get
			{
				string text = "";
				try
				{
					if (string_1.Length == 32)
					{
						text = string_1;
					}
					if (smethod_14(string_1, ""))
					{
						string_1 = smethod_9(smethod_0() + smethod_3());
					}
					return string_1;
				}
				catch
				{
					return smethod_9(GetHardDiskSerialNo());
				}
			}
		}

		public static string String_1 => smethod_9(smethod_0() + smethod_3());

		public static string String_2
		{
			get
			{
				string text = smethod_4("Win32_Processor", "UniqueId");
				try
				{
					if (smethod_14(text, ""))
					{
						text = smethod_4("Win32_Processor", "ProcessorId");
						if (smethod_14(text, ""))
						{
							text = smethod_4("Win32_Processor", "Name");
							if (smethod_14(text, ""))
							{
								text = smethod_4("Win32_Processor", "Manufacturer");
							}
							text += smethod_4("Win32_Processor", "MaxClockSpeed");
						}
					}
				}
				catch
				{
				}
				return text;
			}
		}

		public static string smethod_0()
		{
			if (string.IsNullOrEmpty(string_0))
			{
				string_0 = smethod_9("CPU >> " + String_2 + "\nBIOS >> " + smethod_5() + "\nBASE >> " + smethod_7());
			}
			return string_0;
		}

		public static string GetSING()
		{
			return smethod_9(DateTime.Now.ToString());
		}

		public static string GetHardDiskSerialNo()
		{
			string result = "";
			try
			{
				using (ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = new ManagementClass("Win32_DiskDrive").GetInstances().GetEnumerator())
				{
					if (managementObjectEnumerator.MoveNext())
					{
						result = Convert.ToString(((ManagementObject)managementObjectEnumerator.Current)["SerialNumber"]);
					}
				}
			}
			catch
			{
				result = smethod_3();
			}
			return result;
		}

		public static string smethod_3()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			string text = string.Empty;
			NetworkInterface[] array = allNetworkInterfaces;
			foreach (NetworkInterface networkInterface in array)
			{
				if (text == string.Empty)
				{
					text = networkInterface.GetPhysicalAddress().ToString();
				}
			}
			return text;
		}

		private static string smethod_3(string string_2, string string_3, string string_4)
		{
			string text = "";
			foreach (ManagementObject instance in new ManagementClass(string_2).GetInstances())
			{
				if (smethod_14(instance[string_4].ToString(), "True") && smethod_14(text, ""))
				{
					try
					{
						text = instance[string_3].ToString();
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

		private static string smethod_4(string string_2, string string_3)
		{
			string text = "";
			foreach (ManagementObject instance in new ManagementClass(string_2).GetInstances())
			{
				if (smethod_14(text, ""))
				{
					try
					{
						text = instance[string_3].ToString();
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

		private static string smethod_5()
		{
			try
			{
				return smethod_4("Win32_BIOS", "Manufacturer") + smethod_4("Win32_BIOS", "SMBIOSBIOSVersion") + smethod_4("Win32_BIOS", "IdentificationCode") + smethod_4("Win32_BIOS", "SerialNumber") + smethod_4("Win32_BIOS", "ReleaseDate") + smethod_4("Win32_BIOS", "Version");
			}
			catch
			{
				return "";
			}
		}

		private static string smethod_7()
		{
			try
			{
				return smethod_4("Win32_BaseBoard", "Model") + smethod_4("Win32_BaseBoard", "Manufacturer") + smethod_4("Win32_BaseBoard", "Name") + smethod_4("Win32_BaseBoard", "SerialNumber");
			}
			catch
			{
				return "";
			}
		}

		private static string smethod_9(string string_2)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = new ASCIIEncoding().GetBytes(string_2);
			return smethod_10(mD5CryptoServiceProvider.ComputeHash(bytes));
		}

		public static string smethod_10(byte[] byte_0)
		{
			string text = string.Empty;
			foreach (byte num in byte_0)
			{
				int num2 = num & 0xF;
				int num3 = (num >> 4) & 0xF;
				text = ((num3 <= 9) ? (text + num3) : (text + (char)(num3 - 10 + 65)));
				text = ((num2 <= 9) ? (text + num2) : (text + (char)(num2 - 10 + 65)));
			}
			return text;
		}

		private static bool smethod_14(string string_2, string string_3)
		{
			return string_2 == string_3;
		}
	}
}
