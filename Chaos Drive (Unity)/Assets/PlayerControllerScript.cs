using UnityEngine;
using System.Collections;

public class PlayerControllerScript : MonoBehaviour {

    int lives = 3;
    GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (lives > 0 && player == null)
        {
            player = Instantiate(Resources.Load(@"Player"), new Vector3(0, 0, 0), new Quaternion()) as GameObject;
            lives--;
        }
	}
}
