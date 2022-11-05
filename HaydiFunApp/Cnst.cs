using HashidsNet;
using System.Globalization;

namespace HaydiFunApp;

public static class Cnst
{
    public const string PublishVersion = "HaydiFun© v0.5 Şener Demiral®";
    public const string BrowserUsrIdKey = "haydifun";
    public const string Height = "calc(100vh - 140px)";
    public const string UsrChangeEvnt = "UsrChangeEvnt";
    public const string EtkChangeEvnt = "EtkChangeEvnt";
    public const string ChatChangeEvnt = "ChatChangeEvnt";
    public static CultureInfo trCI = new("tr-TR", false);

    public static Hashids hashIds0 = new Hashids("this is my salt", 0);   // Nekadar gerekiyorsa
    public static Hashids hashIds5 = new Hashids("this is my salt", 5);
    public static Hashids hashIds8 = new Hashids("this is my salt", 8);
    public static Hashids hashIds11 = new Hashids("this is my salt", 11);

    public static void StringToDictionary(string str, Dictionary<int, char> dst)
    {
        // str: {char}{int),...
        // dst[int] = char
        char c;
        int i;
        dst.Clear();    // 
        foreach (var m in str.Split(",", StringSplitOptions.RemoveEmptyEntries))
        {
            c = m[0];
            i = Int32.Parse(m.Substring(1));
            dst[i] = c;
        }
    }

    public static void StringToHashSet(string str, HashSet<int> dst)
    {
        dst.Clear();
        int i;
        foreach (var m in str.Split(",", StringSplitOptions.RemoveEmptyEntries))
        {
            i = Int32.Parse(m);
            dst.Add(i);
        }

    }

    public static bool CanChat(char usrStu) => usrStu switch
    {
        '*' => true,
        'K' => true,
        'k' => true,
        _ => false
    };

    public static char ToggleFanTyp(char oldStu) => oldStu switch
    {
        ' ' => 'T',
        'T' => 'E',
        'E' => ' ',
        _ => ' '
    };

    /// <summary>
    /// Usr Davetliyse yapar 
    /// </summary>
    /// <param name="oldStu"></param>
    /// <returns></returns>
    public static char ToggleUsrStu(char oldStu) => oldStu switch
    {
        '?' => 'K',     // Davet edildim -> Katiliyorum
        'K' => 'R',     // Katiliyorum -> Katilamiyorum Red (Bunlari listenin sonuna atmak gerek)
        'R' => 'K',     // Katilamiyorum -> Katiliyorum
        _ => ' '
    };
    //Rune r = new Rune(0x1F535);
    //emoji = r.ToString();
    // "💤"; // "⚪"; //"🟢" "\uD83D\uDDE3"
    //"📞☎✅✔✓➕"

    public static string emjUsrStu(char stu) => stu switch
    {
        '?' => "❔",
        'K' => "👍",
        'R' => "👎",
        '+' => "➕",
        'k' => "👍",
        'r' => "👎",
        _ => "*"
    };
    public static string emjFanTyp(char typ) => typ switch
    {
        'T' => "🟢",     // Takip ediyorum/ediliyorum (Yeşil yuvarlak)
        'E' => "⛔",    // Engelledim/Engellendim
        _ => "_"
    };
    public static string emjChatStu(bool hasChat) => hasChat switch
    {
        true => "😉",
        false => "😴"
    };

    public static string emjOnlineStu(bool isOnline) => isOnline switch
    {
        true => emjOnline,
        false => ""
    };

    public static string emjOnline = "✅";
}
