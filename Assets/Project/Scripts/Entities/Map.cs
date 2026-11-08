using System.Collections.Generic;
using Newtonsoft.Json;

public class Map
{
    public List<TilemapDetails> ListTilemapDetail { get; set; }

    public Map()
    {
        ListTilemapDetail = new List<TilemapDetails>();
    }

    public Map(List<TilemapDetails> listTilemapDetail)
    {
        ListTilemapDetail =
            listTilemapDetail ?? new List<TilemapDetails>();
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public int GetLength()
    {
        return ListTilemapDetail == null
            ? 0
            : ListTilemapDetail.Count;
    }
}
