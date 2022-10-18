using System.Collections.Concurrent;

namespace HaydiFunApp;

public class EtkHub
{
    public ConcurrentDictionary<int, EtkMdl> Etks;
    private readonly IPubs pubs;
    public EtkHub(IPubs pubs)
    {
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        Etks = new(concurrencyLevel, initialCapacity);

        this.pubs = pubs;

        Etks.TryAdd(1, new EtkMdl { ETid = 1, UTid = 10, Usr = "AA10", EXD = DateTime.Now, Mbrs = new List<string> { "11:K", "12:R", "13:k" } });
        Etks.TryAdd(2, new EtkMdl { ETid = 2, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(10), Mbrs = new List<string> { "11:R", "13:K", "14:r" } });
        Etks.TryAdd(3, new EtkMdl { ETid = 3, UTid = 10, Usr = "AA10", EXD = DateTime.Now.AddSeconds(20), Mbrs = new List<string> { "11:B", "14:K", "15:K" } });

    }

    public void Deneme()
    {
        var aaa = Etks.Where(x => x.Value.Mbrs.Contains("11:K")).Select(x => x.Value).ToList();
        var bbb = Etks.Where(x => x.Value.Mbrs.Any(x => x == "11:K" || x == "11:B")).Select(x => x.Value).ToList();

        var ccc = bbb.OrderByDescending(x => x.EXD);
        //Update 3 nolu Etkinligin 11:B memberi 11:k 
        var ddd = Etks[3].Mbrs.FindIndex(x => x == "11:B");
        Etks[3].Mbrs[ddd] = "11:k";
    }
    public sealed class EtkMdl
    {
        public int ETid;
        public int UTid;
        public string? Usr;
        public DateTime? EXD;
        public List<string> Mbrs = new();
    }

}