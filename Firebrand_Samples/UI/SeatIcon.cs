using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeatIcon : MonoBehaviour
{

    public GameObject icon;
    public Image Colour1Img;
    public Image Colour2Img;

    public void UpdateColours(Color Colour1, Color Colour2, bool isVisible)
    {

        if (isVisible)
        {
            icon.SetActive(true);
            Colour1Img.color = Colour1;
            Colour2Img.color = Colour2;
        } else
        {
            icon.SetActive(false);
        }

    }
}
