using System.ComponentModel;
using static HaydiFunApp.Pubs;

namespace HaydiFunApp;

public interface IPubs
{
  public event EventHandler? XxChanged;
  public void XxRaise();

  public event EventHandler? UsrChanged;
  public void UsrRaise();

  public event EventHandler<AdmMsgEventArgs>? AdmMsgChanged;
  public void AdmMsgRaise(string who, string msg);

  public event EventHandler<ChatEventArgs>? ChatChanged;
  public void ChatRaise(int grp);

  public void ChatActionAdd(int id, Action<int, string> handler);
  public void ChatActionRemove(int id, Action<int, string> handler);
  public void ChatActionRaise(int id, int x, string y);
  public int ChatActionCount();
  public int ChatActionHandlerCount(int id);

  public int ChatUsrCount(int id);

  public void AddDynEvent(string key, Action<dynamic> handler);
  public void RemoveDynEvent(string key, Action<dynamic> handler);
  public void RaiseDynEvent(string key, dynamic prms);
  public int OnLineUsrCnt();
}