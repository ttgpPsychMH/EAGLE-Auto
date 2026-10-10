using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// Analysis only: compile exact selected methods against fake game/process/command dependencies.
// No reference to, load of, or execution of the automation assembly, native hooks or Windows APIs.
internal static class Program
{
    private static Assembly library;
    private static string repo;
    private static readonly List<object> results = new List<object>();
    private static ClassDeclarationSyntax Source(string path, string name) => CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(repo,path)))
        .GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c=>c.Identifier.Text==name);
    private static MethodDeclarationSyntax Method(ClassDeclarationSyntax c,string name,int count=-1) => c.Members.OfType<MethodDeclarationSyntax>()
        .Single(m=>m.Identifier.Text==name && (count<0 || m.ParameterList.Parameters.Count==count));
    private static PropertyDeclarationSyntax Property(ClassDeclarationSyntax c,string name) => c.Members.OfType<PropertyDeclarationSyntax>().Single(p=>p.Identifier.Text==name);
    private static string Field(ClassDeclarationSyntax c,string name) => c.Members.OfType<FieldDeclarationSyntax>().Single(f=>f.Declaration.Variables.Any(v=>v.Identifier.Text==name)).ToFullString();
    private static dynamic Create()
    {
        dynamic g=Activator.CreateInstance(library.GetType("TinhKiemAuto.Game"));
        library.GetType("TinhKiemAuto.FrmMain").GetField("dicGame").GetValue(null).GetType().GetMethod("Clear").Invoke(
            library.GetType("TinhKiemAuto.FrmMain").GetField("dicGame").GetValue(null),null);
        g.AddToForm(100);return g;
    }
    private static void Probe(string name,Func<object> check)
    {
        object observed=check();results.Add(new {name,observed});Console.WriteLine("CONFIRMED: "+name);
    }
    private static void Require(bool condition,string message) { if(!condition)throw new InvalidOperationException(message); }
    private static List<string> Commands(dynamic g)=>(List<string>)g.Commands;
    private static byte[] Packet(string text,bool privateFirst=false,bool utf8=false)
    {
        byte[] data;
        if(utf8)data=Encoding.UTF8.GetBytes(text);
        else
        {
            char[] codes=(char[])library.GetType("TinhKiemAuto.ConverterEx").GetField("Unicodes").GetValue(null);
            data=text.Select(c=>{int index=Array.IndexOf(codes,c);Require(index>=0,"Unrepresentable test character");return (byte)index;}).ToArray();
        }
        byte[] packet=new byte[15+data.Length];BitConverter.GetBytes(100).CopyTo(packet,0);
        packet[4]=218;packet[5]=3;packet[10]=4;data.CopyTo(packet,15);
        if(privateFirst){packet[4]=30;packet[5]=2;packet[10]=3;packet[12]=218;packet[13]=3;packet[18]=4;}
        return packet;
    }
    public static int Main(string[] args)
    {
        repo=args.Single();library=Compile();
        Probe("legacy_recognizes_all_11_schools_with_matching_notice",()=>{
            var schools=new Dictionary<string,int>{{"duongmon",37},{"modung",32},{"tinhtuc",6},{"tieudao",9},{"thieulam",1},{"thienson",8},{"thienlong",7},{"ngamy",5},{"vodang",4},{"minhgiao",2},{"caibang",3}};
            foreach(var pair in schools){dynamic g=Create();g.Receive(Packet("Giang ho tieu tieu thong bao Ac Ba xuat hien tai "+pair.Key));Require((int)g.AcBa==pair.Value,pair.Key);}
            return new {recognized=11,encoding="Actual legacy VISCII decoder and VietLien"};});
        Probe("unicode_viscii_notice_recognized",()=>{dynamic g=Create();g.Receive(Packet("Giang hồ tiểu tiểu: Ác Bá xuất hiện tại Nga My"));Require((int)g.AcBa==5,"VISCII notice");return (int)g.AcBa;});
        Probe("new_npc_name_notice_ignored",()=>{dynamic g=Create();g.Receive(Packet("NPC Nga My Sơn thông báo Ác Bá đã xuất hiện tại phái Nga My"));Require((int)g.AcBa==-1,"Expected old NPC name restriction");return (int)g.AcBa;});
        Probe("nga_mi_alias_ignored",()=>{dynamic g=Create();g.Receive(Packet("Giang hồ tiểu tiểu: Ác Bá xuất hiện tại Nga Mi"));Require((int)g.AcBa==-1,"Expected missing alias");return (int)g.AcBa;});
        Probe("end_notice_misclassified_as_active",()=>{dynamic g=Create();g.Receive(Packet("Giang hồ tiểu tiểu: Ác Bá tại Nga My đã bị tiêu diệt"));Require((int)g.AcBa==5,"Expected no lifecycle predicate");return (int)g.AcBa;});
        Probe("multi_school_notice_last_branch_wins",()=>{dynamic g=Create();g.Receive(Packet("Giang ho tieu tieu o ngamy bao Ac Ba xuat hien tai thieulam"));Require((int)g.AcBa==5,"Independent if order");return new {actual=5,eventClauseSchool=1};});
        Probe("utf8_notice_not_supported_by_viscii_decoder",()=>{dynamic g=Create();g.Receive(Packet("Giang hồ tiểu tiểu: Ác Bá xuất hiện tại Nga My",utf8:true));Require((int)g.AcBa==-1,"Expected codec mismatch");return (int)g.AcBa;});
        Probe("private_signature_before_system_signature_stops_scan",()=>{dynamic g=Create();g.Receive(Packet("Giang ho tieu tieu Ac Ba xuat hien tai ngamy",privateFirst:true));Require((int)g.AcBa==-1,"Scan break");return (int)g.AcBa;});
        Probe("short_payload_throws_before_notice_catch",()=>{dynamic g=Create();string kind=null;try{g.Receive(new byte[3]);}catch(ArgumentException e){kind=e.GetType().Name;}Require(kind!=null,"Expected short payload exception");return kind;});
        Probe("notice_is_per_account_and_never_expires_on_clear",()=>{dynamic g=Create(),m=Activator.CreateInstance(g.GetType());m.AddToForm(101);g.Receive(Packet("Giang ho tieu tieu Ac Ba xuat hien tai ngamy"));g.ClearMission();g.Advance(86400000);Require((int)g.AcBa==5&&(int)m.AcBa==-1,"Per-account stale target");return new {leader=(int)g.AcBa,member=(int)m.AcBa};});
        Probe("ac_ba_menu_flag_does_not_enable_receive_hook",()=>{dynamic g=Create();g.IsAcBa=true;Require(!(bool)g.ShouldHook(),"Expected missing IsAcBa predicate");return (bool)g.ShouldHook();});
        Probe("unknown_event_goes_to_own_school",()=>{dynamic g=Create();g.DatDoiAcBa();Require(Commands((object)g).Contains("Goto:9:95:105"),"Own school travel");return Commands((object)g).ToArray();});
        Probe("no_matching_school_member_means_no_travel",()=>{dynamic g=Create();g.AcBa=5;g.Handoff();g.DatDoiAcBa();Require(Commands((object)g).Count==0,"Expected no matching member");return Commands((object)g).Count;});
        Probe("handoff_can_choose_auto_off_offline_uninitialized_member",()=>{dynamic g=Create(),m=Activator.CreateInstance(g.GetType());g.AcBa=5;m.TLBB.Id="member";m.TLBB.Name="member";m.TLBB.IsLeader=false;m.TLBB.Menpai=5;m.TLBB.Online=false;m.TLBB.OnlineTimeSec=4;m.IsAuto=m.IsInit=false;m.AddToForm(101);g.Handoff();Require(Commands((object)g).Contains("Appoint:member")&&(bool)m.IsAcBa&&(int)m.AcBa==-1,"Unsafe handoff");return new {command=Commands((object)g).ToArray(),memberTarget=(int)m.AcBa,memberAuto=(bool)m.IsAuto};});
        Probe("outside_ac_ba_not_busy_for_calendar",()=>{dynamic g=Create();Require(!(bool)g.Busy,"Expected IsAcBa missing from IsBusy");return (bool)g.Busy;});
        Probe("auto_off_keeps_ac_ba_flag",()=>{dynamic g=Create();g.IsAuto=false;g.AutoOffGuard();Require((bool)g.IsAcBa,"Expected retained IsAcBa");return (bool)g.IsAcBa;});
        Probe("paused_ac_ba_still_sends_travel_command",()=>{dynamic g=Create();g.Pause(true);g.DatDoiAcBa();Require(Commands((object)g).Any(c=>c.StartsWith("Goto:")),"Expected no module pause guard");return Commands((object)g).ToArray();});
        Probe("empty_saved_route_throws",()=>{dynamic g=Create();g.EmptyRoute();string kind=null;try{g.DatDoiAcBa();}catch(NullReferenceException e){kind=e.GetType().Name;}Require(kind!=null,"Expected missing AcBaPoint fallback");return kind;});
        Probe("negative_move_index_throws",()=>{dynamic g=Create();g.MoveIndex=-2;string kind=null;try{g.MoveNext(new int[,]{{10,100},{40,10}});}catch(IndexOutOfRangeException e){kind=e.GetType().Name;}Require(kind!=null,"Expected negative index");return kind;});
        Probe("nearest_waypoint_uses_x_as_y",()=>{dynamic g=Create();g.CharX=10;g.CharY=100;g.MoveIndex=-1;g.MoveNext(new int[,]{{10,100},{40,10}});Require(Commands((object)g).Contains("Move:40:10"),"Expected wrong nearest point");return new {actualIndex=(int)g.MoveIndex,correctNearestIndex=0,commands=Commands((object)g).ToArray()};});
        Probe("generic_next_never_updates_best_distance",()=>{dynamic g=Create();g.CharX=10;g.CharY=100;g.MoveIndex=-1;g.Next(new int[,]{{10,100},{200,200}});Require((int)g.MoveIndex==1,"Expected last eligible point");return new {actualIndex=(int)g.MoveIndex,correctNearestIndex=0};});
        Probe("different_dungeon_still_runs_ac_ba_patrol",()=>{dynamic g=Create();g.SetDifferentDungeon();g.DatDoiAcBa();Require(Commands((object)g).Contains("LegacyMoveNext"),"Expected broad IsMapPhuBan branch");return Commands((object)g).ToArray();});
        Probe("unseen_dead_boss_sets_completion_state",()=>{dynamic g=Create();g.TLBB.MapId=173;g.AddDeadBoss();g.DatDoiAcBa();Require((bool)g.IsBossDie,"Expected HP0 accepted without live ID");return (bool)g.IsBossDie;});
        Probe("boss_dead_return_prevents_40_second_reset",()=>{dynamic g=Create();g.TLBB.MapId=173;g.AddDeadBoss();g.DatDoiAcBa();g.Advance(100000);g.DatDoiAcBa();Require((bool)g.IsBossDie&&Commands((object)g).Contains("FollowAll"),"Unreachable reset while boss dead");return new {bossDead=(bool)g.IsBossDie,commands=Commands((object)g).ToArray()};});
        Probe("monster_with_entry_name_is_talked_to",()=>{dynamic g=Create();g.AddNpc(false);g.TryEntry();Require(Commands((object)g).Contains("Talk:100"),"Missing IsNPC check");return Commands((object)g).ToArray();});
        Probe("wrong_dialog_clicks_all",()=>{dynamic g=Create();g.AddNpc();g.SetDialog(99,-1);g.TryEntry();Require(Commands((object)g).Contains("ClickAll"),"Wrong dialog fallback");return Commands((object)g).ToArray();});
        Probe("auto_off_member_receives_summon_command",()=>{dynamic g=Create(),m=Activator.CreateInstance(g.GetType());m.TLBB.Id="member";m.TLBB.IsLeader=false;m.IsAuto=false;m.TLBB.MapId=10;m.AddToForm(101);g.TrieuTap();Require(Commands((object)m).Any(c=>c.StartsWith("Goto:")),"Expected no member Auto guard");return Commands((object)m).ToArray();});
        var output=new {baseline="910020a",scope="Selected v0.5 managed methods/branches compiled with fake dependencies; no automation executable/native DLL executed",probes=results.Count,unexpectedFailures=0,results};
        File.WriteAllText(Path.Combine(repo,"analysis/ac-ba/evidence/probes.json"),JsonSerializer.Serialize(output,new JsonSerializerOptions{WriteIndented=true})+"\n");
        Console.WriteLine("PASS: "+results.Count+" observations reproduced on selected legacy methods.");return 0;
    }

    private static Assembly Compile()
    {
        string baseline="analysis/chickenautoex-107/recovered/TinhKiemAuto/";
        var game=Source(".build/ac-ba/ReviewedGame.cs","Game");var form=Source("src/ChickenAutoEx/FrmMain.cs","FrmMain");
        var player=Source(baseline+"TLBB.cs","TLBB");var utility=Source(baseline+"TINHKIEM.cs","TINHKIEM");var converter=Source(baseline+"ConverterEx.cs","ConverterEx");
        string methods=string.Join("\n",new[]{"DatDoiAcBa","TalkNPCPhuBan","TrieuTap","IsMapPhuBan","ClearMission"}.Select(n=>Method(game,n).ToFullString()));
        methods+=Method(game,"MoveNext",1).ToFullString()+Method(game,"Next",1).ToFullString()+Property(game,"AcBaPoint").ToFullString()+Property(game,"IsBusy").ToFullString()+Property(game,"Party").ToFullString()+Property(game,"IsMapAcBa").ToFullString();
        var auto=Method(game,"Auto");var handoff=auto.DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString().StartsWith("IsAcBa && AcBa != TLBB.Menpai"));
        var hook=auto.DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString().StartsWith("!IsHooked &&"));
        var off=auto.DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString()=="!IsAuto");
        var copy=(BlockSyntax)((IfStatementSyntax)Method(form,"WndProc").Body.Statements[0]).Statement;
        var reception=copy.Statements.Skip(3).TakeWhile(s=>!(s is IfStatementSyntax i && i.Condition.ToString()=="num != -1"));
        var maps=Source(baseline+"MAP.cs","MAP");string mapFields=string.Join("\n",maps.Members.OfType<FieldDeclarationSyntax>().Where(f=>f.Declaration.Type.ToString()=="int"));
        string schools=string.Join("\n",new[]{"THIEULAM","MINHGIAO","CAIBANG","VODANG","NGAMY","TINHTUC","THIENLONG","THIENSON","TIEUDAO","MODUNG","DUONGMON"}.Select(n=>"public static class "+n+" {"+Field(Source(baseline+n+".cs",n),"Id")+"}"));
        string source="using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Text.RegularExpressions;using Stopwatch=TinhKiemAuto.FakeStopwatch;namespace TinhKiemAuto {"+Fixture+
            "public static class MAP {"+mapFields+"}"+schools+Source(baseline+"MENPAI.cs","MENPAI").ToFullString()+
            "public class TLBB {"+Player+Property(player,"MapAcBa")+Property(player,"MapMonPhai")+"}"+
            "public static class TINHKIEM {"+Field(utility,"vietnameseSigns")+string.Join("\n",new[]{"VietLien","ClearSign","ParseInt","GetDistance"}.Select(n=>Method(utility,n).ToFullString()))+"}"+
            "public static class ConverterEx {"+Field(converter,"Unicodes")+Method(converter,"VISCII2UnicodeEx")+"}"+
            "public class FrmMain {public static bool AlarmAcBa;public static Dictionary<int,Game> dicGame=new Dictionary<int,Game>();public void Receive(byte[] array){"+string.Join("\n",reception)+"}}"+
            "public class Game {"+Fields+methods+"public void Handoff(){"+handoff+"}public bool ShouldHook()=>"+hook.Condition+";public void AutoOffGuard(){"+off+"}}}";
        var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
        var compilation=CSharpCompilation.Create("AcBaAnalysis",new[]{CSharpSyntaxTree.ParseText(source)},refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using var bytes=new MemoryStream();var emitted=compilation.Emit(bytes);
        if(!emitted.Success)throw new InvalidOperationException(string.Join("\n",emitted.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));
        return Assembly.Load(bytes.ToArray());
    }

    private const string Player="""
        public string Id="leader",KeyId="leader",Name="leader";public int MapId=1,PlayerState,Menpai=1,OnlineTimeSec=4;
        public bool IsLeader=true,Online=true,IsFollow,HaveRide,IsQuestOpen,IsRide;
        """;
    private const string Fixture="""
        public class FakeStopwatch {public static long Now;private long start=Now;public static FakeStopwatch StartNew()=>new FakeStopwatch();public TimeSpan Elapsed=>TimeSpan.FromMilliseconds(Now-start);}
        public class GameObject {public int Id=100;public string Name="Giang ho tieu tieu",Title="";public bool IsNPC=true;public float X,Y,HP=1;public string CleanName=>TINHKIEM.VietLien(Name);}
        public class ObjectsFixture {public List<GameObject> All=new List<GameObject>(),NearMonter18m=new List<GameObject>(),NearMonter12m=new List<GameObject>(),NearMonter20m=new List<GameObject>();}
        public class QuestFrame {private Game owner;public QuestFrame(){}public QuestFrame(Game g){owner=g;}public void ClickAll(){owner.Commands.Add("ClickAll");}public int StrOptionExtra1,StrOptionExtra2;public string Name="";public static List<QuestFrame> Enum(Game g)=>g.Dialog;}
        public static class Global {public static bool Paused;}
        public static class Option {public static bool AlarmChat;}
        public static class Setting {public static string Route="95,105-69,79";public static string LoadMAP(string map)=>Route;}
        """;
    private const string Fields="""
        public TLBB TLBB=new TLBB();public ObjectsFixture Objects=new ObjectsFixture();public List<QuestFrame> Dialog=new List<QuestFrame>();public List<string> Commands=new List<string>();public int ProcessId=100,AcBa=-1,MapAcTac,MapTKC,MoveIndex=-1,MapATIndex=-1;public static int TickCount=18;
        public bool IsAuto=true,IsInit=true,IsAcBa=true,IsRide,IsAlarmAcBa,IsHooked,AlarmChat,IsBossDie,IsTrieuTap,IsTheoQ,IsP,TraQ,NhanQ,IsClick,IsContinute,IsNhamBinhSinhDie;
        public bool IsThuyLao,IsKyCuoc,IsTrungAc,IsLauLanTamBao,IsQ123LauLan,IsQ123ToChau,IsYenTuO,IsPhungHoangLangMo,IsPMP,IsTuBaoBon,IsLuyenKim,IsHuyetChien;
        public float CharX,CharY,RoundX,RoundY;public FakeStopwatch ClearTime=FakeStopwatch.StartNew(),BossDieTime=FakeStopwatch.StartNew(),TimeStand=FakeStopwatch.StartNew(),tranTime=FakeStopwatch.StartNew();public QuestFrame QuestFrame;
        public Game(){FakeStopwatch.Now+=3000;Global.Paused=false;Setting.Route="95,105-69,79";QuestFrame=new QuestFrame(this);}public bool Busy=>IsBusy;public bool IsMoveEx=>true;
        public void Advance(long ms){FakeStopwatch.Now+=ms;}public void Pause(bool value){Global.Paused=value;}public void Receive(byte[] bytes){new FrmMain().Receive(bytes);}public void AddToForm(int id){ProcessId=id;FrmMain.dicGame[id]=this;}
        public void SetDifferentDungeon(){TLBB.MapId=MAP.TranLongKyCuoc;}public void EmptyRoute(){Setting.Route="";}public void AddDeadBoss(){Objects.All.Add(new GameObject{Name="acba",HP=0,IsNPC=false});}public void AddNpc(bool npc=true){Objects.All.Add(new GameObject{IsNPC=npc});}
        public void SetDialog(int a,int b){TLBB.IsQuestOpen=true;Dialog.Add(new QuestFrame{StrOptionExtra1=a,StrOptionExtra2=b});}public void TryEntry(){TalkNPCPhuBan();}
        public bool GoTo(float x,float y,int map){Commands.Add($"Goto:{map}:{x}:{y}");return false;}public void Move(float x,float y){Commands.Add($"Move:{x}:{y}");}public void Talk(int id){Commands.Add("Talk:"+id);}public void QuestFrameOptionClicked(QuestFrame f){Commands.Add($"Option:{f.StrOptionExtra1}:{f.StrOptionExtra2}");}public void CloseQuest(){Commands.Add("Close");TLBB.IsQuestOpen=false;}public bool PickItem()=>false;
        public void StopFollow(){Commands.Add("StopFollow");}public void UpRide(){Commands.Add("UpRide");IsRide=true;}public void DownRide(){Commands.Add("DownRide");IsRide=false;}public void Ride(){Commands.Add("Ride");}public void AskTeamFollow(){Commands.Add("FollowAll");}public void MoveNext(){Commands.Add("LegacyMoveNext");}public void FixKetMap(){Commands.Add("FixKet");}public void AppointLeader(string name){Commands.Add("Appoint:"+name);}
        public bool ThuyLaoRouteFinished;public bool CanActThuyLao()=>false;public void RunThuyLaoMember(Game g){}public void StopThuyLao(string message){}public void ResetThuyLaoProgress(){}
        """;
}
