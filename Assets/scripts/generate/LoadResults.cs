using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LoadResults : MonoBehaviour
{
    [Header("UI Input Fields")]
    public TMP_InputField fromInput;
    public TMP_InputField toInput;

    [Header("List Template")]
    public GameObject itemTemplate;
    public Transform contentParent;

  [System.Serializable]
public class CubeData
{
    public float size;
    public float x;
    public float y;
    public float z;
    public float x_center;
    public float y_center;
    public float z_center;
}

[System.Serializable]
public class ResultData
{
    public float result;
    public int n;
    public int accuracy;
    public CubeData[] cubes;
}

    public void LoadRange()
    {
        
        int from = int.Parse(fromInput.text);
        int to = int.Parse(toInput.text);

        StartCoroutine(LoadResultsRange(from, to));
    }

    IEnumerator LoadResultsRange(int from, int to)
    {
        // töröljük a régi elemeket
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        for (int i = from; i <= to; i++)
        {
            UnityWebRequest req = UnityWebRequest.Get($"http://hauerszabolcs.hu:8100/results/{i}");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string json = req.downloadHandler.text;

                if (string.IsNullOrWhiteSpace(json) || json == "null")
                {
                    Debug.LogWarning($"Empty json for ID={i}");
                    continue;
                }

                ResultData result = null;
                try
                {
                    result = JsonUtility.FromJson<ResultData>(json);
                }
                catch
                {
                    Debug.LogWarning($"Invalid JSON for ID={i}: {json}");
                    continue;
                }

                if (result == null)
                {
                    Debug.LogWarning($"Parsed result is NULL for ID={i}");
                    continue;
                }
                // extra validáció
                if (result.cubes == null || result.cubes.Length == 0)
                {
                    Debug.LogWarning($"Skipping ID={i}, cubes is empty");
                    continue;
                }

                // UI elem létrehozás
                GameObject item = Instantiate(itemTemplate, contentParent);
                item.SetActive(true);

                int cubesCount = (result.cubes != null) ? result.cubes.Length : 0;

                item.transform.Find("ResultName")
                    .GetComponent<TMP_Text>().text =
                    $"méret={result.n}, pontosság={result.accuracy}, eredmény={result.result}";

                int capturedID = i;

                item.transform.Find("LoadButton")
                    .GetComponent<Button>()
                    .onClick.AddListener(() => LoadResult(capturedID));
            }
            else
            {
                Debug.LogWarning($"No result for ID={i}");
            }

            yield return null;
        }
    }

    void LoadResult(int id)
    { 
        StartCoroutine(LoadAndShow(id));
    }
    IEnumerator LoadAndShow(int id)
    {
        UnityWebRequest req = UnityWebRequest.Get($"http://hauerszabolcs.hu:8100/results/{id}");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Hiba a betöltésnél! URL: /results/" + id);
            yield break;
        }

        string json = req.downloadHandler.text;

        if (string.IsNullOrWhiteSpace(json) || json == "null" || json.StartsWith("<"))
        {
            Debug.LogError($"Invalid or empty JSON for ID={id}: {json}");
            yield break;
        }

        Debug.Log("Megoldás JSON betöltve, továbbítás a LoadCubesnak");

        var loader = FindFirstObjectByType<LoadCubes>();

        if (loader != null)
            loader.LoadSolutionFromJson(json);
        else
            Debug.LogError("Nincs LoadCubesFromFile a jelenetben!");
    }
}
