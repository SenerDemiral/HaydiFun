using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace HaydiFunApp;

public sealed class Pubs : IPubs
{
    private class DENEME
    {
        public Action<dynamic> Handler { get; set; }
        public List<int> UsrIds { get; set; }
    }
    private ConcurrentDictionary<string, Action<dynamic>> DynEvent;
    private ConcurrentDictionary<string, DENEME> Deneme;

    public Pubs()
    {
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        DynEvent = new(concurrencyLevel, initialCapacity);

        Deneme = new(concurrencyLevel, initialCapacity);
    }
    #region DenemeHadler
    public event EventHandler? XxChanged; // Subscribe to this
    public void XxRaise()
    {
        XxChanged?.Invoke(this, EventArgs.Empty);
    }
    //-------------------------
    public event EventHandler<AdmMsgEventArgs>? AdmMsgChanged;
    public void AdmMsgRaise(string who, string msg)
    {
        // who: H/A/M/T/#  Herkes, Adm, Mgz, Tnt veya number olabilir
        var args = new AdmMsgEventArgs();
        args.Who = who.ToUpper();
        args.Msg = $"Admin'den duyuru<br/>{msg}";

        AdmMsgChanged?.Invoke(this, args);
    }
    //-------------------------
    public event EventHandler<ChatEventArgs>? ChatChanged;
    public void ChatRaise(int grp)
    {
        var args = new ChatEventArgs();
        args.Grp = grp;
        ChatChanged?.Invoke(this, args);
    }
    //-------------------------
    public event EventHandler? UsrChanged;
    public void UsrRaise()
    {
        // Kimin geldigi/gittigi onemli degil
        // Bir kisi birden cok Login olabilir
        UsrChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion DenemeHandler

    /// <summary>
    /// Key: UsrCntChanged  handler: UsrCntChanged Enter/Exit/Login/Logout for every user
    /// Key: UT:{UTid}     handler: UsrPostAdr for OnlineUser
    ///                                 Fr    To
    ///     Davet Edildim               Ownr->User
    ///     Davetime yanit geldi        User->Owner
    ///     Davetime katilmak istiyor   User->Owner
    ///     key: To, prms: Fr, Typ
    /// Key: ET:{ETid}      handler: ChatChanged for Davet/Etkinlik
    /// </summary>
    //-------------------------------------
    public void AddDeneme(string key, Action<dynamic> handler, int usrId)
    {
        if (Deneme.ContainsKey(key))
        {
            Deneme[key].Handler += handler;
            Deneme[key].UsrIds.Add(usrId);
        }
        else
        {
            //DENEME d = new();
            //d.Handler = handler;
            //d.UsrIds = new List<int> { usrId };
            Deneme.TryAdd(key, new DENEME
            {
                Handler = handler,
                UsrIds = new List<int> { usrId }
            });
        }
    }
    public void RemoveDeneme(string key, Action<dynamic> handler, int usrId)
    {
        if (Deneme.ContainsKey(key))
        {
            Deneme[key].Handler -= handler!;
            Deneme[key].UsrIds.Remove(usrId);
            if (Deneme[key].Handler == null)
            {
                DENEME ot;
                Deneme.TryRemove(key, out ot);
            }
        }
    }
    public void RaiseDeneme(string key, dynamic prms)
    {
        if (Deneme.ContainsKey(key))
            Deneme[key].Handler?.Invoke(prms);
    }

    public void Subscribe(string key, Action<dynamic> handler)
    {
        if (DynEvent.ContainsKey(key))
            DynEvent[key] += handler;
        else
            DynEvent.TryAdd(key, handler);
    }

    public void UnSubscribe(string key, Action<dynamic> handler)
    {
        if (DynEvent.ContainsKey(key) && DynEvent[key] != null)
        {
            DynEvent[key] -= handler!;
            if (DynEvent[key] == null)
            {
                DynEvent.TryRemove(key, out _);
            }
        }
    }
    public void Publish(string key, dynamic prms)
    {
        // Ikisi de ayni 
        if (DynEvent.ContainsKey(key))
            DynEvent[key]?.Invoke(prms);

        //if (DynEvent.ContainsKey(key) && DynEvent[key] != null)
        //    DynEvent[key](prms);
    }
    public int OnLineUsrCnt()
    {
        if (DynEvent.ContainsKey(Cnst.UsrChangeEvnt))
            return DynEvent[Cnst.UsrChangeEvnt].GetInvocationList().Count();

        return 0;
    }
    //private List<Chat> ChatUsrList = new();
    //_ = ChatUsrList.Remove(ChatUsrList?.Find(x => x.ChatId == id && x.UTid == id + 1));
    //return ChatUsrList.Where(x => x.ChatId == id).DistinctBy(x => x.UTid).Count();

    private sealed class Chat
    {
        public int ChatId;
        public int UsrId;
    }
}



public sealed class AdmMsgEventArgs : EventArgs
{
    public string? Who;
    public string? Msg;
}
public sealed class ChatEventArgs : EventArgs
{
    public int Grp;
}
