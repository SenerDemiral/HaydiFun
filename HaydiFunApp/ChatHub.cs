using DataLibrary;
using System.Collections.Concurrent;
using static MudBlazor.CategoryTypes;

namespace HaydiFunApp;

public class ChatHub
{
    public ConcurrentDictionary<int, List<ChatMdl>> ChatD;
    private readonly IPubs pubs;
    private readonly IDataAccess db;

    public ChatHub(IPubs pubs, IDataAccess db)
    {
        this.db = db;
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        ChatD = new(concurrencyLevel, initialCapacity);

        this.pubs = pubs;
    }

    public void RemoveChats(string key)
    {
        int etId = int.Parse(key.Replace("Chats:", ""));
        ChatD.Remove(etId, out _);
    }
    public void AddChat(int etId, int utId, string info)
    {
        // db insert, returns ChatMdl
        // Publish ChatChange
        // ayni zamanda EXD == ET.LAD Publish EtkChange

        var res = db.StoreProc<ChatMdl, dynamic>("EC_INS(@ETid, @UTid, @Info", new { ETid = etId, UTid=utId, Info=info });
        if (res != null)
        {
            ChatD[etId].Add(res);

            // Sadece bunu dinleyenlere gidecek, dinleyen kalmadiginda ChatD[etId].Remove ???
            pubs.Publish($"Chat:{etId}", new { });    
            //pubs.Publish(Cnst.ChatChangeEvnt, new { ETid = etId });
            pubs.Publish(Cnst.EtkChangeEvnt, new { ETid = etId });
        }
    }
    public void LoadEtkChats(int etId)
    {
        if (!ChatD.ContainsKey(etId))
        {
            ChatD[etId] = new List<ChatMdl>();
            var res = db.LoadData<ChatMdl, dynamic>("select * from EC_GETALL(@iETid", new { iETid = etId });
            if (res != null)
            {
                foreach (var itm in res)
                {
                    ChatD[etId].Add(itm);
                }
            }
        }
    }

    public List<ChatMdl> GetEtkChats(int etId)
    {
        if (!ChatD.ContainsKey(etId))
            LoadEtkChats(etId);
        return ChatD[etId];
    }

    public sealed class ChatMdl
    {
        public int ECid;
        public int ETid;
        public int UTid;
        public DateTime? EXD;
        public string? Info;
    }

}