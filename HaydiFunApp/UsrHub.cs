﻿using DataLibrary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace HaydiFunApp;
// Login olmus Userlar, eger birden cok yerdeyse sayisi
public sealed class UsrHub
{
    public ConcurrentDictionary<int, UsrModel> Usrs;
    private readonly IPubs pubs;
    public UsrHub(IPubs pubs)
    {
        Usrs = new ConcurrentDictionary<int, UsrModel>();
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
                Cnt = 1
            };
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
    public int UsrCnt()
    {
        return Usrs.Count();
    }
    public sealed class UsrModel
    {
        public int UsrId;
        public string? Usr;
        public DateTime? EXD;
        public int Cnt;
        public string? Avatar;
        public string ImgUrl => $"uploads/{Avatar}?width=100";

    }
}

