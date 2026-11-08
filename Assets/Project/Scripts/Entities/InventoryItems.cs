using Newtonsoft.Json;
using UnityEngine;

public class InventoryItems 
{
    public string Name { get; set; }

    public string Description { get; set; }

    public InventoryItems()
    { 
    }

    public InventoryItems(string Name, string Description)
    {
        this.Name = Name;
        this.Description = Description;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
