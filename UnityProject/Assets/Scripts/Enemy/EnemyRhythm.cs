using UnityEngine;
using System.Collections;

public class ImageSpawn : MonoBehaviour
{
    public GameObject UpPrefab;
    public GameObject DownPrefab;
    public GameObject LeftPrefab;
    public GameObject RightPrefab;

    private float bpm = 126f;
    private Queue images;
    private int index;

    void Start()
    {
        index = 0;
        images = new Queue();
        InvokeRepeating("SpawnImage", 0, 60f / bpm * 2);
        InvokeRepeating("RemoveImage", 60f / bpm * 2, 60f / bpm * 2);
    }

    void SpawnImage()
    {
        GameObject[] image_list = new GameObject[4];
        image_list[0] = UpPrefab;
        image_list[1] = DownPrefab;
        image_list[2] = LeftPrefab;
        image_list[3] = RightPrefab;
        index = (index + 1) % 4;
        /* switch (random_value) */
        /* { */
        /*     case 0: */
        /*         image = Instantiate(UpPrefab, new Vector3(-280, 0, 0), Quaternion.identity) as GameObject; */
        /*         break; */
        /*     case 1: */
        /*         image = Instantiate(DownPrefab, new Vector3(-100, 0, 0), Quaternion.identity) as GameObject; */
        /*         break; */
        /*     case 2: */
        /*         image = Instantiate(LeftPrefab, new Vector3(80, 0, 0), Quaternion.identity) as GameObject; */
        /*         break; */
        /*     case 3: */
        /*         image = Instantiate(RightPrefab, new Vector3(180, 0, 0), Quaternion.identity) as GameObject; */
        /*         break; */
        /* } */
        
        GameObject image = Instantiate(image_list[index], new Vector3(-280 + (index * 180), 0, 0), Quaternion.identity);
        images.Enqueue(image);
        image.transform.SetParent(gameObject.transform, false);
    }

    void RemoveImage()
    {
        GameObject oldest_image = images.Dequeue() as GameObject;
        Destroy(oldest_image);
    }
}
