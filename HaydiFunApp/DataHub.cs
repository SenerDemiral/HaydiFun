using DataLibrary;
using System.Collections.Concurrent;
using System.Reflection;

namespace HaydiFunApp;

public sealed class DataHub
{
  public ConcurrentDictionary<int, List<EC>> Chats;
  private ConcurrentDictionary<int, List<ECusr>> ChatUsrs;
  private readonly IDataAccess db;
  private readonly IPubs pub;
  public DataHub(IDataAccess dataAccess, IPubs pub)
  {
    int concurrencyLevel = Environment.ProcessorCount * 2;
    int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
    this.db = dataAccess;
    Chats = new(concurrencyLevel, initialCapacity);
    ChatUsrs = new(concurrencyLevel, initialCapacity);
    this.pub = pub;
  }
  public async Task ChatInit(int grp, int utId)
  {
    // UI Chat'a girerken mutlaka ilk once buraya gelmeli
    if (!Chats.ContainsKey(grp))
    {
      Chats[grp] = (await db.LoadData<EC, dynamic>("select * from EC_GET(@ETid)", new { ETid = grp })).ToList();
    }

    if (!ChatUsrs.ContainsKey(grp))
    {
      ChatUsrs[grp] = new();
      ChatUsrs[grp].Add(new ECusr()
      {
        UTid = utId,
        Cnt = 1
      });
    }
    else
    {
      // Find usr in list and increment 1
      var usr = ChatUsrs[grp].Find(x => x.UTid == utId);
      if (usr != null)
      {
        usr.Cnt++;
      }
      else
      {
        ChatUsrs[grp].Add(new ECusr()
        {
          UTid = utId,
          Cnt = 1
        });
      }
    }
  }
  public async Task ChatAdd(int grp, int utId, string info)
  {
    // ChatInit'i onceden cagirdigi icin gerekli degil
    //if (!Chats.ContainsKey(etId))
    //{
    //    // Chats[etId] = new List<EC>();
    //    Chats[etId] = (await db.LoadData<EC, dynamic>("select * from EC_GET(@ETid)", new { ETid = etId })).ToList();
    //    pub.ChatRaise();
    //}
    var ec = new EC()
    {
      ECid = 0,
      ETid = grp,
      UTid = utId,
      Info = info
    };

    var res = await db.StoreProcAsync<dynamic, EC>("EC_INS(@ETid, @UTid, @Info)", ec);
    ec.ECid = res.ECID;
    ec.UTnn = res.UTNN;
    ec.InsTS = res.INSTS;

    Chats[grp].Insert(0, ec);

    pub.ChatRaise(grp);
    // Publish EC nin ETid sine ekleme yapildi
    // Buna Abone olanlar Refresh yapmali 
    // Aslinda bu Dict uzerinden yapsin tum islemini
  }

  public void ChatRemove(int grp, int utId)
  {
    if (ChatUsrs.ContainsKey(grp))
    {
      // Find usr in list and decrement 1, kalmayinca sil
      var usr = ChatUsrs[grp].Find(x => x.UTid == utId);
      if (usr != null)
      {
        if (usr.Cnt > 1)
          usr.Cnt--;
        else
          ChatUsrs[grp].Remove(usr);
      }
      if (ChatUsrs[grp].Count == 0)
      {
        ChatUsrs.TryRemove(grp, out var lst);
      }
    }
    // ChatPage.Dispose dan burayi cagir.
    // et ye ait user kaydini bir azalt.
    // bu et deki tum 
    // Bu Chat deki tum userlar ciktiysa sil, birdahakine db den okusun
  }
  private sealed class ECusr
  {
    public int UTid;
    public int Cnt;
  }
}

public sealed class EC
{
  public int ECid { get; set; }
  public int ETid { get; set; }
  public int UTid { get; set; }
  public string? UTnn { get; set; }
  public string? Info { get; set; }
  public DateTime? InsTS { get; set; }

}
