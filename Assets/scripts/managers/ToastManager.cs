using System.Collections;
using TMPro;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    public GameObject toastPanel;
    public TMP_Text toastText;

    private void Awake()
    {
        Instance = this;   
    }

    public void Show(string message, bool isError)
    {
        StopAllCoroutines();
        StartCoroutine(ShowToast(message, isError));
    }

    private IEnumerator ShowToast(string message, bool isError)
    {
        toastPanel.SetActive(true);
        toastText.text = message;
        toastText.color = isError ? Color.red : Color.green;

        yield return new WaitForSeconds(2f);

        toastPanel.SetActive(false);
    }
}