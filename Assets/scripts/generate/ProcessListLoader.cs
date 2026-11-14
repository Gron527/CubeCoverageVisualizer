using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ProcessListLoader : MonoBehaviour
{
    public GameObject itemTemplate;
    public Transform contentParent;
    public Button refreshButton;
    [System.Serializable]
    public class ProcessData
    {
        public int n;
        public int population_size;
        public int generations;
        public int mutation_rate;
        public int accuracy;
        public int reach;
        public int fitness_mode;
    }

    

    void Start()
    {
        refreshButton.onClick.AddListener(RefreshList);
    }

    public IEnumerator LoadProcesses()
    {
        UnityWebRequest req = UnityWebRequest.Get("http://hauerszabolcs.hu:8100/processes");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            ToastManager.Instance.Show("Hiba történt!", true);
            yield break;
        }

        string json = req.downloadHandler.text;

        ProcessData[] processes = JsonHelper.FromJson<ProcessData>(json);

       // régi elemek kitörlése
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

     // újak létrehozása
        for (int i = 0; i < processes.Length; i++)
        {
            int index = i;
            var p = processes[i];

            GameObject item = Instantiate(itemTemplate, contentParent);
            item.SetActive(true);

            item.transform.Find("ProcessName")
     .GetComponent<TMP_Text>()
     .text = $"méret={p.n}, populáció={p.population_size}, generációk={p.generations}";

            item.transform.Find("ActivateButton")
                .GetComponent<Button>()
                .onClick.AddListener(() => ActivateProcess(index));

            item.transform.Find("TerminateButton")
                .GetComponent<Button>()
                .onClick.AddListener(() => TerminateProcess(index));
        }
    }

    void ActivateProcess(int index)
    {
        StartCoroutine(CallSimplePost($"http://hauerszabolcs.hu:8100/change_active_process/{index}",
            "Aktív folyamat beállítva"));
    }

    void TerminateProcess(int index)
    {
        StartCoroutine(CallSimplePost($"http://hauerszabolcs.hu:8100/terminate_process/{index}",
            "Folyamat leállítva"));
    }
    public void RefreshList()
    {
        StartCoroutine(LoadProcesses());
        
    }
    IEnumerator CallSimplePost(string url, string successMessage)
    {
        UnityWebRequest req = UnityWebRequest.PostWwwForm(url, "");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            ToastManager.Instance.Show(successMessage, false);
            StartCoroutine(LoadProcesses());
        }
        else
        {
            ToastManager.Instance.Show("Hiba történt!", true);
        }
    }
}