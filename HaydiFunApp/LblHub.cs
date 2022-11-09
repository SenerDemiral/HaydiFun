using DataLibrary;
using static HaydiFunApp.UsrHub;
using System.Collections.Concurrent;
using System.Reflection;

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
        
        foreach (var itm in lbls.Split(",", StringSplitOptions.RemoveEmptyEntries))
        {
            key = Int32.Parse(itm);
            lst.Add(LblD[key].FullName);
        }
        return lst;
        //if (TSG?.osLbls.Count > 0)
        //model.Lbls = string.Join(',', TSG.osLbls.Select(x => x.LTid));
    }

    public List<LT> Lbls2LTlst(string lbls)
    {
        List<LT> lst = new();
        int key;

        foreach (var itm in lbls.Split(",", StringSplitOptions.RemoveEmptyEntries))
        {
            key = Int32.Parse(itm);
            lst.Add(LblD[key]);
        }
        return lst;
    }

    public Dictionary<int, string> Lbls2Dict(string lbls)
    {
        Dictionary<int, string> dict = new();
        int key;

        foreach (var itm in lbls.Split(",", StringSplitOptions.RemoveEmptyEntries))
        {
            key = Int32.Parse(itm);
            dict.Add(key, LblD[key].FullName);
        }
        return dict;
    }

    public string DictList2Lbls(Dictionary<int, string> dict)
    {
        if (dict.Count > 0)
        {
            return string.Join(',', dict.Keys);
        }
        return string.Empty;
    }
}

public sealed record LT
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

