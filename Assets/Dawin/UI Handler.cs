using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance;

    public RectTransform[] elementsUITransform;
    public Image[] elementsUI;
    
    private Color tempColor;

    #region Positional

    public RectTransform topPos = new RectTransform();
    public Image topImage;

    public RectTransform bottomPos;
    public Image bottomImage;

    public RectTransform leftPos;
    public Image leftImage;

    public RectTransform rightPos;
    public Image rightImage;

    #endregion


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        topPos = elementsUITransform[0];
        rightPos = elementsUITransform[1];
        bottomPos = elementsUITransform[2];
        leftPos = elementsUITransform[3];

        topImage = elementsUI[0];
        rightImage = elementsUI[1];
        bottomImage = elementsUI[2];
        leftImage = elementsUI[3];
    }

    public void lockElement(int index)
    {
        tempColor = elementsUI[index].color;
        elementsUI[index].GetComponent<Image>().color = new Color(0.3f,0.3f,0.3f,1f);
        Debug.Log("Locking element " + elementsUI[index].GetComponent<Image>().color);
    }

    public void UnlockElement(int index)
    {
        elementsUI[index].GetComponent<Image>().color = new Color(1,1,1,1);
    }
    
    public void RotateElement(int dir)
    {
        StartCoroutine(RotateUI(dir));
    }

    private IEnumerator RotateUI(int dir)
    {
        var pos1 = topPos.position;
        var pos2 = rightPos.position;
        var pos3 = bottomPos.position;
        var pos4 = leftPos.position;

        var sca1 = topPos.localScale;
        var sca2 = rightPos.localScale;
        var sca3 = bottomPos.localScale;
        var sca4 = leftPos.localScale;

        if (dir == 1)
        {
            float counter = 0.0f;
            float duration = 0.1f;
            var col1 = elementsUI[0].color;
            col1 = new Color(col1.r, col1.g, col1.b, rightImage.color.a);
            var col2 = elementsUI[1].color;
            col2 = new Color(col2.r, col2.g, col2.b, bottomImage.color.a);
            var col3 = elementsUI[2].color;
            col3 = new Color(col3.r, col3.g, col3.b, leftImage.color.a);
            var col4 = elementsUI[3].color;
            col4 = new Color(col4.r, col4.g, col4.b, topImage.color.a);
            while (duration > counter)
            {
                counter += Time.deltaTime;
                //opacity
                var lerpTimer = counter / duration;
                elementsUI[0].GetComponent<Image>().color = Color.Lerp(elementsUI[0].GetComponent<Image>().color,
                    col1, lerpTimer);
                elementsUI[1].GetComponent<Image>().color = Color.Lerp(elementsUI[1].GetComponent<Image>().color,
                    col2, lerpTimer);
                elementsUI[2].GetComponent<Image>().color = Color.Lerp(elementsUI[2].GetComponent<Image>().color,
                    col3, lerpTimer);
                elementsUI[3].GetComponent<Image>().color = Color.Lerp(elementsUI[3].GetComponent<Image>().color,
                    col4, lerpTimer);

                //position
                elementsUITransform[0].position = Vector3.Lerp(elementsUITransform[0].position, pos2,
                    lerpTimer);
                elementsUITransform[1].position = Vector3.Lerp(elementsUITransform[1].position, pos3,
                    lerpTimer);
                elementsUITransform[2].position = Vector3.Lerp(elementsUITransform[2].position, pos4,
                    lerpTimer);
                elementsUITransform[3].position = Vector3.Lerp(elementsUITransform[3].position, pos1,
                    lerpTimer);

                elementsUITransform[0].localScale = Vector3.Lerp(elementsUITransform[0].localScale, sca2, lerpTimer);
                elementsUITransform[1].localScale = Vector3.Lerp(elementsUITransform[1].localScale, sca3, lerpTimer);
                elementsUITransform[2].localScale = Vector3.Lerp(elementsUITransform[2].localScale, sca4, lerpTimer);
                elementsUITransform[3].localScale = Vector3.Lerp(elementsUITransform[3].localScale, sca1, lerpTimer);

                yield return null;
            }
        }
        else
        {
            var col1 = elementsUI[0].color;
            col1 = new Color(col1.r, col1.g, col1.b, leftImage.color.a);
            var col2 = elementsUI[1].color;
            col2 = new Color(col2.r, col2.g, col2.b, topImage.color.a);
            var col3 = elementsUI[2].color;
            col3 = new Color(col3.r, col3.g, col3.b, rightImage.color.a);
            var col4 = elementsUI[3].color;
            col4 = new Color(col4.r, col4.g, col4.b, bottomImage.color.a);


            float counter = 0.0f;
            float duration = 0.1f;
            while (duration > counter)
            {
                counter += Time.deltaTime;
                //opacity
                var lerpTimer = counter / duration;

                elementsUI[0].GetComponent<Image>().color = Color.Lerp(elementsUI[0].GetComponent<Image>().color,
                    col1, lerpTimer);
                elementsUI[1].GetComponent<Image>().color = Color.Lerp(elementsUI[1].GetComponent<Image>().color,
                    col2, lerpTimer);
                elementsUI[2].GetComponent<Image>().color = Color.Lerp(elementsUI[2].GetComponent<Image>().color,
                    col3, lerpTimer);
                elementsUI[3].GetComponent<Image>().color = Color.Lerp(elementsUI[3].GetComponent<Image>().color,
                    col4, lerpTimer);

                //position
                elementsUITransform[0].position = Vector3.Lerp(elementsUITransform[0].position, pos4, lerpTimer);
                elementsUITransform[1].position = Vector3.Lerp(elementsUITransform[1].position, pos1, lerpTimer);
                elementsUITransform[2].position = Vector3.Lerp(elementsUITransform[2].position, pos2, lerpTimer);
                elementsUITransform[3].position = Vector3.Lerp(elementsUITransform[3].position, pos3, lerpTimer);

                elementsUITransform[0].localScale =
                    Vector3.Lerp(elementsUITransform[0].localScale, sca4, lerpTimer);
                elementsUITransform[1].localScale =
                    Vector3.Lerp(elementsUITransform[1].localScale, sca1, lerpTimer);
                elementsUITransform[2].localScale =
                    Vector3.Lerp(elementsUITransform[2].localScale, sca2, lerpTimer);
                elementsUITransform[3].localScale =
                    Vector3.Lerp(elementsUITransform[3].localScale, sca3, lerpTimer);


                yield return null;
            }
        }
    }
    
    
}