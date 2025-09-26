using IniParser;
using IniParser.Model;

namespace Shared;

/// <summary>
/// Interacting with the .ini files.
/// </summary>
public static class IniFile
{
    private static readonly System.Text.Encoding Encoding = System.Text.Encoding.UTF8;

    /// <summary>
    /// IMI Parser for files.
    /// </summary>
    public static FileIniDataParser DataParser { get; private set; } = new();

    /// <summary>
    /// Reading the <see cref="KeyData"/> in <paramref name="section"/> with the <paramref name="key"/> from <paramref name="filename"/>
    /// </summary>
    /// <param name="filename">FileName to Read</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <returns>Readed <see cref="KeyData"/> or <see langword="null"/></returns>
    public static KeyData? GetKeyData(string filename, string section, string key)
    {
        IniData data = DataParser.ReadFile(filename, Encoding);
        if (!data.Sections.ContainsSection(section))
            return null;
        if (!data[section].ContainsKey(key))
            return null;
        return data[section].GetKeyData(key);
    }

    /// <summary>
    /// Reading a value in <paramref name="section"/> with the <paramref name="key"/> from <paramref name="filename"/>
    /// </summary>
    /// <param name="filename">FileName to Read</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <returns>Readed value or <see cref="string.Empty"/></returns>
    public static string Read(string filename, string section, string key)
    {
        KeyData? data = GetKeyData(filename, section, key);
        if (data == null)
            return string.Empty;
        return data.Value;
    }

    /// <summary>
    /// Reading a value <typeparamref name="T"/> type in <paramref name="section"/> with the <paramref name="key"/> from <paramref name="filename"/>
    /// </summary>
    /// <typeparam name="T">A Type that has <see cref="IParsable{TSelf}"/></typeparam>
    /// <param name="filename">FileName to Read</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <returns>Readed value or default</returns>
    public static T? Read<T>(string filename, string section, string key) where T : IParsable<T>
    {
        string readed = Read(filename, section, key);
        if (string.IsNullOrEmpty(readed))
            return default;
        T.TryParse(readed, null, out T? value);
        return value;
    }

    /// <summary>
    /// Reading comments in <paramref name="section"/> with the <paramref name="key"/> from <paramref name="filename"/>
    /// </summary>
    /// <param name="filename">FileName to Read</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <returns>Readed comments or Empty list</returns>
    public static List<string> ReadComments(string filename, string section, string key)
    {
        KeyData? data = GetKeyData(filename, section, key);
        if (data == null)
            return [];
        return data.Comments;
    }

    /// <summary>
    /// Checks if the <paramref name="section"/> with and <paramref name="key"/> exists inside the <paramref name="filename"/>
    /// </summary>
    /// <param name="filename">FileName to read from</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <returns><see langword="true"/> if the contains a value otherwise, <see langword="false"/>.</returns>
    public static bool Exists(string filename, string section, string key)
    {
        string readed = Read(filename, section, key);
        return !string.IsNullOrEmpty(readed);
    }

    private static void WriteTemp(string filename)
    {
        if (File.Exists(filename))
            return;
        string? path = Path.GetDirectoryName(filename);
        if (!string.IsNullOrEmpty(path))
            Directory.CreateDirectory(path);
        File.WriteAllText(filename, $"{DataParser.Parser.Configuration.CommentString}Temp");
    }

    /// <summary>
    /// Write a <paramref name="value"/> to the <paramref name="filename"/> with a Key of <paramref name="key"/> and a Section as <paramref name="section"/>
    /// </summary>
    /// <param name="filename">FileName to write to</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <param name="value">The Value</param>
    public static void Write(string filename, string section, string key, string? value)
    {
        if (value == null)
            return;
        WriteTemp(filename);
        IniData data = DataParser.ReadFile(filename, Encoding);
        data[section][key] = value;
        DataParser.WriteFile(filename, data, Encoding);
    }

    /// <summary>
    /// Write a <paramref name="keyData"/> to the <paramref name="filename"/> in a Section as <paramref name="section"/>
    /// </summary>
    /// <param name="filename">FileName to write to</param>
    /// <param name="section">INI Section</param>
    /// <param name="keyData">Key data</param>
    public static void Write(string filename, string section, KeyData? keyData)
    {
        if (keyData == null)
            return;
        WriteTemp(filename);
        IniData data = DataParser.ReadFile(filename, Encoding);
        data[section].SetKeyData(keyData);
        DataParser.WriteFile(filename, data, Encoding);
    }

    /// <summary>
    /// Write a <paramref name="value"/> of Type <typeparamref name="T"/> to the <paramref name="filename"/> with a Key of <paramref name="key"/> and a Section as <paramref name="section"/>
    /// </summary>
    /// <typeparam name="T">A Type that has <see cref="IParsable{Self}"/></typeparam>
    /// <param name="filename">FileName to write to</param>
    /// <param name="section">INI Section</param>
    /// <param name="key">INI Key</param>
    /// <param name="value">The Value</param>
    public static void Write<T>(string filename, string section, string key, T value) where T : IParsable<T>
    {
        Write(filename, section, key, value.ToString());
    }
}
