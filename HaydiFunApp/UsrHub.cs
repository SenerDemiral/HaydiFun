using DataLibrary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using static HaydiFunApp.EtkHub;
using static HaydiFunApp.UsrHub;
using static MudBlazor.CategoryTypes;
using static HaydiFunApp.Cnst;

namespace HaydiFunApp;
// Login olmus Userlar, eger birden cok yerdeyse sayisi
public sealed class UsrHub
{
    public ConcurrentDictionary<int, UsrMdl> UsrD;
    private readonly IDataAccess db;
    private readonly IPubs pubs;
    public UsrHub(IPubs pubs, IDataAccess db)
    {
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        UsrD = new(concurrencyLevel, initialCapacity);

        this.pubs = pubs;
        this.db = db;
    }
    public void LoadAll()
    {
        UsrD.Clear();
        Task<IEnumerable<UsrMdl>> res = db.LoadDataAsync<UsrMdl, dynamic>("select * from UT_GETALL", new { });

        foreach (var itm in res.Result)
        {
            UsrMdl m = new()
            {
                UTid = itm.UTid,
                Usr = itm.Usr,
                Info = itm.Info,
                Avatar = itm.Avatar,
                Lbls = itm.Lbls,
                LblAds = itm.LblAds,
                //MbrD = AAA(itm.Mbrs)
            };
            StringToDictionary(itm.Fans, m.FanD);
            StringToHashSet(itm.Lbls, m.LblH);

            UsrD.TryAdd(itm.UTid, m);

        }
        //StringToDictionary(itm.Mbrs, ChatD[etId].MbrD);
        //if (ChatD.ContainsKey(etId))
        //{

        //}
        //ChatD[etId] = mdl2;
    }

    public void RefreshUsr(int utId, int cnt = 0)
    {
        int NOU;
        var itm = db.StoreProc<UsrMdl, dynamic>("UT_GET(@iUTid)", new { iUTid = utId });

        if (UsrD.ContainsKey(utId))
        {
            UsrD[utId].UTid = itm.UTid;
            UsrD[utId].Usr = itm.Usr;
            UsrD[utId].Info = itm.Info;
            UsrD[utId].Avatar = itm.Avatar;
            UsrD[utId].Lbls = itm.Lbls;
            UsrD[utId].LblAds = itm.LblAds;
            if(cnt > 0)
            {
                UsrD[utId].Cnt = cnt;
            }
            Cnst.StringToDictionary(itm.Fans, UsrD[utId].FanD);

            NOU = UsrD.Count(x => x.Value.isOnline);
            pubs.Publish(key: Cnst.UsrChangeEvnt, new { UsrId = utId, Ops = "M", NOU = NOU });
        }
        else
        {
            UsrMdl m = new UsrMdl
            {
                UTid = itm.UTid,
                Usr = itm.Usr,
                Info = itm.Info,
                Avatar = itm.Avatar,
                Lbls = itm.Lbls,
                LblAds = itm.LblAds,
            };
            Cnst.StringToDictionary(itm.Fans, m.FanD);
            UsrD.TryAdd(itm.UTid, m);

            NOU = UsrD.Count(x => x.Value.isOnline);
            pubs.Publish(key: Cnst.UsrChangeEvnt, new { UsrId = utId, Ops = "I", NOU = NOU });
        }
    }
    public void UsrAdd(int usrId, string usr, string avatar)
    {
        if (!UsrD.ContainsKey(usrId))
        {
            UsrD[usrId] = new()
            {
                UTid = usrId,
                Usr = usr,
                EXD = DateTime.Now,
                Avatar = avatar,
                Cnt = 1,
            };
            UsrD[usrId].FanD.Add(usrId + 1, 'T');
            //   pubs.UsrRaise();
        }
        else
        {
            UsrD[usrId].Cnt++;
        }
        //pubs.UsrRaise();

        pubs.Publish(key: Cnst.UsrChangeEvnt, new { NOU = UsrD.Count, Sbj = $"#of Online Users : {UsrD.Count}" });

    }
    public void UsrRemove(int usrId)
    {
        UsrMdl? usr;
        if (UsrD.ContainsKey(usrId))
        {
            UsrD[usrId].Cnt--;

            if (UsrD[usrId].Cnt == 0)
            {
                if (UsrD.TryRemove(usrId, out usr))
                {
                    // User Cikti
                }
            }
            //pubs.UsrRaise();
            pubs.Publish(key: Cnst.UsrChangeEvnt, new { NOU = UsrD.Count, Sbj = $"#of Online Users : {UsrD.Count}" });

        }
    }
    public void UsrEnter(int usrId)
    {
        if (UsrD.ContainsKey(usrId))
        {
            UsrD[usrId].Cnt++;
            
            var NOU = UsrD.Count(x => x.Value.isOnline);
            pubs.Publish(key: Cnst.UsrChangeEvnt, new { UsrId = usrId, Ops = "E", NOU = NOU });
        }
        else
        {
            // Read User from db
            RefreshUsr(usrId, 1);
        }
    }

    public void UsrExit(int usrId)
    {
        UsrMdl? usr;
        if (UsrD.ContainsKey(usrId))
        {
            UsrD[usrId].Cnt--;

            var NOU = UsrD.Count(x => x.Value.isOnline);
            pubs.Publish(key: Cnst.UsrChangeEvnt, new { UsrId = usrId, Ops = "X", NOU = NOU });
        }
    }
    public void UsrModifed(int usrId)
    {
        // Yapildigi yerden Raise et
        // Avatar, Lbls, Fans, Info degisebilir
        // Load UT rec

    }
    public int UsrCnt()
    {
        return UsrD.Count();
    }
    public sealed class UsrMdl
    {
        public int UTid;
        public string? Usr;
        public DateTime? EXD;
        public int Cnt; // if 0 Offline
        public string? Info;
        public string? Avatar;
        public string? Lbls;
        public string? LblAds;
        public string? Fans;
        public string ImgUrl => $"uploads/{Avatar}?width=50";
        public string ImgUrlBig => $"uploads/{Avatar}?width=360";

        public bool isOnline => Cnt == 0 ? false : true;
        public Dictionary<int, char> FanD = new();
        public HashSet<int> LblH = new();

    }
}

