using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AddGeneticProcess : MonoBehaviour
{
    public TMP_InputField nField;
    public TMP_InputField populationField;
    public TMP_InputField generationsField;
    public TMP_InputField mutationField;
    public TMP_InputField accuracyField;
    public TMP_InputField reachField;
    public TMP_InputField fitnessField;
    public TMP_InputField priorityField;
    public Toggle startImmediatelyToggle;

    public void SendProcess()
    {
        StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest()
    {
        // input validáció
        if (!CheckAllFields())
        {
            ToastManager.Instance.Show("Minden mezõt tölts ki helyesen!", true);
            yield break;
        }

        
        GeneticProcessData data = new GeneticProcessData();
        data.n = int.Parse(nField.text);
        data.population_size = int.Parse(populationField.text);
        data.generations = int.Parse(generationsField.text);
        float m;
        float.TryParse(mutationField.text.Replace(',', '.'),
                       NumberStyles.Float,
                       CultureInfo.InvariantCulture,
                       out m);
        data.mutation_rate = m;
        data.accuracy = int.Parse(accuracyField.text);
        data.reach = int.Parse(reachField.text);
        data.fitness_mode = int.Parse(fitnessField.text);
        data.priority = int.Parse(priorityField.text);
        data.start_immediately = startImmediatelyToggle.isOn;

        string json = JsonUtility.ToJson(data);
        Debug.Log("Sending JSON: " + json);

        
        UnityWebRequest req = new UnityWebRequest("http://hauerszabolcs.hu:8100/add_genetic_process", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            ToastManager.Instance.Show("Hiba történt: " + req.error, true);
            Debug.LogError(req.error);
        }
        else
        {
            ToastManager.Instance.Show("Sikeres létrehozás!", false);
            Debug.Log("Success: " + req.downloadHandler.text);
        }
    }

   

   
    bool IsValidMutationRate()
    {
        string t = mutationField.text.Replace(',', '.');
        float tmp;
        return float.TryParse( t,NumberStyles.Float,CultureInfo.InvariantCulture,out tmp)
            && tmp >= 0f && tmp <= 1f;
    }
    bool IsFitnessModeValid()
    {
        if (!IsInt(fitnessField))
            return false;

        int v = int.Parse(fitnessField.text);
        return v == 1 || v == 2;
    }
    bool CheckAllFields()
    {
        return
            IsInt(nField) &&
            IsInt(populationField) &&
            IsInt(generationsField) &&
            IsValidMutationRate() &&
            IsInt(accuracyField) &&
            IsInt(reachField) &&
            IsFitnessModeValid() &&
            IsInt(priorityField);
    }

    bool IsInt(TMP_InputField f)
    {
        return !string.IsNullOrEmpty(f.text) && int.TryParse(f.text, out _);
    }
}