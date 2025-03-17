using NUnit.Framework;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InfinityLoadPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _loadingTextField;
    [SerializeField] GameObject _loadingImageObj;

    
    private async void Start()
    {
        StartCoroutine(UpdateLoadingTextField());
    }

    IEnumerator UpdateLoadingTextField()
    {
        float timer = 0f;
        string[] loadingDots = { "", ".", ". .", ". . ." };
        int pointer = 0;
        while(true)
        {
            _loadingTextField.text = $"{TextProvider.Loading} {loadingDots[pointer]}";
            yield return new WaitForSeconds(1f);
            ++pointer;
            timer += Time.deltaTime;
            if (pointer >= loadingDots.Length)
                pointer = 0;
        }

    }

}
