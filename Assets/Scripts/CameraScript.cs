using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GridScript grid;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit)) return;
            
            if (hit.collider.gameObject.GetComponent<IslandTileScript>())
            {
                grid.CheckClickedIsland(hit.collider.gameObject.GetComponent<IslandTileScript>().islandID); 
            }
        }
    }
    
}
