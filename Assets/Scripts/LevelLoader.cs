using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] List<Object> Stages;

    // Stages
    GameObject leftSect;
    GameObject midSect;
    GameObject rightSect;
    List<GameObject> loadedLevels = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        leftSect = (GameObject) Instantiate(Stages[0], new Vector3(-50f, 0f, 0f), Quaternion.identity, gameObject.transform);
        midSect = (GameObject) Instantiate(Stages[0], new Vector3(0f, 0f, 0f), Quaternion.identity, gameObject.transform);
        rightSect = (GameObject) Instantiate(Stages[Random.Range(0, Stages.Count)], new Vector3(50f, 0f, 0f), Quaternion.identity, gameObject.transform);
        loadedLevels.Add(leftSect);
        loadedLevels.Add(midSect);
        loadedLevels.Add(rightSect);
    }

    // Update is called once per frame
    void Update()
    {
        /*
         When LevelBlocks reaches x = -50, 
        - deconstruct leftSect, 
        - set midSect to leftSect, 
        - set rightSect to midSect ,
        - instantiate rightSect,
        - set position 
        O(undefined) complexity; probably bad but looks nice. Sorry Carey.
         */
        if (transform.position.x <= -50f)
        {
            Destroy(loadedLevels[0]);
            loadedLevels.RemoveAt(0);
            Debug.Log(Random.Range(0, loadedLevels.Count));
            loadedLevels.Add((GameObject) Instantiate(Stages[Random.Range(0, Stages.Count)], gameObject.transform, false));
            RearrangeStage();
        }
        if (transform.position.x >= 50f)
        {
            Destroy(loadedLevels[loadedLevels.Count - 1]);
            loadedLevels.RemoveAt(loadedLevels.Count - 1);
            loadedLevels.Insert(0, (GameObject)Instantiate(Stages[Random.Range(0, Stages.Count)], gameObject.transform, false));
            RearrangeStage();
        }
    }

    void RearrangeStage()
    {
        //leftSect.transform.position = new Vector3(-50f, 0f, 0f);
        //midSect.transform.position = new Vector3(0f, 0f, 0f);
        //rightSect.transform.position = new Vector3(50f, 0f, 0f);
        int index = -1;
        transform.position = new Vector3(0f, 0f, transform.position.z);
        foreach (GameObject obj in loadedLevels)  // Should only have 3 items max at any time
        {
            obj.transform.position = new Vector3(50f * index, 0f, 0f);
            index++;
        }
        
    }
} 
