using System.Collections.Concurrent;

namespace HaydiFunApp;

public class ChatHub
{
    private ConcurrentDictionary<int, List<int>> ChatDict;

    public ChatHub()
    {
        int concurrencyLevel = Environment.ProcessorCount * 2;
        int initialCapacity = 101;  // 101,199,293,397,499,599,691,797,887,997 PrimeNumber
        ChatDict = new(concurrencyLevel, initialCapacity);
        ChatDict.TryAdd(1, new List<int> { 10, 11, 12, 12, 12 });
        ChatDict.TryAdd(2, new List<int> { 10, 11, 12, 13 });

    }
    public void Deneme()
    {
        ChatDict.TryAdd(3, new List<int> { 10, 11 });

        var qqq = ChatDict[1].Count;                 // # Connection
        var www = ChatDict[1].Distinct().Count();    // # unique user

        // UTid=10 hangi Chatlerde var?
        var eee = ChatDict.Where(x => x.Value.Contains(10)).Select(x => x.Key);
        // Chat yapan tum connectionlar multi same usr
        var ttt = ChatDict.Values.SelectMany(x => x).ToList();
        // Chat yapan Unique Usr sayisi
        var yyy = ChatDict.Values.SelectMany(x => x).Distinct().Count();
        var uuu = ChatDict.Values.SelectMany(x => x).Distinct().ToList();


        ChatDict[1].Remove(12);
        ChatDict[1].Remove(12);
        ChatDict[1].Remove(12);
        ChatDict[1].Remove(12); // Bulamazsa hata vermiyor
        ChatDict[1].Clear();
        if (ChatDict[1].Count == 0)
        {
            ChatDict.Remove(1, out _);
        }
    }

    public void AddChatHub(int etId, int usrId)
    {
        if (ChatDict.ContainsKey(etId))
            ChatDict[etId].Add(usrId);
        else
            ChatDict.TryAdd(etId, new List<int> { usrId });
    }

    public void RemoveChatHub(int etId, int usrId)
    {
        if (ChatDict.ContainsKey(etId))
        {
            ChatDict[etId].Remove(usrId);

            if (ChatDict[etId].Count == 0)
                ChatDict.Remove(etId, out _);
        }
    }

    public int CntUsrChatHub(int etId)
    {
        if(ChatDict.ContainsKey(etId))
            return ChatDict[etId].Distinct().Count();
        
        return 0;
    }
    public int CntDavetChatHub()
    {
        return (int)(ChatDict.Count);
    }
}
