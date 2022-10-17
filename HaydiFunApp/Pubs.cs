using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace HaydiFunApp;

public sealed class Pubs : IPubs
{
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

  /// <summary>
  /// Bunu kullan
  /// </summary>
  private List<Chat> ChatUsrList = new();
  
  //-------------------------------------
  private ConcurrentDictionary<string, Action<dynamic>> DynEvent = new();
  public void AddDynEvent(string key, Action<dynamic> handler)
  {
    if (DynEvent.ContainsKey(key))
      DynEvent[key] += handler;
    else
      DynEvent.TryAdd(key, handler);
  }
  public void RemoveDynEvent(string key, Action<dynamic> handler)
  {
    if (DynEvent.ContainsKey(key))
    {
      DynEvent[key] -= handler!;
      if (DynEvent[key] == null)
      {
        Action<dynamic> ot = null;
        DynEvent.TryRemove(key, out ot);
      }
    }
  }
  public void RaiseDynEvent(string key, dynamic prms)
  {
    if (DynEvent.ContainsKey(key) && DynEvent[key] != null)
    {
      DynEvent[key](prms);
    }
  }
  public int OnLineUsrCnt()
  {
    if(DynEvent.ContainsKey(Constants.UsrCntChange))
      return DynEvent[Constants.UsrCntChange].GetInvocationList().Count();

    return 0;
  }
  //-------------------------------------
  private ConcurrentDictionary<int, Action<int, string>> ChatAction = new();
  public void ChatActionAdd(int id, Action<int, string> handler)
  {
    if (ChatAction.ContainsKey(id))
      ChatAction[id] += handler;
    else
      ChatAction.TryAdd(id, handler);

    //var aaa = ChatAction[key].GetInvocationList();

    ChatUsrList.Add(new Chat { ChatId = id, UsrId = id + 1 });
  }
  public void ChatActionRemove(int id, Action<int, string> handler)
  {
    if (ChatAction.ContainsKey(id))
    {
      ChatAction[id] -= handler!;
      //var aaa = ChatAction[key].GetInvocationList();
      if (ChatAction[id] == null)
      {
        Action<int, string> ot = null;
        ChatAction.TryRemove(id, out ot);
      }
    }
    _ = ChatUsrList.Remove(ChatUsrList?.Find(x => x.ChatId == id && x.UsrId == id + 1));
  }
  public void ChatActionRaise(int id, int x, string y)
  {
    if (ChatAction.ContainsKey(id) && ChatAction[id] != null)
    {
      x = ChatUsrList.Where(x => x.ChatId == id).Count();
      ChatAction[id](x, y);
      //ChatAction[key]?.Invoke(x, y);
    }
  }
  public int ChatUsrCount(int id)
  {
    return ChatUsrList.Where(x => x.ChatId == id).DistinctBy(x => x.UsrId).Count();
  }
  public int ChatActionCount()
  {
    return ChatAction.Count;
  }

  public int ChatActionHandlerCount(int id)
  {
    if (ChatAction.ContainsKey(id))
      return ChatAction[id].GetInvocationList().Count();
    return 0;
  }
  //-------------------------------------

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
