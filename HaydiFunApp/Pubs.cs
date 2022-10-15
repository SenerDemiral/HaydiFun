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
  private ConcurrentDictionary<int, Action<int, string>> ChatAction = new();
  private List<Chat> ChatUsrList = new();
  public void ChatActionAdd(int id, Action<int, string> handler)
  {
    if (ChatAction.ContainsKey(id))
      ChatAction[id] += handler;
    else
      ChatAction.TryAdd(id, handler);
    var aaa = ChatAction[id].GetInvocationList();
    
    ChatUsrList.Add(new Chat { ChatId = id, UsrId = id+1});
    var nnn = ChatUsrList.Where(x => x.ChatId == id).Count();
  }
  public void ChatActionRemove(int id, Action<int, string> handler)
  {
    if (ChatAction.ContainsKey(id))
    {
      ChatAction[id] -= handler!;
      //var aaa = ChatAction[id].GetInvocationList();
      if (ChatAction[id] == null)
      {
        Action<int,string> ot = null;
        ChatAction.TryRemove(id, out ot);
      }
    }
    //ChatUsrList.Remove(new Chat { ChatId = id, UsrId = id + 1 });
    //var ccc = ChatUsrList.Where(x => x.ChatId == id && x.UsrId == id+1).FirstOrDefault(); // yoksa null
    //var ddd = ChatUsrList.FirstOrDefault(x => x.ChatId == id && x.UsrId == id + 1); // yoksa null
    var eee = ChatUsrList.Find(x => x.ChatId == id && x.UsrId == id + 1); // yoksa null
    //var fff = ChatUsrList.FindIndex(x => x.ChatId == id && x.UsrId == id + 1); // yoksa -1
    ChatUsrList.Remove(eee);  // null geldiginde hata vermiyor
  }
  public void ChatActionRaise(int id, int x, string y)
  {
    if (ChatAction.ContainsKey(id) && ChatAction[id] != null)
    {
      x = ChatUsrList.Where(x => x.ChatId == id).Count();
      ChatAction[id](x, y);
      //ChatAction[id]?.Invoke(x, y);
    }

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
