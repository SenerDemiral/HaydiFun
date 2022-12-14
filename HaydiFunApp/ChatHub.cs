using DataLibrary;
using System.Collections.Concurrent;
using static MudBlazor.CategoryTypes;

namespace HaydiFunApp;

public class ChatHub
{
    public ConcurrentDictionary<int, List<ChatMdl>> ChatD;
    private readonly IPubs pubs;
    private readonly IDataAccess db;
    private readonly EtkHub EtkHub;

    public ChatHub(IPubs pubs, IDataAccess db, EtkHub etkHub)
    {
        this.db = db;
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        ChatD = new(concurrencyLevel, initialCapacity);
        EtkHub = etkHub;
        this.pubs = pubs;
    }

    public void RemoveChats(int etId)
    {
        //int etId = int.Parse(key.Replace("EC:", ""));
        ChatD.Remove(etId, out _);
    }
    public void AddChat(int etId, int utId, string info)
    {
        // db insert, returns ChatMdl
        // Publish ChatChange
        // ayni zamanda EXD == ET.LAD Publish EtkChange

        var res = db.StoreProc<ChatMdl, dynamic>("EC_INS(@ETid, @UTid, @Info)", new { ETid = etId, UTid = utId, Info = info });
        if (res != null)
        {
            ChatD[etId].Insert(0, res);
            EtkHub.EtkD[etId].LAD = res.EXD;    // EtkHub LAD a gore diziliyor

            // Sadece bunu dinleyenlere gidecek, dinleyen kalmadiginda ChatD[etId].Remove ???
            pubs.Publish($"EC:{etId}", new { ETid = etId, UTid = utId, Info = info });
            //pubs.Publish(Cnst.ChatChangeEvnt, new { ETid = etId });
            pubs.Publish(Cnst.EtkChangeEvnt, new { ETid = etId, LAD = res.EXD });
        }
    }
    public void LoadEtkChats(int etId)
    {
        if (!ChatD.ContainsKey(etId))
        {
            ChatD[etId] = new List<ChatMdl>();
            var res = db.LoadData<ChatMdl, dynamic>("select * from EC_GETALL(@iETid)", new { iETid = etId });
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
        public DateTime EXD;
        private string _Info;
        public string? Info
        {
            get => _Info;
            set
            {
                _Info = value;
                InfoNOL = _Info.Count(x => x == '\n') + 1;
                if (InfoNOL > 5)
                {
                    InfoNOL = 5;
                }
            }
        }
        public int InfoNOL;
    }

}