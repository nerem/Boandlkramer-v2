using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBloodDecals : MonoBehaviour {

    [SerializeField]
    GameObject[] bloodDecals;


    public void SpawnRandomBloodDecal(Vector3 position)
    {
        Vector3 pos = new Vector3(position.x, position.y + .9f, position.z);
        int index = Random.Range(0, bloodDecals.Length);
        Debug.Log("Spawning Blood decal with index " + index.ToString());

        GameObject go = Instantiate(bloodDecals[index], pos, Quaternion.identity);
        go.transform.parent = this.transform;
        go.transform.Rotate(Vector3.right, 90f);

        // apply random rotation
        go.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));
    }
}
