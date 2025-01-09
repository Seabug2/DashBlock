using UnityEngine;

[CreateAssetMenu]
public class BlockPrefabsList : ScriptableObject
{
    public GameObject[] blocks;

    public int Length => blocks.Length;

    public GameObject this[int index]
    {
        get
        {
            if (index < 1 || index >= blocks.Length)
            {
                return blocks[0];
            }

            return blocks[index];
        }
    }
}
