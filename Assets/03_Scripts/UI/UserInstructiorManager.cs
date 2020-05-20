using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton class responsible for sending messages to the user
/// </summary>
public class UserInstructiorManager : MonoBehaviour
{
    public Text InstructionText;
    public string DefaultTextValue = "Check patient info and medical history";

    public float textFlashLength = 5.0f; // How long the text will flash when a new one is set

    private void Start()
    {
        SetInstructionText(DefaultTextValue);
    }

    public void SetInstructionText(string _text)
    {
        InstructionText.text = _text;
        StartCoroutine(FlashText(InstructionText));
    }

    private IEnumerator FlashText(Text textelement)
    {
        float t = 0;
        float alpha = 0; 

        while (t < textFlashLength)
        {
            t += Time.deltaTime;
            alpha += Time.deltaTime /2f;
            if(alpha > .5f )
            {
                alpha = 0;
            }
            textelement.color = new Color(textelement.color.r, textelement.color.g, textelement.color.b,alpha);
            yield return null;
        }
        textelement.color = new Color(textelement.color.r, textelement.color.g, textelement.color.b, 1);
    }

    #region singleton
    public static UserInstructiorManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogError("UserInstructiorManager already exist destroying script attatched too : " + transform.name);
        }

    }
    #endregion
}
