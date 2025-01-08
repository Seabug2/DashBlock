using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : Singleton
{
    TextMeshPro tmp;
    TextMeshPro TMP => tmp ??= GetComponent<TextMeshPro>();
}
