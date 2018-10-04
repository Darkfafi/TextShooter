using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordUIDisplayItem : MonoBehaviour
{
    [SerializeField]
    private Text _wordTextDisplay;
	

    public void SetText(string text)
    {
        _wordTextDisplay.text = text;
    }
}
