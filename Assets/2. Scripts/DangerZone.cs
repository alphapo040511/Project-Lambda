using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [Header("즉시 사망 여부")]
    public bool isInstantDeath;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"플레이어가 아님 [name : {other.gameObject.name}, tag : {other.gameObject.tag}]");
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();

        if(player != null)
        {
            if(isInstantDeath)
                player.SetState(new DeathState(player));
            else
                player.SetState(new ExposureState(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"플레이어가 아님 {other.gameObject.name}");
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            if (player.currentState is not DeathState)      // 사망 상태가 아닌 경우만 탈출
            {
                player.SetState(new IdleState(player));
            }
        }
    }
}
