using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Animator Movement;
    public void set1()
    {
        Movement.SetInteger("Button_Movement", 1);
    }

    public void set2()
    {
        Movement.SetInteger("Button_Movement", 2);
    }

    public void set3()
    {
        Movement.SetInteger("Button_Movement", 3);
    }

    public void set4()
    {
        Movement.SetInteger("Button_Movement", 4);
    }

    public void set5()
    {
        Movement.SetInteger("Button_Movement", 5);
    }
}
