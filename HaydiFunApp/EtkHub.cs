using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace HaydiFunApp;

public class EtkHub
{
    public ConcurrentDictionary<int, EtkMdl> Etks;
    public ConcurrentDictionary<int, EtkMdl2> Etks2;
    private readonly IPubs pubs;
    public EtkHub(IPubs pubs)
    {
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        Etks = new(concurrencyLevel, initialCapacity);
        Etks2 = new(concurrencyLevel, initialCapacity);

        this.pubs = pubs;

        Etks.TryAdd(1, new EtkMdl { ETid = 1, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(10), Mbrs = new List<string> { "11:K", "12:R", "13:k" } });
        Etks.TryAdd(2, new EtkMdl { ETid = 2, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(20), Mbrs = new List<string> { "11:R", "13:K", "14:r" } });
        Etks.TryAdd(3, new EtkMdl { ETid = 3, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(30), Mbrs = new List<string> { "11:B", "14:K", "15:K" } });

        Etks2.TryAdd(1, new EtkMdl2 { ETid = 1, OwnId = 10, OwnUsr = "AA10", EXD = DateTime.Now.AddSeconds(10), Mbrs = new Dictionary<int, char> { { 11, 'K' }, { 12, 'R' }, { 13, 'k' } } });
        Etks2.TryAdd(2, new EtkMdl2 { ETid = 2, OwnId = 13, OwnUsr = "BB13", EXD = DateTime.Now.AddSeconds(20), Mbrs = new Dictionary<int, char> { { 11, 'R' }, { 13, 'K' }, { 14, 'r' } } });
        Etks2.TryAdd(3, new EtkMdl2 { ETid = 3, OwnId = 11, OwnUsr = "CC11", EXD = DateTime.Now.AddSeconds(30), Mbrs = new Dictionary<int, char> { { 11, 'B' }, { 14, 'K' }, { 15, 'K' } } });
    }

    public void Deneme()
    {
        var aaa = Etks.Where(x => x.Value.Mbrs.Contains("11:K")).Select(x => x.Value).ToList();
        var bbb = Etks.Where(x => x.Value.Mbrs.Any(x => x == "11:K" || x == "11:B")).Select(x => x.Value).ToList();

        string mbrs = "K31,B45,r66";
        StringToDictionary(mbrs, Etks2[3].Mbrs);

        //string[] mbra = mbrs.Split(",", StringSplitOptions.RemoveEmptyEntries);
        //foreach (var m in mbra)
        //{
        //    char c = m[0];
        //    int i  = Int32.Parse(m.Substring(1));
        //    Etks2[3].Mbrs[i] = c;
        //}
        //UpdEtkMbrStu(3, "11:B", "11:r");

        //var ddd = GetUsrEtkts("11");

        MdfEtkMbr2(1, 11, 'Z'); // Update OK
        MdfEtkMbr2(1, 99, 'K'); // Insert OK

        //var ne = new EtkMdl2 { ETid = 1, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(10), Mbrs = new Dictionary<int, string> { { 11, "K" }, { 12, "R" }, { 13, "k" } } });
        //var ne = Etks2[1];
        //ne.ETid = 55;
        //ne.EXD = DateTime.Now.AddDays(1);
        //MdfEtk2(ne);
        var ddd = GetUsrEtkts(13);

    }

    public void StringToDictionary(string s, Dictionary<int, char> dst)
    {
        dst.Clear();
        string[] sa = s.Split(",", StringSplitOptions.RemoveEmptyEntries);
        foreach (var m in sa)
        {
            char c = m[0];
            int i = Int32.Parse(m.Substring(1));
            dst[i] = c;
        }
    }
    public Dictionary<int,char> StringToDictionary(string s)
    {
        Dictionary<int, char> d = new Dictionary<int, char>();
        string[] sa = s.Split(",", StringSplitOptions.RemoveEmptyEntries);
        foreach (var m in sa)
        {
            char c = m[0];
            int i = Int32.Parse(m.Substring(1));
            d[i] = c;
        }
        return d;
    }
    public void UpdEtkMbrStu(int etId, string oldMbr, string newMbr)
    {
        var i = Etks[etId].Mbrs.FindIndex(x => x == oldMbr);
        if (i != -1)
            Etks[etId].Mbrs[i] = newMbr;
    }
    public void MdfEtk2(EtkMdl2 mdl2)
    {
        int etId = mdl2.ETid;
        Etks2[etId] = mdl2;
    }

    // Update/Insert member
    public void MdfEtkMbr2(int etId, int mbr, char stu)
    {
        if (Etks2.ContainsKey(etId))
        {
            Etks2[etId].Mbrs[mbr] = stu;
        }
        //if (Etks2.TryGetValue(etId, out var etkt))
        //{
        //    if (etkt.Mbrs.TryGetValue(mbr, out var _))
        //        Etks2[etId].Mbrs[mbr] = newStu;
        //    else
        //    {
        //        Etks2[etId].Mbrs[mbr] = newStu;
        //    }
        //}
    }

    public List<EtkMdl> GetUsrEtkts(string mbr)
    {
        mbr += ":";
        // mbr.Stu ne olursa User gorecek?
        var bbb = Etks
            //.Where(x => x.Value.Mbrs.Any(x => x == "11:K" || x == "11:B"))
            .Where(x => x.Value.Mbrs.Any(x => x.StartsWith(mbr)))
            .Select(x => x.Value)
            .OrderByDescending(x => x.EXD)
            .ToList();

        return bbb;
    }

    public List<EtkMdl2> GetUsrEtkts(int mbr)
    {
        // mbr.Stu ne olursa User gorecek?
        var bbb = Etks2
            //.Where(x => x.Value.Mbrs.Any(x => x == "11:K" || x == "11:B"))
            .Where(x => x.Value.Mbrs.ContainsKey(mbr))
            .Select(x => x.Value)
            .OrderByDescending(x => x.EXD)
            .ToList();

        foreach(var itm in bbb)
        {
            itm.UsrId = mbr;
            itm.UsrStu = itm.Mbrs[mbr];
            if (itm.OwnId == mbr)
            {
                itm.isOwnr = true;
            }
        }

        return bbb;
    }
    public sealed class EtkMdl
    {
        public int ETid;
        public int UTid;
        public string? Usr;
        public DateTime? EXD;
        public List<string> Mbrs = new();
    }
    public sealed class EtkMdl2
    {
        public int ETid;
        public int OwnId;
        public string? OwnUsr;
        public DateTime? EXD;
        public Dictionary<int, char> Mbrs = new();
        public string? Lbls;

        public int UsrId;
        public char UsrStu;
        public bool isOwnr;
    }

}