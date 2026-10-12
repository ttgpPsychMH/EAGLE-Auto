using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using EagleAuto.NativeReaderSimulation;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class NativeReaderEnvelopeTests
    {
        private static JsonDocument Fixtures()=>JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"NativeReaderFixtures.json")));
        private static byte[] Envelope()
        {using var doc=Fixtures();return Convert.FromHexString(doc.RootElement.GetProperty("fixtures").EnumerateArray().First(f=>f.GetProperty("id").GetString()=="length_20").GetProperty("envelope_hex").GetString());}
        [Theory][InlineData("length_1")][InlineData("length_15")][InlineData("length_16")][InlineData("length_20")][InlineData("length_999")][InlineData("length_1000")][InlineData("length_4096")][InlineData("length_65536")]
        public void CppEnvelopeRoundTripsAllOriginalBytes(string id)
        {
            using var doc=Fixtures();var f=doc.RootElement.GetProperty("fixtures").EnumerateArray().Single(f=>f.GetProperty("id").GetString()==id);
            byte[] expected=Convert.FromHexString(f.GetProperty("payload_hex").GetString()),frame=Convert.FromHexString(f.GetProperty("envelope_hex").GetString());
            Assert.True(EnvelopeDecoder.TryDecode(frame,4242,out uint sequence,out var actual));Assert.Equal(1U,sequence);Assert.Equal(expected,actual);
            byte[] legacy=Convert.FromHexString(f.GetProperty("legacy_hex").GetString());
            if(expected.Length>15&&expected.Length<1000){Assert.Equal(expected.Length,legacy.Length);Assert.Equal(expected.Take(expected.Length-4),legacy.Skip(4));}
            else Assert.Empty(legacy);
        }
        [Fact] public void SplitUtf8MustBeJoinedBeforeTextDecoding()
        {
            using var doc=Fixtures();var utf8=new UTF8Encoding(false,true);byte[] merged=Array.Empty<byte>();uint last=0;
            foreach(var f in doc.RootElement.GetProperty("fixtures").EnumerateArray().Where(f=>f.GetProperty("id").GetString().StartsWith("utf8_fragment_")))
            {
                Assert.True(EnvelopeDecoder.TryDecode(Convert.FromHexString(f.GetProperty("envelope_hex").GetString()),4242,out uint seq,out var payload));
                Assert.Equal(last+1,seq);last=seq;merged=merged.Concat(payload).ToArray();
                Assert.Throws<DecoderFallbackException>(()=>utf8.GetString(payload));
            }
            Assert.Equal("AảB",utf8.GetString(merged)); // Synthetic UTF-8 only; actual TLBB framing remains unknown.
        }
        [Theory][InlineData(0)][InlineData(4)][InlineData(5)][InlineData(6)][InlineData(7)][InlineData(8)][InlineData(12)][InlineData(16)][InlineData(20)]
        public void CorruptHeaderFailsClosed(int offset)
        {var bytes=Envelope();bytes[offset]^=1;Assert.False(EnvelopeDecoder.TryDecode(bytes,4242,out _,out _));}
        [Fact] public void ExtraBytesCannotMasqueradeAsOneEnvelope()
        {Assert.False(EnvelopeDecoder.TryDecode(Envelope().Concat(new byte[]{0}).ToArray(),4242,out _,out _));}
        [Fact] public void TruncatedEnvelopeIsRejected()
        {var bytes=Envelope();Array.Resize(ref bytes,bytes.Length-1);Assert.False(EnvelopeDecoder.TryDecode(bytes,4242,out _,out _));}
        [Theory][InlineData(0)][InlineData(-1)][InlineData(4243)]
        public void WrongOrInvalidPidIsRejected(int pid)
        {Assert.False(EnvelopeDecoder.TryDecode(Envelope(),pid,out _,out _));}
        [Fact] public void PayloadLengthOverflowIsRejectedBeforeAllocation()
        {var b=Envelope();for(int i=16;i<20;i++)b[i]=255;Assert.False(EnvelopeDecoder.TryDecode(b,4242,out _,out _));}
        [Fact] public void NullAndTinyInputAreRejected()
        {Assert.False(EnvelopeDecoder.TryDecode(null,4242,out _,out _));Assert.False(EnvelopeDecoder.TryDecode(new byte[23],4242,out _,out _));}
        [Fact] public void DecoderOwnsOutputBuffer()
        {var b=Envelope();Assert.True(EnvelopeDecoder.TryDecode(b,4242,out _,out var payload));byte first=payload[0];b[24]^=255;Assert.Equal(first,payload[0]);}
    }
}
