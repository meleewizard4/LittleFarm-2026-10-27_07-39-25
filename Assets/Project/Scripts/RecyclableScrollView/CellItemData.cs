using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderData;

public class CellItemData : MonoBehaviour, ICell
{
    public Text nameLabel; 
    public Text desLabel; 

    private InventoryItems _contactInfo;
    private int _cellIndex;

    public void ConfigureCell(InventoryItems invenItems, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = invenItems;

        nameLabel.text = invenItems.Name;
        desLabel.text = invenItems.Description;
    }

}
