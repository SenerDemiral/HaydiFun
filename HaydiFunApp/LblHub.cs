using DataLibrary;
using static HaydiFunApp.UsrHub;
using System.Collections.Concurrent;

namespace HaydiFunApp;

public sealed class LblHub
{
    public Dictionary<int, LT> LblD;
    public Dictionary<int, LG> LblGD;

    private readonly IDataAccess db;
    public LblHub(IDataAccess db)
    {
        this.db = db;
    }

    public void LoadAll()
    {
        LblD = db.LoadData<LT, dynamic>("select * from LT order by LGid, Name", new { }).ToDictionary((x) => x.LTid);
        LblGD = db.LoadData<LG, dynamic>("select * from LG order by LGid", new { }).ToDictionary((x) => x.LGid);
    }

    public List<string> Lbls2List(string lbls)
    {
        List<string> lst = new();
        int key;
        string[] sa = lbls.Split(",", StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var itm in sa)
        {
            key = Int32.Parse(itm);
            lst.Add(LblD[key].FullName);
        }
        return lst;
    }
}

public sealed class LT
{
    public int LTid { get; set; }
    public string? Name { get; set; }
    public string? FullName { get; set; }
    public bool isDisabled { get; set; } = false;
    public int LGid { get; set; }
}

public sealed class LG
{
    public int LGid { get; set; }
    public string? Name { get; set; }
    public string? Mny { get; set; }
    public bool isMny
    {
        get => Mny == "E" ? true : false;
        set { Mny = value ? "E" : "H"; }
    }
    public bool isExpanded { get; set; }
}

