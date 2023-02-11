using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//music reference
//https://pixabay.com/music/search/rpg%20game/?manual_search=1&order=None

public class menu : MonoBehaviour
{
    
    public TMP_InputField width;
    public TMP_InputField height;
    public TMP_InputField loops;
    public TMP_InputField delay;

    public GameObject info;

    public void displayInfo()
    {
        info.SetActive(!info.active);
    }


    public void assign()
    {
        int w = convertToInt( width.text);
        int h = convertToInt(height.text);
        int l = convertToInt( loops.text);
        float d = convertToFloat(delay.text);

        if (w < 50) w = 50;
        if (w > 1000) w = 1000;
        h = w;
        if(l < 10) l = 10;
        if(d < 0f) d = 0f;

        GameData.tileWidth = w;
        GameData.tileHeight = h;
        GameData.loopTimes = l;
        GameData.delay = d;

        SceneManager.LoadScene("GameScene");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private int convertToInt(string value)
    {
        int i = -1;
        try
        {
        i = Int32.Parse(value);
        } catch(Exception e)
        {
            i = -1;
        }


        return i;
    }
    private float convertToFloat(string value)
    {
        float i = -1f;
        try
        {
            i = float.Parse(value);
        }
        catch (Exception e)
        {
            i = -1f;
        }


        return i;
    }


}

