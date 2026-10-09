// Program.Main — ILSpyCmd 9.1.0.7988, original payload
.method private hidebysig static 
		void Main () cil managed 
	{
		.custom instance void [mscorlib]System.STAThreadAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x70be8
		// Header size: 12
		// Code size: 89 (0x59)
		.maxstack 4
		.entrypoint
		.locals init (
			[0] class [mscorlib]System.Threading.Mutex
		)

		IL_0000: ldc.i4.0
		IL_0001: ldstr "MyUniqueMutexName"
		IL_0006: newobj instance void [mscorlib]System.Threading.Mutex::.ctor(bool, string)
		IL_000b: stloc.0
		.try
		{
			IL_000c: call void TinhKiemAuto.AntiDump::Initialize()
			IL_0011: ldloc.0
			IL_0012: ldc.i4.0
			IL_0013: ldc.i4.0
			IL_0014: callvirt instance bool [mscorlib]System.Threading.WaitHandle::WaitOne(int32, bool)
			IL_0019: brfalse.s IL_0037

			IL_001b: call void [System.Windows.Forms]System.Windows.Forms.Application::EnableVisualStyles()
			IL_0020: ldc.i4.0
			IL_0021: call void [System.Windows.Forms]System.Windows.Forms.Application::SetCompatibleTextRenderingDefault(bool)
			IL_0026: call void TinhKiemAuto.Program::CheckTaiNguyen()
			IL_002b: newobj instance void TinhKiemAuto.FrmMain::.ctor()
			IL_0030: call void [System.Windows.Forms]System.Windows.Forms.Application::Run(class [System.Windows.Forms]System.Windows.Forms.Form)
			IL_0035: leave.s IL_0058

			IL_0037: ldstr "Đã có phiên bản [ChickenAuto] khác đang hoạt động\nNếu bạn chắc chắn rằng Auto không bật\nVui lòng khởi động lại máy tính và thử lại. "
			IL_003c: ldstr "Cảnh Báo"
			IL_0041: ldc.i4.0
			IL_0042: ldc.i4.s 16
			IL_0044: call valuetype [System.Windows.Forms]System.Windows.Forms.DialogResult [System.Windows.Forms]System.Windows.Forms.MessageBox::Show(string, string, valuetype [System.Windows.Forms]System.Windows.Forms.MessageBoxButtons, valuetype [System.Windows.Forms]System.Windows.Forms.MessageBoxIcon)
			IL_0049: pop
			IL_004a: leave.s IL_0058
		} // end .try
		finally
		{
			IL_004c: ldloc.0
			IL_004d: brfalse.s IL_0057

			IL_004f: ldloc.0
			IL_0050: callvirt instance void [mscorlib]System.Threading.WaitHandle::Close()
			IL_0055: ldnull
			IL_0056: stloc.0

			IL_0057: endfinally
		} // end handler

		IL_0058: ret
	} // end of method Program::Main

// Program.CheckTaiNguyen — ILSpyCmd 9.1.0.7988, original payload
.method public hidebysig static 
		void CheckTaiNguyen () cil managed 
	{
		// Method begins at RVA 0x70cc2
		// Header size: 1
		// Code size: 61 (0x3d)
		.maxstack 8

		IL_0000: ldstr "TinhKiemAuto"
		IL_0005: ldstr "EasyHook.dll"
		IL_000a: call string TinhKiemAuto.Global::get_APPPath()
		IL_000f: ldstr "\\Bin\\EasyHook.dll"
		IL_0014: call string [mscorlib]System.String::Concat(string, string)
		IL_0019: call void TinhKiemAuto.TINHKIEM::FileInstall(string, string, string)
		IL_001e: ldstr "TinhKiemAuto"
		IL_0023: ldstr "Newtonsoft.Json.dll"
		IL_0028: call string TinhKiemAuto.Global::get_APPPath()
		IL_002d: ldstr "\\Newtonsoft.Json.dll"
		IL_0032: call string [mscorlib]System.String::Concat(string, string)
		IL_0037: call void TinhKiemAuto.TINHKIEM::FileInstall(string, string, string)
		IL_003c: ret
	} // end of method Program::CheckTaiNguyen

// FrmMain.FrmMain_Load — ILSpyCmd 9.1.0.7988, original payload
.method private hidebysig 
		instance void FrmMain_Load (
			object sender,
			class [mscorlib]System.EventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x5afa8
		// Header size: 12
		// Code size: 965 (0x3c5)
		.maxstack 5
		.locals init (
			[0] bool,
			[1] class [System]System.Net.WebClient,
			[2] string[],
			[3] int32,
			[4] string[],
			[5] valuetype [mscorlib]System.DateTime
		)

		IL_0000: ldc.i4.0
		IL_0001: stloc.0
		IL_0002: newobj instance void [System]System.Net.WebClient::.ctor()
		IL_0007: stloc.1
		.try
		{
			.try
			{
				IL_0008: ldloc.1
				IL_0009: ldsfld string TinhKiemAuto.Global::UpdateURL
				IL_000e: callvirt instance string [System]System.Net.WebClient::DownloadString(string)
				IL_0013: dup
				IL_0014: callvirt instance int32 [mscorlib]System.String::get_Length()
				IL_0019: ldc.i4.0
				IL_001a: bgt.s IL_0027

				IL_001c: ldstr "Download error"
				IL_0021: newobj instance void [mscorlib]System.Exception::.ctor(string)
				IL_0026: throw

				IL_0027: ldc.i4.2
				IL_0028: newarr [mscorlib]System.Char
				IL_002d: dup
				IL_002e: ldc.i4.0
				IL_002f: ldc.i4.s 13
				IL_0031: stelem.i2
				IL_0032: dup
				IL_0033: ldc.i4.1
				IL_0034: ldc.i4.s 10
				IL_0036: stelem.i2
				IL_0037: callvirt instance string[] [mscorlib]System.String::Split(char[])
				IL_003c: dup
				IL_003d: ldlen
				IL_003e: brtrue.s IL_004b

				IL_0040: ldstr "Split download content error"
				IL_0045: newobj instance void [mscorlib]System.Exception::.ctor(string)
				IL_004a: throw

				IL_004b: stloc.2
				IL_004c: ldc.i4.0
				IL_004d: stloc.3
				IL_004e: br.s IL_00cf
				// loop start (head: IL_00cf)
					IL_0050: ldloc.2
					IL_0051: ldloc.3
					IL_0052: ldelem.ref
					IL_0053: ldc.i4.1
					IL_0054: newarr [mscorlib]System.Char
					IL_0059: dup
					IL_005a: ldc.i4.0
					IL_005b: ldc.i4.s 61
					IL_005d: stelem.i2
					IL_005e: callvirt instance string[] [mscorlib]System.String::Split(char[])
					IL_0063: stloc.s 4
					IL_0065: ldloc.s 4
					IL_0067: ldlen
					IL_0068: conv.i4
					IL_0069: ldc.i4.2
					IL_006a: blt.s IL_00cb

					IL_006c: ldloc.s 4
					IL_006e: ldc.i4.0
					IL_006f: ldelem.ref
					IL_0070: callvirt instance string [mscorlib]System.String::Trim()
					IL_0075: ldstr "Version"
					IL_007a: call bool [mscorlib]System.String::op_Equality(string, string)
					IL_007f: brfalse.s IL_00cb

					IL_0081: ldloc.s 4
					IL_0083: ldc.i4.1
					IL_0084: ldelem.ref
					IL_0085: callvirt instance string [mscorlib]System.String::Trim()
					IL_008a: call int32 [mscorlib]System.Int32::Parse(string)
					IL_008f: call string TinhKiemAuto.Global::get_Version()
					IL_0094: call int32 [mscorlib]System.Int32::Parse(string)
					IL_0099: ble.s IL_00a6

					IL_009b: ldstr "Version too low"
					IL_00a0: newobj instance void [mscorlib]System.Exception::.ctor(string)
					IL_00a5: throw

					IL_00a6: ldc.i4.1
					IL_00a7: stloc.0
					IL_00a8: ldarg.0
					IL_00a9: ldfld class [System.Windows.Forms]System.Windows.Forms.RichTextBox TinhKiemAuto.FrmMain::txtlogs
					IL_00ae: ldstr "Checked Server Version: "
					IL_00b3: ldloc.s 4
					IL_00b5: ldc.i4.1
					IL_00b6: ldelem.ref
					IL_00b7: callvirt instance string [mscorlib]System.String::Trim()
					IL_00bc: ldstr "\n"
					IL_00c1: call string [mscorlib]System.String::Concat(string, string, string)
					IL_00c6: callvirt instance void [System.Windows.Forms]System.Windows.Forms.TextBoxBase::AppendText(string)

					IL_00cb: ldloc.3
					IL_00cc: ldc.i4.1
					IL_00cd: add
					IL_00ce: stloc.3

					IL_00cf: ldloc.3
					IL_00d0: ldloc.2
					IL_00d1: ldlen
					IL_00d2: conv.i4
					IL_00d3: blt IL_0050
				// end loop

				IL_00d8: leave.s IL_00fc
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_00da: callvirt instance string [mscorlib]System.Exception::get_Message()
				IL_00df: call valuetype [System.Windows.Forms]System.Windows.Forms.DialogResult [System.Windows.Forms]System.Windows.Forms.MessageBox::Show(string)
				IL_00e4: pop
				IL_00e5: ldstr "http://chickenauto.com/download"
				IL_00ea: call class [System]System.Diagnostics.Process [System]System.Diagnostics.Process::Start(string)
				IL_00ef: pop
				IL_00f0: leave.s IL_00fc
			} // end handler
		} // end .try
		finally
		{
			IL_00f2: ldloc.1
			IL_00f3: brfalse.s IL_00fb

			IL_00f5: ldloc.1
			IL_00f6: callvirt instance void [mscorlib]System.IDisposable::Dispose()

			IL_00fb: endfinally
		} // end handler

		IL_00fc: ldloc.0
		IL_00fd: brtrue.s IL_010b

		IL_00ff: ldarg.0
		IL_0100: call instance void [System]System.ComponentModel.Component::Dispose()
		IL_0105: call void [System.Windows.Forms]System.Windows.Forms.Application::Exit()
		IL_010a: ret

		IL_010b: ldarg.0
		IL_010c: ldfld class [System.Windows.Forms]System.Windows.Forms.RichTextBox TinhKiemAuto.FrmMain::txtlogs
		IL_0111: ldstr "Bật auto :"
		IL_0116: call valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
		IL_011b: stloc.s 5
		IL_011d: ldloca.s 5
		IL_011f: call instance string [mscorlib]System.DateTime::ToString()
		IL_0124: ldstr "\n"
		IL_0129: call string [mscorlib]System.String::Concat(string, string, string)
		IL_012e: callvirt instance void [System.Windows.Forms]System.Windows.Forms.TextBoxBase::AppendText(string)
		IL_0133: ldarg.0
		IL_0134: ldfld class [System.Windows.Forms]System.Windows.Forms.NotifyIcon TinhKiemAuto.FrmMain::notifyIcon1
		IL_0139: ldstr "ChickenAuto"
		IL_013e: callvirt instance void [System.Windows.Forms]System.Windows.Forms.NotifyIcon::set_Text(string)
		IL_0143: ldarg.0
		IL_0144: ldfld class [System.Windows.Forms]System.Windows.Forms.NotifyIcon TinhKiemAuto.FrmMain::notifyIcon1
		IL_0149: newobj instance void [System.Windows.Forms]System.Windows.Forms.ContextMenu::.ctor()
		IL_014e: callvirt instance void [System.Windows.Forms]System.Windows.Forms.NotifyIcon::set_ContextMenu(class [System.Windows.Forms]System.Windows.Forms.ContextMenu)
		IL_0153: ldarg.0
		IL_0154: ldfld class [System.Windows.Forms]System.Windows.Forms.NotifyIcon TinhKiemAuto.FrmMain::notifyIcon1
		IL_0159: callvirt instance class [System.Windows.Forms]System.Windows.Forms.ContextMenu [System.Windows.Forms]System.Windows.Forms.NotifyIcon::get_ContextMenu()
		IL_015e: callvirt instance class [System.Windows.Forms]System.Windows.Forms.Menu/MenuItemCollection [System.Windows.Forms]System.Windows.Forms.Menu::get_MenuItems()
		IL_0163: ldstr "Hiện Auto"
		IL_0168: ldarg.0
		IL_0169: ldftn instance void TinhKiemAuto.FrmMain::HienAuto(object, class [mscorlib]System.EventArgs)
		IL_016f: newobj instance void [mscorlib]System.EventHandler::.ctor(object, native int)
		IL_0174: newobj instance void [System.Windows.Forms]System.Windows.Forms.MenuItem::.ctor(string, class [mscorlib]System.EventHandler)
		IL_0179: callvirt instance int32 [System.Windows.Forms]System.Windows.Forms.Menu/MenuItemCollection::Add(class [System.Windows.Forms]System.Windows.Forms.MenuItem)
		IL_017e: pop
		IL_017f: ldarg.0
		IL_0180: ldfld class [System.Windows.Forms]System.Windows.Forms.NotifyIcon TinhKiemAuto.FrmMain::notifyIcon1
		IL_0185: callvirt instance class [System.Windows.Forms]System.Windows.Forms.ContextMenu [System.Windows.Forms]System.Windows.Forms.NotifyIcon::get_ContextMenu()
		IL_018a: callvirt instance class [System.Windows.Forms]System.Windows.Forms.Menu/MenuItemCollection [System.Windows.Forms]System.Windows.Forms.Menu::get_MenuItems()
		IL_018f: ldstr "Thoát Auto"
		IL_0194: ldarg.0
		IL_0195: ldftn instance void TinhKiemAuto.FrmMain::Thoat(object, class [mscorlib]System.EventArgs)
		IL_019b: newobj instance void [mscorlib]System.EventHandler::.ctor(object, native int)
		IL_01a0: newobj instance void [System.Windows.Forms]System.Windows.Forms.MenuItem::.ctor(string, class [mscorlib]System.EventHandler)
		IL_01a5: callvirt instance int32 [System.Windows.Forms]System.Windows.Forms.Menu/MenuItemCollection::Add(class [System.Windows.Forms]System.Windows.Forms.MenuItem)
		IL_01aa: pop
		IL_01ab: ldarg.0
		IL_01ac: ldstr "ChickenAuto | Version : "
		IL_01b1: call string TinhKiemAuto.Global::get_Version()
		IL_01b6: call string [mscorlib]System.String::Concat(string, string)
		IL_01bb: callvirt instance void [System.Windows.Forms]System.Windows.Forms.Control::set_Text(string)
		IL_01c0: call string TinhKiemAuto.Global::get_DataPath()
		IL_01c5: ldstr "\\20.dat"
		IL_01ca: call string [mscorlib]System.String::Concat(string, string)
		IL_01cf: call bool [mscorlib]System.IO.File::Exists(string)
		IL_01d4: brtrue.s IL_01f4

		IL_01d6: ldstr "TinhKiemAuto"
		IL_01db: ldstr "Scripts.xml"
		IL_01e0: call string TinhKiemAuto.Global::get_DataPath()
		IL_01e5: ldstr "\\20.dat"
		IL_01ea: call string [mscorlib]System.String::Concat(string, string)
		IL_01ef: call void TinhKiemAuto.TINHKIEM::FileInstallMaHoa(string, string, string)

		IL_01f4: call class [mscorlib]System.Collections.Generic.List`1<class TinhKiemAuto.Script> TinhKiemAuto.Scripts::Load()
		IL_01f9: pop
		IL_01fa: call valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
		IL_01ff: stsfld valuetype [mscorlib]System.DateTime TinhKiemAuto.Global::ExaclyTime
		IL_0204: ldarg.0
		IL_0205: ldfld class [System]System.ComponentModel.BackgroundWorker TinhKiemAuto.FrmMain::AccountLogin
		IL_020a: callvirt instance bool [System]System.ComponentModel.BackgroundWorker::get_IsBusy()
		IL_020f: brtrue.s IL_021c

		IL_0211: ldarg.0
		IL_0212: ldfld class [System]System.ComponentModel.BackgroundWorker TinhKiemAuto.FrmMain::AccountLogin
		IL_0217: callvirt instance void [System]System.ComponentModel.BackgroundWorker::RunWorkerAsync()

		IL_021c: ldsfld bool TinhKiemAuto.Global::LanLuot
		IL_0221: brfalse.s IL_0233

		IL_0223: ldarg.0
		IL_0224: ldfld class [System.Windows.Forms]System.Windows.Forms.Timer TinhKiemAuto.FrmMain::tmrLogin
		IL_0229: ldc.i4 10000
		IL_022e: callvirt instance void [System.Windows.Forms]System.Windows.Forms.Timer::set_Interval(int32)

		IL_0233: call string TinhKiemAuto.Global::get_DataPath()
		IL_0238: ldstr "\\MapPath.dat"
		IL_023d: call string [mscorlib]System.String::Concat(string, string)
		IL_0242: call bool [mscorlib]System.IO.File::Exists(string)
		IL_0247: brtrue.s IL_0267

		IL_0249: ldstr "TinhKiemAuto"
		IL_024e: ldstr "MapPath.dat"
		IL_0253: call string TinhKiemAuto.Global::get_DataPath()
		IL_0258: ldstr "\\MapPath.dat"
		IL_025d: call string [mscorlib]System.String::Concat(string, string)
		IL_0262: call void TinhKiemAuto.TINHKIEM::FileInstall(string, string, string)

		IL_0267: ldstr "TinhKiemAuto"
		IL_026c: ldstr "skillList.json"
		IL_0271: call string TinhKiemAuto.Global::get_DataPath()
		IL_0276: ldstr "\\SkillList.dat"
		IL_027b: call string [mscorlib]System.String::Concat(string, string)
		IL_0280: call void TinhKiemAuto.TINHKIEM::FileInstallMaHoa(string, string, string)
		IL_0285: call string TinhKiemAuto.Global::get_DataPath()
		IL_028a: ldstr "\\16.dat"
		IL_028f: call string [mscorlib]System.String::Concat(string, string)
		IL_0294: call bool [mscorlib]System.IO.File::Exists(string)
		IL_0299: brtrue.s IL_02b9

		IL_029b: ldstr "TinhKiemAuto"
		IL_02a0: ldstr "Screen.txt"
		IL_02a5: call string TinhKiemAuto.Global::get_DataPath()
		IL_02aa: ldstr "\\16.dat"
		IL_02af: call string [mscorlib]System.String::Concat(string, string)
		IL_02b4: call void TinhKiemAuto.TINHKIEM::FileInstallMaHoa(string, string, string)

		IL_02b9: call string TinhKiemAuto.Global::get_DataPath()
		IL_02be: ldstr "\\17.dat"
		IL_02c3: call string [mscorlib]System.String::Concat(string, string)
		IL_02c8: call bool [mscorlib]System.IO.File::Exists(string)
		IL_02cd: brtrue.s IL_02ed

		IL_02cf: ldstr "TinhKiemAuto"
		IL_02d4: ldstr "PathList.txt"
		IL_02d9: call string TinhKiemAuto.Global::get_DataPath()
		IL_02de: ldstr "\\17.dat"
		IL_02e3: call string [mscorlib]System.String::Concat(string, string)
		IL_02e8: call void TinhKiemAuto.TINHKIEM::FileInstallMaHoa(string, string, string)

		IL_02ed: call string TinhKiemAuto.Global::get_DataPath()
		IL_02f2: ldstr "\\18.dat"
		IL_02f7: call string [mscorlib]System.String::Concat(string, string)
		IL_02fc: call bool [mscorlib]System.IO.File::Exists(string)
		IL_0301: brtrue.s IL_0321

		IL_0303: ldstr "TinhKiemAuto"
		IL_0308: ldstr "sdbai.json"
		IL_030d: call string TinhKiemAuto.Global::get_DataPath()
		IL_0312: ldstr "\\18.dat"
		IL_0317: call string [mscorlib]System.String::Concat(string, string)
		IL_031c: call void TinhKiemAuto.TINHKIEM::FileInstallMaHoa(string, string, string)

		IL_0321: call void TinhKiemAuto.Models.TrainData::LoadData()
		IL_0326: ldarg.0
		IL_0327: call instance void TinhKiemAuto.FrmMain::LoadTrainMap()
		IL_032c: call string TinhKiemAuto.Global::get_DataPath()
		IL_0331: ldstr "\\19.dat"
		IL_0336: call string [mscorlib]System.String::Concat(string, string)
		IL_033b: call bool [mscorlib]System.IO.File::Exists(string)
		IL_0340: brtrue.s IL_0360

		IL_0342: ldstr "TinhKiemAuto"
		IL_0347: ldstr "allmap.txt"
		IL_034c: call string TinhKiemAuto.Global::get_DataPath()
		IL_0351: ldstr "\\19.dat"
		IL_0356: call string [mscorlib]System.String::Concat(string, string)
		IL_035b: call void TinhKiemAuto.TINHKIEM::FileInstallMaHoa(string, string, string)

		IL_0360: nop
		.try
		{
			IL_0361: call class [mscorlib]System.OperatingSystem [mscorlib]System.Environment::get_OSVersion()
			IL_0366: callvirt instance class [mscorlib]System.Version [mscorlib]System.OperatingSystem::get_Version()
			IL_036b: callvirt instance int32 [mscorlib]System.Version::get_Major()
			IL_0370: ldc.i4.5
			IL_0371: ble.s IL_03a4

			IL_0373: call string [mscorlib]System.Environment::get_SystemDirectory()
			IL_0378: ldstr "\\user32.dll"
			IL_037d: call string [mscorlib]System.String::Concat(string, string)
			IL_0382: ldstr "ChangeWindowMessageFilter"
			IL_0387: call class [mscorlib]System.Delegate FunctionLoader::LoadFunction<class TinhKiemAuto.FrmMain/ChangeWindowMessageFilterDelegate>(string, string)
			IL_038c: castclass TinhKiemAuto.FrmMain/ChangeWindowMessageFilterDelegate
			IL_0391: stsfld class TinhKiemAuto.FrmMain/ChangeWindowMessageFilterDelegate TinhKiemAuto.FrmMain::ChangeWindowMessageFilter
			IL_0396: ldsfld class TinhKiemAuto.FrmMain/ChangeWindowMessageFilterDelegate TinhKiemAuto.FrmMain::ChangeWindowMessageFilter
			IL_039b: ldc.i4.s 74
			IL_039d: ldc.i4.1
			IL_039e: callvirt instance int32 TinhKiemAuto.FrmMain/ChangeWindowMessageFilterDelegate::Invoke(uint32, int32)
			IL_03a3: pop

			IL_03a4: call void TinhKiemAuto.FrmMain::LoadAccountLogin()
			IL_03a9: ldarg.0
			IL_03aa: call instance void TinhKiemAuto.FrmMain::LoadConfig()
			IL_03af: leave.s IL_03be
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_03b1: callvirt instance string [mscorlib]System.Object::ToString()
			IL_03b6: call valuetype [System.Windows.Forms]System.Windows.Forms.DialogResult [System.Windows.Forms]System.Windows.Forms.MessageBox::Show(string)
			IL_03bb: pop
			IL_03bc: leave.s IL_03be
		} // end handler

		IL_03be: ldarg.0
		IL_03bf: call void TinhKiemAuto.Win::Active(class [System.Windows.Forms]System.Windows.Forms.Form)
		IL_03c4: ret
	} // end of method FrmMain::FrmMain_Load
