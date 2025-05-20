using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AddonManager : MonoBehaviour
{
    void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);
    }
}
