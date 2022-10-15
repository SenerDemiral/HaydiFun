using System.ComponentModel;
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
  private Dictionary<int, Action<int, string>> ChatAction = new();
  public void ChatActionAdd(int id, Action<int, string> handler)
  {
    if (ChatAction.ContainsKey(id))
      ChatAction[id] += handler;
    else
      ChatAction.Add(id, handler);
  }
  public void ChatActionRemove(int id, Action<int, string>? handler)
  {
    if (ChatAction.ContainsKey(id))
    {
      ChatAction[id] -= handler;
      if (ChatAction[id] == null)
        ChatAction.Remove(id);
    }
  }
  public void ChatActionRaise(int id, int x, string y)
  {
    if (ChatAction.ContainsKey(id))
      ChatAction[id](x, y);
  }

  public int ChatActionCount()
  {
    return ChatAction.Count;
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
