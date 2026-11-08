using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


public class User {
    public string Name { get; set; }
    public int Gold { get; set; }
    public int Diamond { get; set; }
    public Map UserMap { get; set; }

    public User()
    {
    }

    public User(string name, int gold, int diamond, Map userMap)
    {
        Name = name;
        Gold = gold;
        Diamond = diamond;
        UserMap = userMap;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
