using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    #region Variables
    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    float horzSpeed = 5;
    float vertSpeed = 4;
    #endregion


    #region Methods
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var moveVector = new Vector2(0, 0);

        if (Input.GetKey(moveUp))
        {
            moveVector.y = vertSpeed;
        }
        else if (Input.GetKey(moveDown))
        {
            moveVector.y = -vertSpeed;
        }

        if (Input.GetKey(moveLeft))
        {
            moveVector.x = -horzSpeed;
        }
        else if (Input.GetKey(moveRight))
        {
            moveVector.x = horzSpeed;
        }

        rigidbody2D.velocity = moveVector;
    }
    #endregion
}
