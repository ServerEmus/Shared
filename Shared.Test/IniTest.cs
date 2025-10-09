using IniParser.Model;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Shared.Test;

public class IniTest
{
    private const string TestName = "test.ini";

    [Fact]
    public void TestSimpleFileCreating()
    {
        if (File.Exists(TestName))
            File.Delete(TestName);
        IniFile.Write("test.ini", "test", "str", "test");
        Assert.Multiple(() =>
        {
            Assert.True(File.Exists("test.ini"));
            Assert.Equal("test", IniFile.Read("test.ini", "test", "str"));
        });
    }

    [Fact]
    public void TestParsables()
    {
        IniFile.Write("test.ini", "test", "bool", true);
        Assert.True(IniFile.Read<bool>("test.ini", "test", "bool"));
        IniFile.Write("test.ini", "test", "byte", byte.MaxValue);
        Assert.Equal(byte.MaxValue, IniFile.Read<byte>("test.ini", "test", "byte"));
        IniFile.Write("test.ini", "test", "ushort", ushort.MaxValue);
        Assert.Equal(ushort.MaxValue, IniFile.Read<ushort>("test.ini", "test", "ushort"));
        IniFile.Write("test.ini", "test", "short", short.MaxValue);
        Assert.Equal(short.MaxValue, IniFile.Read<short>("test.ini", "test", "short"));
        IniFile.Write("test.ini", "test", "int", int.MaxValue);
        Assert.Equal(int.MaxValue, IniFile.Read<int>("test.ini", "test", "int"));
        IniFile.Write("test.ini", "test", "uint", uint.MaxValue);
        Assert.Equal(uint.MaxValue, IniFile.Read<uint>("test.ini", "test", "uint"));
        IniFile.Write("test.ini", "test", "long", long.MaxValue);
        Assert.Equal(long.MaxValue, IniFile.Read<long>("test.ini", "test", "long"));
        IniFile.Write("test.ini", "test", "ulong", ulong.MaxValue);
        Assert.Equal(ulong.MaxValue, IniFile.Read<ulong>("test.ini", "test", "ulong"));
    }

    [Fact]
    public void TestCustom()
    {
        MyOwnValue ownValue = new()
        { 
            IP = IPAddress.Loopback,
        };
        IniFile.Write("test.ini", "test", "myown", ownValue);
        Assert.Equal(ownValue, IniFile.Read<MyOwnValue>("test.ini", "test", "myown"));
    }

    [Fact]
    public void TestKeyData()
    {
        // todo
        KeyData keyData = new("keydata")
        { 
            Value = "yeet",
            Comments =
            {
                "My Comment Here",
                "Other comment lol"
            }
        };
        IniFile.Write("test.ini", "test", keyData);
        KeyData? readKeyData = IniFile.GetKeyData("test.ini", "test", "keydata");
        Assert.NotNull(readKeyData);
        Assert.Equal(keyData.KeyName, readKeyData.KeyName);
        Assert.Equal(keyData.Comments, readKeyData.Comments);
        Assert.Equal(keyData.Value, readKeyData.Value);
    }

    public class MyOwnValue : IParsable<MyOwnValue>, IEqualityComparer<MyOwnValue>
    {
        private const string prefix = "myown";
        private const char splitter = '|';
        private const int version = 1;
        public required IPAddress IP;

        public static MyOwnValue Parse(string s, IFormatProvider? provider)
        {
            if (TryParse(s, provider, out MyOwnValue? result))
                return result;
            return new()
            { 
                IP = IPAddress.None,
            };
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out MyOwnValue result)
        {
            result = null;
            if (string.IsNullOrEmpty(s))
                return false;
            if (!s.StartsWith(prefix))
                return false;

            var splitted = s[prefix.Length ..].Split(splitter);
            if (splitted.Length < 1)
                return false;

            // verion check
            if (!int.TryParse(splitted[1], provider, out int ver))
                return false;
            if (ver != version)
                return false;

            // ip
            if (!IPAddress.TryParse(splitted[2], out IPAddress? ip))
                return false;

            result = new()
            { 
                IP  = ip
            };
            return true;
        }

        public override string ToString()
        {
            return $"{prefix}{splitter}{version}{splitter}{IP}{splitter}";
        }

        public override int GetHashCode()
        {
            return version.GetHashCode() + IP.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not MyOwnValue myOwnValue)
                return false;
            return IP.Equals(myOwnValue.IP);
        }

        public bool Equals(MyOwnValue? x, MyOwnValue? y)
        {
            if (x is null && y is null)
                return true;
            if (x == null)
                return false;
            if (y == null)
                return false;
            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] MyOwnValue obj)
        {
            return obj.GetHashCode();
        }
    }
}
