using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[ExecuteInEditMode]
public class FbioImageProTool : MonoBehaviour
{
    // Start is called before the first frame update
    public Image image;
    public Image ImageToReadFrom;


    void Update()
    {
        //get most used color in image
        Sprite s = ImageToReadFrom.sprite;
        Color32[] pixels = s.texture.GetPixels32();
        int total = pixels.Length;
        float r = 0;
        float g = 0;
        float b = 0;
        for (int i = 0; i < total; i++)
        {
            r += pixels[i].r;
            g += pixels[i].g;
            b += pixels[i].b;
        }
        r /= total;
        g /= total;
        b /= total;

        //convert r to 100%
        r = r / 255;
        g = g / 255;
        b = b / 255;

        //change image color 
        image.color = new Color(r, g, b,1f);

        


    }
}
