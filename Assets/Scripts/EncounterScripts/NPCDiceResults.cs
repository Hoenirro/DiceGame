using UnityEngine;
using System.Collections;

public class NPCDiceResults : MonoBehaviour
{
    [SerializeField] private Sprite[] diceFaceSprites;
    [SerializeField] private GameObject diceSlotPrefab;

    public void SpawnDiceResults(int[] diceResults)
    {
        ClearDiceResults();
        StartCoroutine(TimeDelay(0.2f));
        foreach (var result in diceResults)
        {
            StartCoroutine(SpawnDiceWithDelay(result));
            StartCoroutine(TimeDelay(0.1f));
        }
    }

    private IEnumerator TimeDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private IEnumerator SpawnDiceWithDelay(int result)
    {
        yield return new WaitForSeconds(0.2f);
        GameObject diceSlot = Instantiate(diceSlotPrefab, transform);
        SpriteRenderer spriteRenderer = diceSlot.GetComponent<SpriteRenderer>();
        if (result >= 1 && result <= diceFaceSprites.Length)
        {
            spriteRenderer.sprite = diceFaceSprites[result - 1];
        }
        else
        {
            Debug.LogWarning("Dice result out of range: " + result);
        }
    }

    public void ClearDiceResults()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
