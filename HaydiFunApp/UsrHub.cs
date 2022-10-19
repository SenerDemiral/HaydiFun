using DataLibrary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace HaydiFunApp;
// Login olmus Userlar, eger birden cok yerdeyse sayisi
public sealed class UsrHub
{
    public ConcurrentDictionary<int, UsrModel> Usrs;
    private readonly IPubs pubs;
    public UsrHub(IPubs pubs)
    {
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        Usrs = new(concurrencyLevel, initialCapacity);

        this.pubs = pubs;
    }

    public void UsrAdd(int usrId, string usr, string avatar)
    {
        if (!Usrs.ContainsKey(usrId))
        {
            Usrs[usrId] = new()
            {
                UsrId = usrId,
                Usr = usr,
                EXD = DateTime.Now,
                Avatar = avatar,
                Cnt = 1,
            };
            Usrs[usrId].Fans.Add(usrId+1, 'T');
            //   pubs.UsrRaise();
        }
        else
        {
            Usrs[usrId].Cnt++;
        }
        //pubs.UsrRaise();

        pubs.RaiseDynEvent(key: Constants.UsrCntChange, new { NOU = Usrs.Count, Sbj = $"#of Online Users : {Usrs.Count}" });

    }
    public void UsrRemove(int usrId)
    {
        UsrModel? usr;
        if (Usrs.ContainsKey(usrId))
        {
            Usrs[usrId].Cnt--;

            if (Usrs[usrId].Cnt == 0)
            {
                if (Usrs.TryRemove(usrId, out usr))
                {
                    // User Cikti
                }
            }
            //pubs.UsrRaise();
            pubs.RaiseDynEvent(key: Constants.UsrCntChange, new { NOU = Usrs.Count, Sbj = $"#of Online Users : {Usrs.Count}" });

        }
    }
    public void UsrEnter(int usrId, string usr, string avatar)
    {
        if (!Usrs.ContainsKey(usrId))
        {
            // Read User from db
            UsrModel? u = new();
            u.Cnt++;
            Usrs[usrId] = u;
        }
        else
        {
            Usrs[usrId].Cnt++;
        }

        var NOU = Usrs.Where(x => x.Value.isOnline).Count();
        pubs.RaiseDynEvent(key: Constants.UsrCntChange, new { NOU = NOU });

    }

    public void UsrExit(int usrId)
    {
        UsrModel? usr;
        if (Usrs.ContainsKey(usrId))
        {
            Usrs[usrId].Cnt--;

            var NOU = Usrs.Where(x => x.Value.isOnline).Count();
            pubs.RaiseDynEvent(key: Constants.UsrCntChange, new { NOU = NOU });
        }
    }
    public void UsrModifed(int usrId)
    {
        // Yapildigi yerden Raise et
        // Avatar, Lbls, Fans degisebilir
        // Load UT rec
        
    }
    public int UsrCnt()
    {
        return Usrs.Count();
    }
    public sealed class UsrModel
    {
        public int UsrId;
        public string? Usr;
        public DateTime? EXD;
        public int Cnt; // if 0 Offline
        public string? Avatar;
        public string ImgUrl => $"uploads/{Avatar}?width=100&height=100";

        public bool isOnline => Cnt == 0 ? false : true;
        public string Lbls;
        public Dictionary<int, char> Fans = new();

    }
}

