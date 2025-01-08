using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockPrefabsList : ScriptableObject
{
    public Block[] blocks;

    public Block this[int index]
    {
        get
        {
            if (index < 1 || index > blocks.Length)
            {
                return blocks[0];
            }

            return blocks[index];
        }
    }
}
