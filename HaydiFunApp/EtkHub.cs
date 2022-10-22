using DataLibrary;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using static MudBlazor.CategoryTypes;

namespace HaydiFunApp;

public class EtkHub
{
    public ConcurrentDictionary<int, EtkMdl> EtkD;
    private readonly IPubs pubs;
    private readonly IDataAccess db;

    public EtkHub(IPubs pubs, IDataAccess db)
    {
        this.db = db;
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        EtkD = new(concurrencyLevel, initialCapacity);

        this.pubs = pubs;

        //long t1 = Environment.TickCount;
        //for (int i = 0; i < 1_000_000; i++)
        //{
        //    EtkD.TryAdd(i,
        //        new EtkMdl
        //        {
        //            ETid = i,
        //            OwnId = 10,
        //            OwnUsr = "AA10",
        //            EXD = DateTime.Now.AddSeconds(10),
        //            MbrD = { { 11, 'K' }, { 12, 'R' }, { 13, 'k' } }
        //            //MbrD = new Dictionary<int, char> { { 11, 'K' }, { 12, 'R' }, { 13, 'k' } }
        //        });
        //}
        //long t2 = Environment.TickCount;
        //Console.WriteLine($"SetEtks: {t2 - t1}");


        /*
        EtkD.TryAdd(2, 
            new EtkMdl { ETid = 2, OwnId = 13, OwnUsr = "BB13", EXD = DateTime.Now.AddSeconds(20), 
                MbrD = new Dictionary<int, char> { { 11, 'R' }, { 13, 'K' }, { 14, 'r' } } });
        EtkD.TryAdd(3, 
            new EtkMdl { ETid = 3, OwnId = 11, OwnUsr = "CC11", EXD = DateTime.Now.AddSeconds(30), 
                MbrD = new Dictionary<int, char> { { 11, 'B' }, { 14, 'K' }, { 15, 'K' } } });
        */
    }

    public void AddDeneme()
    {
        long t1 = Environment.TickCount;
        for (int i = 0; i < 1_000_000; i++)
        {
            EtkD.TryAdd(i,
                new EtkMdl
                {
                    ETid = i,
                    Typ = 'G',
                    OwnId = 10,
                    OwnUsr = "AA10",
                    EXD = DateTime.Now.AddSeconds(10),
                    MbrD = { { i, 'K' }, { i + 1, 'R' }, { i + 2, 'k' } }
                    //MbrD = new Dictionary<int, char> { { 11, 'K' }, { 12, 'R' }, { i, 'k' } }
                });
            //Task.Delay(1);
        }
        long t2 = Environment.TickCount;
        Console.WriteLine($"SetEtks: {t2 - t1}");
    }

    public void Deneme()
    {
        LoadAllEtk();
        //AddDeneme();
        //EtkD.Clear();
        //AddDeneme();
        //AddDeneme();
        //AddDeneme();

        //var aaa = EtkD.Where(x => x.Value.MbrD.Contains("11:K")).Select(x => x.Value).ToList();
        //var bbb = EtkD.Where(x => x.Value.MbrD.Any(x => x == "11:K" || x == "11:B")).Select(x => x.Value).ToList();

        //string mbrs = "K31,B45,r66";
        //StringToDictionary(mbrs, EtkD[3].MbrD);

        //var ddd = GetUsrEtkts("11");

        //MdfEtkMbr(1, 11, 'Z'); // Update OK
        //MdfEtkMbr(1, 99, 'K'); // Insert OK

        //var ne = new EtkMdl { ETid = 1, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(10), MbrD = new Dictionary<int, string> { { 11, "K" }, { 12, "R" }, { 13, "k" } } });
        //var ne = EtkD[1];
        //ne.ETid = 55;
        //ne.EXD = DateTime.Now.AddDays(1);
        //MdfEtk2(ne);

        //long t1 = Environment.TickCount;
        //var ddd = GetUsrEtks(78);
        //long t2 = Environment.TickCount;
        //long t3 = t2 - t1;
        //Console.WriteLine($"GetUsrEtks: {t3}");
        //EtkMdl ccc = new();

        //ccc.MbrD[123] = 'X';

        //RefreshEtk(12);

    }

    public void RefreshEtk(int etId)
    {
        var itm = db.StoreProc<EtkMdl, dynamic>("ET_GET(@iETid)", new { iETid = etId });

        if (EtkD.ContainsKey(etId))
        {
            EtkD[etId].Typ = itm.Typ;
            EtkD[etId].EXD = itm.EXD;
            EtkD[etId].LAD = itm.LAD;
            EtkD[etId].Info = itm.Info;
            EtkD[etId].Lbls = itm.Lbls;
            Constants.StringToDictionary(itm.Mbrs, EtkD[etId].MbrD);
        }
        else
        {
            EtkMdl em = new EtkMdl
            {
                ETid = itm.ETid,
                Typ = itm.Typ,
                OwnId = itm.OwnId,
                OwnUsr = itm.OwnUsr,
                EXD = itm.EXD,
                LAD = itm.LAD,
                Info = itm.Info,
                Lbls = itm.Lbls,
                LblAds = itm.LblAds,
            };
            Constants.StringToDictionary(itm.Mbrs, em.MbrD);

            EtkD.TryAdd(itm.ETid, em);
        }
    }

    public void LoadAllEtk()
    {
        EtkD.Clear();
        Task<IEnumerable<EtkMdl>> res = db.LoadDataAsync<EtkMdl, dynamic>("select * from ET_GETALL", new { });

        foreach (var itm in res.Result)
        {
            EtkMdl em = new EtkMdl
            {
                ETid = itm.ETid,
                Typ = itm.Typ,
                OwnId = itm.OwnId,
                OwnUsr = itm.OwnUsr,
                EXD = itm.EXD,
                LAD = itm.LAD,
                Info = itm.Info,
                Lbls = itm.Lbls,
                LblAds = itm.LblAds,
                //MbrD = AAA(itm.Mbrs)
            };
            Constants.StringToDictionary(itm.Mbrs, em.MbrD);

            EtkD.TryAdd(itm.ETid, em);

        }
        //StringToDictionary(itm.Mbrs, EtkD[etId].MbrD);
        //if (EtkD.ContainsKey(etId))
        //{

        //}
        //EtkD[etId] = mdl2;
    }

    // Update/Insert member
    public void MdfEtkMbr(int etId, int mbr, char stu)
    {
        if (EtkD.ContainsKey(etId))
        {
            EtkD[etId].MbrD[mbr] = stu;
        }
    }

    public List<EtkMdl> GetUsrEtks(int mbr)
    {
        // mbr.Stu ne olursa User gorecek?
        var bbb = EtkD
            .Where(x => x.Value.MbrD.ContainsKey(mbr))
            .Select(x => x.Value)
            .OrderByDescending(x => x.EXD)
            .ToList();

        //foreach(var itm in bbb)
        //{
        //    itm.UTid = mbr;
        //    itm.UsrStu = itm.MbrD[mbr];
        //    if (itm.OwnId == mbr)
        //    {
        //        itm.isOwnr = true;
        //    }
        //}

        return bbb;
    }

    public Dictionary<int, char> AAA(string str)
    {
        Dictionary<int, char> dst = new();
        char c;
        int i;
        string[] sa = str.Split(",", StringSplitOptions.RemoveEmptyEntries);
        foreach (var m in sa)
        {
            c = m[0];
            i = Int32.Parse(m.Substring(1));
            dst[i] = c;
        }
        return dst;
    }
    public sealed class EtkMdl
    {
        public int ETid;
        public int OwnId;
        public string? OwnUsr;
        public char Typ;
        public DateTime? EXD;
        public DateTime? LAD;
        public string? Info;
        public string? Lbls;
        public string? LblAds;
        public string Mbrs;

        public Dictionary<int, char> MbrD = new();
        public int UsrId;
        public char UsrStu;
        public bool isOwnr;

        // Unicode G yesil / Ozel kirmizi birsey bul
        public string TypAd => Typ switch
        {
            'G' => "Gnl",
            'O' => "Özl",
            _ => "???"
        };
    }

}