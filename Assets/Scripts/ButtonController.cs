using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public float target = 0;
    public float speed = 1;
    public void set(int movement, int yearCount)
    {
        float buttonWidth = 1.425f;
        target =  buttonWidth * movement - (yearCount * buttonWidth / 2) + 0.7125f;
    }

    void Update()
    {
        if(Mathf.Abs(target - transform.position.x) > speed * Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime * Mathf.Sign(target - transform.position.x), transform.position.y);
        } else
        {
            transform.position = new Vector3(target, transform.position.y);
        }
    }
}
