using DataLibrary;
using System.Collections.Concurrent;
using System.Text;
using static MudBlazor.CategoryTypes;

namespace HaydiFunApp;

public class EtkHub
{
    public ConcurrentDictionary<int, EtkMdl> EtkD;
    private readonly IPubs pubs;
    private readonly IDataAccess db;
    private readonly UsrHub UsrHub;

    public EtkHub(IPubs pubs, IDataAccess db, UsrHub usrHub)
    {
        this.db = db;
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        EtkD = new(concurrencyLevel, initialCapacity);
        UsrHub = usrHub;
        this.pubs = pubs;

        //long t1 = Environment.TickCount;
        //for (int i = 0; i < 1_000_000; i++)
        //{
        //    ChatD.TryAdd(i,
        //        new ChatMdl
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
        ChatD.TryAdd(2, 
            new ChatMdl { ETid = 2, OwnId = 13, OwnUsr = "BB13", EXD = DateTime.Now.AddSeconds(20), 
                MbrD = new Dictionary<int, char> { { 11, 'R' }, { 13, 'K' }, { 14, 'r' } } });
        ChatD.TryAdd(3, 
            new ChatMdl { ETid = 3, OwnId = 11, OwnUsr = "CC11", EXD = DateTime.Now.AddSeconds(30), 
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
        LoadAll();
        //AddDeneme();
        //ChatD.Clear();
        //AddDeneme();
        //AddDeneme();
        //AddDeneme();

        //var aaa = ChatD.Where(x => x.Value.MbrD.Contains("11:K")).Select(x => x.Value).ToList();
        //var bbb = ChatD.Where(x => x.Value.MbrD.Any(x => x == "11:K" || x == "11:B")).Select(x => x.Value).ToList();

        //string mbrs = "K31,B45,r66";
        //StringToDictionary(mbrs, ChatD[3].MbrD);

        //var ddd = GetUsrEtkts("11");

        //MdfEtkMbr(1, 11, 'Z'); // Update OK
        //MdfEtkMbr(1, 99, 'K'); // Insert OK

        //var ne = new ChatMdl { ETid = 1, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(10), MbrD = new Dictionary<int, string> { { 11, "K" }, { 12, "R" }, { 13, "k" } } });
        //var ne = ChatD[1];
        //ne.ETid = 55;
        //ne.EXD = DateTime.Now.AddDays(1);
        //MdfEtk2(ne);

        //long t1 = Environment.TickCount;
        //var ddd = GetUsrEtks(78);
        //long t2 = Environment.TickCount;
        //long t3 = t2 - t1;
        //Console.WriteLine($"GetUsrEtks: {t3}");
        //ChatMdl ccc = new();

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
            EtkD[etId].LblAds = itm.LblAds;
            Cnst.StringToDictionary(itm.Mbrs, EtkD[etId].MbrD);
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
            Cnst.StringToDictionary(itm.Mbrs, em.MbrD);

            EtkD.TryAdd(itm.ETid, em);
        }
        pubs.Publish(Cnst.EtkChangeEvnt, new { ETid = itm.ETid });
    }

    public void LoadAll()
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
            Cnst.StringToDictionary(itm.Mbrs, em.MbrD);


            EtkD.TryAdd(itm.ETid, em);

        }
    }

    // Update/Insert member
    public void MdfEtkMbr(int etId, int mbr, char stu)
    {
        if (EtkD.ContainsKey(etId))
        {
            EtkD[etId].MbrD[mbr] = stu;
        }
    }

    public bool ToggleEtkUsrStu(int etId, int usrId)
    {
        char oldStu = EtkD[etId].MbrD[usrId];
        char newStu = Cnst.ToggleUsrStu(oldStu);
        if (newStu != ' ' && oldStu != newStu)
        {
            // Save to db
            db.SaveRec("update EM set Stu = @Stu where ETid = @ETid and UTid = @UTid", new { ETid = etId, UTid = usrId, Stu = newStu });
            EtkD[etId].MbrD[usrId] = newStu;
            pubs.Publish(Cnst.EtkChangeEvnt, new { ETid = etId });
            return true;
        }
        return false;
    }

    public bool IsUsrEtkMember(int etId, int usrId)
    {
        // Member stu ya da bak
        if (etId == 0 || usrId == 0)
            return false;
        return EtkD[etId].MbrD.ContainsKey(usrId);
    }
    public List<EtkMdl> GetUsrEtks(int mbr)
    {
        // Memberi oldugu veya Genel Davetleri (Katilmak isteyebilir) gorecek
        // mbr.Stu ne olursa User gorecek?
        List<string> onlineMbrs = new List<string>();
        List<string> MbrAds = new List<string>();
        //.Where(x => x.Value.Typ == 'G' || x.Value.MbrD.ContainsKey(mbr))

        var bbb = EtkD
            .Select(x => x.Value)
            .Where(x => x.MbrD.ContainsKey(mbr))
            .OrderBy(x => x.MbrD[mbr] switch
            {
                'K' => 1,
                'k' => 1,
                '?' => 1,
                '+' => 1,
                '*' => 1,
                _ => 9  // R/r
            })
            .ThenByDescending(x => x.LAD)
            .ToList();

        // Katilmak isteyecegi Genel etkinliklere yoksa member olarak ekle ????
        foreach (var v in bbb)
        {
            if (!v.MbrD.ContainsKey(mbr))
            {
                v.MbrD.Add(mbr, '!');
            }

            v.hasChat = pubs.HasSubscription($"EC:{v.ETid}");

            onlineMbrs.Clear();
            MbrAds.Clear();
            foreach (var key in v.MbrD.Keys)
            {
                if (UsrHub.UsrD[key].isOnline)
                {
                    onlineMbrs.Add(UsrHub.UsrD[key].Usr);
                }
                if (v.Typ == 'O')
                {
                    if (UsrHub.UsrD[key].isOnline)
                        MbrAds.Add(UsrHub.UsrD[key].Usr + "✅");
                    else
                        MbrAds.Add(UsrHub.UsrD[key].Usr);
                }
            }
            v.OnlineMbrs = string.Join(", ", onlineMbrs);
            v.MbrAds = string.Join(", ", MbrAds);

        }

        return bbb;
    }

    public sealed class EtkMdl
    {
        public int ETid;
        public int OwnId;
        public string? OwnUsr;
        public char Typ;
        //public char Aktf;
        public DateTime? EXD;
        public DateTime? LAD;

        private string _Info;
        public string? Info
        {
            get => _Info;
            set
            {
                _Info = value;
                InfoNOL = _Info.Count(x => x == '\n') + 1;
                if(InfoNOL > 5)
                {
                    InfoNOL = 5;
                }
            }
        }
        public int InfoNOL;

        public string? Lbls;
        public string? LblAds;
        public string Mbrs;
        public string? MbrAds;

        public Dictionary<int, char> MbrD = new();
        public string OnlineMbrs = "";
        public int UsrId;
        public char UsrStu;
        public bool isOwnr;
        public bool hasChat;

        //public bool isAktif => Aktif == 'E' ? true : false;
        // Unicode G yesil / Ozel kirmizi birsey bul
        public string TypAd => Typ switch
        {
            'G' => " ",
            'O' => "🔒",
            _ => "X"
        };
    }

}