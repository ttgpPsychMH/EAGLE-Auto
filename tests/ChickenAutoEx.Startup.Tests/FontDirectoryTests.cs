using System;
using System.IO;
using TinhKiemAuto.Appearance;
using Xunit;
namespace ChickenAutoEx.Startup.Tests
{
    public sealed class FontDirectoryTests : IDisposable
    {
        private readonly string root = Path.Combine(Path.GetTempPath(), "owned-font-choice-" + Guid.NewGuid().ToString("N"));
        private string[] names = { "Aptos.ttf", "Aptos-Bold.ttf", "Aptos-Italic.ttf", "Aptos-Bold-Italic.ttf" };
        public FontDirectoryTests() { Directory.CreateDirectory(root); }
        public void Dispose() { Directory.Delete(root, true); }
        private string MakeFaces()
        {
            string folder = Path.Combine(root, "private-font-input"); Directory.CreateDirectory(folder);
            // Synthetic SFNT signatures only. Never feed these bytes to a native font parser.
            foreach (var name in names) File.WriteAllBytes(Path.Combine(folder, name), new byte[] { 0,1,0,0,0,0,0,0,0,0,0,0 });
            return folder;
        }
        [Fact] public void UserSelectedPathRoundTripsWithoutCopyingOrInstallingFonts()
        {
            string folder=MakeFaces(); Assert.True(FontDirectorySettings.Save(root,folder));
            Assert.Equal(folder,FontDirectorySettings.Read(root));
            Assert.Single(Directory.GetFiles(Path.Combine(root,"EAGLE Auto"),"*",SearchOption.AllDirectories));
        }
        [Fact] public void MissingFaceDoesNotReplaceSavedChoice()
        {
            string folder=MakeFaces(); Assert.True(FontDirectorySettings.Save(root,folder));
            string previous=File.ReadAllText(Path.Combine(root,"EAGLE Auto","Appearance","font-directory.txt"));
            File.Delete(Path.Combine(folder,names[1])); Assert.False(FontDirectorySettings.Save(root,folder));
            Assert.Equal(previous,File.ReadAllText(Path.Combine(root,"EAGLE Auto","Appearance","font-directory.txt")));
            Assert.Null(FontDirectorySettings.Read(root));
        }
        [Fact] public void AlternateFilenamesAreAccepted()
        {
            string folder=MakeFaces(); File.Move(Path.Combine(folder,names[1]),Path.Combine(folder,"AptosBold.ttf"));
            File.Move(Path.Combine(folder,names[3]),Path.Combine(folder,"AptosBoldItalic.ttf"));
            Assert.True(FontDirectorySettings.HasFourFaces(folder));
        }
        [Fact] public void InvalidAndOversizedSettingsFallBack()
        {
            string folder=MakeFaces(); Assert.True(FontDirectorySettings.Save(root,folder));
            string path=Path.Combine(root,"EAGLE Auto","Appearance","font-directory.txt");
            File.WriteAllBytes(path,new byte[]{0xff,0xff}); Assert.Null(FontDirectorySettings.Read(root));
            File.WriteAllBytes(path,new byte[16385]); Assert.Null(FontDirectorySettings.Read(root));
        }
        [Theory][InlineData(0)][InlineData(11)][InlineData(4*1024*1024+1)]
        public void EmptyShortAndOversizedFontFilesAreRejected(int size)
        {
            string folder=MakeFaces(); using(var f=File.Create(Path.Combine(folder,names[0])))f.SetLength(size);
            Assert.False(FontDirectorySettings.HasFourFaces(folder));
        }
        [Fact] public void NonFontDataIsRejectedBeforeNativeLoading()
        { string folder=MakeFaces(); File.WriteAllBytes(Path.Combine(folder,names[0]),new byte[12]);Assert.False(FontDirectorySettings.HasFourFaces(folder)); }
        [Theory][InlineData(null)][InlineData("")][InlineData("Fonts")][InlineData("https://example.test/fonts")]
        public void InvalidSelectionsDoNotWriteSettings(string path)
        { Assert.False(FontDirectorySettings.Save(root,path));Assert.False(Directory.Exists(Path.Combine(root,"EAGLE Auto"))); }
    }
}
