using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    public GameObject player;

    public GameObject floor;
    public GameObject wall;
    public GameObject ceiling;
    public GameObject light;
    public GameObject frame;
    public GameObject door;
    public GameObject exit;
    public GameObject fire;
    public GameObject extinguisher;

    public GameObject manager;

    public int size;
    public int minArea = 10;
    public int minEdge = 3;
    public float tile = 4.0f;

    public int[,] field;
    public GameObject[] objects;
    
    int level;

    GameObject newDoor;

    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("level", 1);
        size = 5 + 5 * level;

        field = new int[size, size];

        Divide(0, size-1, 0, size-1, false);

        int fireCount = (int) Mathf.Floor(level / 2) + 1;
        for (int i=0; i<fireCount; i++)
            AddFire(size);

        CreateField();

        SpawnObjects();

        manager.GetComponent<Manager>().Begin();

        player.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Divide(int left, int right, int top, int bottom, bool direction) {

        int width = right - left + 1;
        int height = bottom - top + 1;
        int area = width * height;

        if (width <= 2*minEdge || height <= 2*minEdge || area <= minArea) return;

        if (direction) {

            // VERTICAL CUT
            int slice = left + minEdge + (int) Mathf.Floor(Random.Range(0, (width - 2*minEdge)));
            for (int i=top; i<bottom+1; i++) {
                field[i, slice] = 1;
            }

            int firstDoor = top + (int) Mathf.Floor(Random.Range(0, height));
            int secondDoor = top + (int) Mathf.Floor(Random.Range(0, height));
            field[firstDoor, slice-1] = 2;
            field[secondDoor, slice+1] = 2;

            Divide(left, slice-1, top, bottom, !direction);
            Divide(slice+1, right, top, bottom, !direction);


        } else {

            // HORIZONTAL CUT
            int slice = top + minEdge + (int) Mathf.Floor(Random.Range(0, (height - 2*minEdge)));
            for (int i=left; i<right+1; i++) {
                field[slice, i] = 1;
            }

            int firstDoor = left + (int) Mathf.Floor(Random.Range(0, width));
            int secondDoor = left + (int) Mathf.Floor(Random.Range(0, width));
            field[slice-1, firstDoor] = 2;
            field[slice+1, secondDoor] = 2;

            Divide(left, right, top, slice-1, !direction);
            Divide(left, right, slice+1, bottom, !direction);
        }

    }



    void AddFire(int size) {

        int x = (int) Mathf.Floor(Random.Range(0, size));
        int y = (int) Mathf.Floor(Random.Range(0, size));

        int timeout = 200;
        while (field[y, x] != 0 && timeout > 0) {
            x = (int) Mathf.Floor(Random.Range(0, size));
            y = (int) Mathf.Floor(Random.Range(0, size));
            timeout--;
        }

        if (timeout <= 0) return;

        field[y, x] = 5;
        List<int> xQueue = new List<int> {x};
        List<int> yQueue = new List<int> {y};

        while (xQueue.Count > 0 && yQueue.Count > 0) {

            int xCurrent = xQueue[0];
            int yCurrent = yQueue[0];

            xQueue.RemoveAt(0);
            yQueue.RemoveAt(0);
            
            if (field[yCurrent, xCurrent] == 0) field[yCurrent, xCurrent] = 3;
            if (field[yCurrent, xCurrent] == 2) field[yCurrent, xCurrent] = 4;

            if (yCurrent != 0) {
                if (field[yCurrent-1, xCurrent] == 0 || field[yCurrent-1, xCurrent] == 2) {
                    yQueue.Add(yCurrent-1);
                    xQueue.Add(xCurrent);
                }
            }
            if (yCurrent != size-1) {
                if (field[yCurrent+1, xCurrent] == 0 || field[yCurrent+1, xCurrent] == 2) {
                    yQueue.Add(yCurrent+1);
                    xQueue.Add(xCurrent);
                }
            } 
            if (xCurrent != 0) {
                if (field[yCurrent, xCurrent-1] == 0 || field[yCurrent, xCurrent-1] == 2) {
                    yQueue.Add(yCurrent);
                    xQueue.Add(xCurrent-1);
                }
            } 
            if (xCurrent != size-1) {
                if (field[yCurrent, xCurrent+1] == 0 || field[yCurrent, xCurrent+1] == 2) {
                    yQueue.Add(yCurrent);
                    xQueue.Add(xCurrent+1);
                }
            } 
        }
        
    }

    void SpawnObjects() {

        bool[,] office = new bool[size, size];
        office[size-1, size-1] = true;

        int x = 0, y = 0;
        bool failed;

        // Add Objects
        int maximum = size * 3;

        for (int i=0; i<=maximum; i++) {

            failed = true;
            while (failed) {
                x = (int) Mathf.Floor(Random.Range(2, size)); 
                y = (int) Mathf.Floor(Random.Range(2, size)); 
                failed = ((field[y, x] != 0 && field[y, x] != 3) || (office[y, x]));
            }

            office[y, x] = true;

            if (i == maximum) {
                // Add the extinguisher in the end
                Instantiate(extinguisher, new Vector3(y * tile, 0, x * tile), Quaternion.identity);
                break;
            }

            int idx = (int) Mathf.Floor(Random.Range(0, 6));
            if (idx < 3) idx = 0;
            else if (idx < 5) idx = 1;
            else idx = 2;

            GameObject newObject = Instantiate(objects[idx], new Vector3(y * tile, 0, x * tile), Quaternion.identity);

            int orientation  = (int) Mathf.Floor(Random.Range(0, 2));
            newObject.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0.0f, 90.0f * orientation, 0.0f);

        }


    }


    void CreateField() {

        for (int i=0; i<size; i++) {
            for (int j=0; j<size; j++) {

                // Floor, Ceiling

                Instantiate(floor, new Vector3(i * tile, 0, j * tile), Quaternion.identity);

                Instantiate(ceiling, new Vector3(i * tile, 0, j * tile), Quaternion.identity);

                // Lights

                if (i % 2 == 0 && j % 4 == 0) {
                    Instantiate(light, new Vector3(i * tile, 0, j * tile), Quaternion.identity);
                }
                
                if (i % 2 == 1 && j % 4 == 2) {
                    Instantiate(light, new Vector3(i * tile, 0, j * tile), Quaternion.identity);
                }

                // Walls

                if (i == 0 || (field[i-1, j] == 1 && (field[i, j] == 0 || field[i, j] == 3 || field[i, j] == 5))) {
                    Instantiate(wall, new Vector3(i * tile - tile + 0.1f, 0, j * tile), Quaternion.identity);
                }

                if (i == size-1 || (field[i+1, j] == 1 && (field[i, j] == 0 || field[i, j] == 3 || field[i, j] == 5))) {
                    Instantiate(wall, new Vector3(i * tile, 0, j * tile), Quaternion.identity);
                }

                if (j == 0 || (field[i, j-1] == 1 && (field[i, j] == 0 || field[i, j] == 3 || field[i, j] == 5))) {
                    Instantiate(wall, new Vector3(i * tile - tile, 0, j * tile - tile + 0.1f), Quaternion.Euler(0, -90.0f, 0));
                }

                if (j == size-1 || (field[i, j+1] == 1 && (field[i, j] == 0 || field[i, j] == 3 || field[i, j] == 5))) {
                    if (j == size-1 && i == size-1) {
                        Instantiate(frame, new Vector3(i * tile, 0, j * tile - 0.1f), Quaternion.Euler(0, 90.0f, 0)); 
                        Instantiate(exit, new Vector3(i * tile, 0, j * tile - 0.1f), Quaternion.Euler(0, 90.0f, 0));
                    }
                    else Instantiate(wall, new Vector3(i * tile, 0, j * tile - 0.1f), Quaternion.Euler(0, 90.0f, 0));
                }

                // Frames

                if (i != 0 && (field[i-1, j] == 1 && (field[i, j] == 2 || field[i, j] == 4))) {
                    Instantiate(frame, new Vector3(i * tile - tile + 0.1f, 0, j * tile), Quaternion.identity);
                    newDoor = Instantiate(door, new Vector3(i * tile - tile + 0.1f, 0, j * tile), Quaternion.identity);
                    if (field[i, j] == 4) {
                        newDoor.transform.GetChild(0).GetComponent<Door>().SetHot();
                    }
                }

                if (i != size-1 && (field[i+1, j] == 1 && (field[i, j] == 2 || field[i, j] == 4))) {
                    Instantiate(frame, new Vector3(i * tile, 0, j * tile), Quaternion.identity);
                    newDoor = Instantiate(door, new Vector3(i * tile, 0, j * tile), Quaternion.identity);
                    if (field[i, j] == 4) {
                        newDoor.transform.GetChild(0).GetComponent<Door>().SetHot();
                    }
                }

                if (j != 0 && (field[i, j-1] == 1 && (field[i, j] == 2 || field[i, j] == 4))) {
                    Instantiate(frame, new Vector3(i * tile - tile, 0, j * tile - tile + 0.1f), Quaternion.Euler(0, -90.0f, 0));
                    newDoor = Instantiate(door, new Vector3(i * tile - tile, 0, j * tile - tile + 0.1f), Quaternion.Euler(0, -90.0f, 0));
                    if (field[i, j] == 4) {
                        newDoor.transform.GetChild(0).GetComponent<Door>().SetHot();
                    }
                }

                if (j != size-1 && (field[i, j+1] == 1 && (field[i, j] == 2 || field[i, j] == 4))) {
                    Instantiate(frame, new Vector3(i * tile, 0, j * tile - 0.1f), Quaternion.Euler(0, 90.0f, 0)); 
                    newDoor = Instantiate(door, new Vector3(i * tile, 0, j * tile - 0.1f), Quaternion.Euler(0, 90.0f, 0));
                    if (field[i, j] == 4) {
                        newDoor.transform.GetChild(0).GetComponent<Door>().SetHot();
                    }
                }


                // Fire

                if (field[i, j] == 5) {
                    Instantiate(fire, new Vector3(i * tile, 0, j * tile), Quaternion.identity);
                }

            }
        }
    }



}
