using UnityEngine;

[CreateAssetMenu(fileName = "TestScriptableObject", menuName = "ScriptableObjects/test")]
public class test : ScriptableObject
{
    public GameObject[] gunBase;
    public GameObject[] magazine;
    public GameObject[] grip;
}
