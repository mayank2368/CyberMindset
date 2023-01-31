using UnityEngine;
using System.Collections.Generic;
 
/// <summary>
/// The script is responsible for settling the Camera resolution to scale and fit any screen
/// </summary>
public class ForceAspectRatio : MonoBehaviour
{
    private int ScreenSizeX = 0;
    private int ScreenSizeY = 0;
    public float WidthRatio = 16.0f, HeightRatio = 9.0f;
 
    private void RescaleCamera()
    {
 
        if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;
 
        float targetaspect = WidthRatio / HeightRatio;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        Camera camera = GetComponent<Camera>();
 
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;
 
            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
 
             camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;
 
            Rect rect = camera.rect;
 
            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;
 
             camera.rect = rect;
        }
 
        ScreenSizeX = Screen.width;
        ScreenSizeY = Screen.height;
    }
 
    void OnPreCull()
    {
        // if (Application.isEditor) return;
        Rect wp = Camera.main.rect;
        Rect nr = new Rect(0, 0, 1, 1);
 
        Camera.main.rect = nr;
        GL.Clear(true, true, Color.black);
       
        Camera.main.rect = wp;
 
    }
 
    // Use this for initialization
    void Start () {
        RescaleCamera();
    }
   
    // void Update () {
    //     RescaleCamera();
    // }
}