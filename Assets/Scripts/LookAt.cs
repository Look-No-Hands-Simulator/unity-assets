using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
    }

    // Update is called once per frame

    void Update()
    {
        if(player.transform.childCount > 0)
        {
            target = player.transform.GetChild(0);
            transform.LookAt(target);
        }

    }

}
