using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace HaydiFunApp;

public sealed class Pubs : IPubs
{
    private ConcurrentDictionary<string, Action<dynamic>> DynEvent;
    public IServiceProvider Services { get; }

    public Pubs(IServiceProvider services)
    {
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        DynEvent = new(concurrencyLevel, initialCapacity);
        
        Services = services;
    }

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
                if (DynEvent.TryRemove(key, out var aaaa))
                {
                    if (key.StartsWith("EC:"))
                    {
                        var a = Services.GetRequiredService<ChatHub>();
                        int etId = int.Parse(key.Replace("EC:", ""));
                        a.RemoveChats(etId);
                        Publish(Cnst.EtkChangeEvnt, new { ETid = etId });
                    }
                }
            }
        }
    }

    public bool HasSubscription(string key)
    {
        return DynEvent.ContainsKey(key) && DynEvent[key] != null;
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
