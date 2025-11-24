using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPuzzleController : InteractionFocus
{
    [Header("Present Settings")]
    public float presentDelay = 1f;
    private float timer = 0;

    [Header("References")]
    public List<MemotyPuzzleMonitor> monitors = new List<MemotyPuzzleMonitor>();
    public List<Sprite> icons = new List<Sprite>();

    protected override void Update()
    {
        base.Update();

        if (isFocused == false)            // 포커싱 되지 않았다면
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = presentDelay;
                SetRandomIcons();
            }
        }

    }

    void SetRandomIcons()
    {
        if (monitors.Count != 9 || icons.Count != 9) return;        // 9개인 경우만 적용

        List<int> rand = GetRandomIndexList();
        for (int i = 0; i < 9; i++)
        {
            monitors[i].SetIcon(icons[rand[i]]);
            monitors[i].SetColor(Color.white);
        }
    }

    public bool CanInteractMonitor()
    {
        return true;        // 나중에 인터렉션 조건 추가
    }

    List<int> GetRandomIndexList()
    {
        List<int> reference = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        List<int> rand = new List<int>();
        while(reference.Count > 0)
        {
            int i = reference[Random.Range(0, reference.Count)];
            rand.Add(i);
            reference.Remove(i);
        }

        return rand;
    }
}
