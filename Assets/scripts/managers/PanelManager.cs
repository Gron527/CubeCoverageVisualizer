using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject loadNewPanel;
    public GameObject loadResultsPanel;
    public GameObject processesPanel;

    public bool isPanelOpen = true;

    void Start()
    {
        ShowMainPanel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ESC = mindig visszalépünk a main panelre
            if (isPanelOpen && mainPanel.activeSelf == false)
            {
                ShowMainPanel();
            }
            else
            {
                ToggleMainPanel();
            }
        }
    }

    public void ToggleMainPanel()
    {
        if (isPanelOpen)
            HideMainPanel();
        else
            ShowMainPanel();
    }

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        loadNewPanel.SetActive(false);
        loadResultsPanel.SetActive(false);
        processesPanel.SetActive(false);

        isPanelOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideMainPanel()
    {
        mainPanel.SetActive(false);
        loadNewPanel.SetActive(false);
        loadResultsPanel.SetActive(false);
        processesPanel.SetActive(false);

        isPanelOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- BUTTON FUNCTIONS ---
    public void OpenLoadNewPanel()
    {
        mainPanel.SetActive(false);
        loadNewPanel.SetActive(true);
        loadResultsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void OpenLoadResultsPanel()
    {
        mainPanel.SetActive(false);
        loadNewPanel.SetActive(false);
        loadResultsPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OpenProcessesPanel()
    {
        mainPanel.SetActive(false);
        loadNewPanel.SetActive(false);
        loadResultsPanel.SetActive(false);
        processesPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
        processesPanel.GetComponentInChildren<ProcessListLoader>().RefreshList();
    } 

    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}