using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmCotroller : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Tree;
    public Tilemap tm_FarmingDirt;

    public TileBase tb_Ground;
    public TileBase tb_Tree;
    public TileBase tb_FarmingDirt;



    // Update is called once per frame
    void Update()
    {
        HandleFarmAction();
    }


    public void HandleFarmAction()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Dao");

            Vector3Int cellPos = tm_FarmingDirt.WorldToCell(transform.position);
            Debug.Log("Cell pos: " + cellPos);

            TileBase crrTileBase = tm_Ground.GetTile(cellPos);

            if (crrTileBase == tb_Ground)
            {
                tm_Ground.SetTile(cellPos, null);
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3Int cellPos = tm_FarmingDirt.WorldToCell(transform.position);
            Debug.Log("Cell pos: " + cellPos);

            TileBase crrTileBase = tm_Ground.GetTile(cellPos);

            if (crrTileBase == null)
            {
                tm_Tree.SetTile(cellPos, tb_Tree);
            }    
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector3Int cellPos = tm_FarmingDirt.WorldToCell(transform.position);
            Debug.Log("Cell pos: " + cellPos);
            TileBase crrTileBase = tm_Tree.GetTile(cellPos);

            if (crrTileBase != null)
            {

                tm_Tree.SetTile(cellPos, null);

                //Lay item va them vao tui do
                InventoryItems itemFlower = new InventoryItems();
                itemFlower.Name = "Test Cay";
                itemFlower.Description = "Cay Test";

                Debug.Log(itemFlower.ToString());
            }
        }

    }



}
