using UnityEngine;

[CreateAssetMenu(fileName = "MessageScreenSO", menuName = "Scriptable Objects/D6SO")]
public class D6SO : ScriptableObject
{
    [SerializeField] private Sprite[] faceSprites;
    public string diceName;
    public int[] faceValues = new int[6];
    
}
