using System.ComponentModel;
using static HaydiFunApp.Pubs;

namespace HaydiFunApp;

public interface IPubs
{
    public void Subscribe(string key, Action<dynamic> handler);
    public void UnSubscribe(string key, Action<dynamic> handler);
    public void Publish(string key, dynamic prms);
    public bool HasSubscription(string key);
    public int OnLineUsrCnt();
}