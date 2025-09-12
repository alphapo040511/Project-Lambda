using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramController : MonoBehaviour
{
    [Header("Shader Settings")]
    public bool enableVertical = false;

    private string shaderPath = "Shaders/HologramShader";
    private Shader holoShader;
    private Material holoMaterial;


    void Start()
    {
        // 1. 쉐이더 로드 후 머티리얼 생성
        Shader holoShader = Resources.Load<Shader>(shaderPath);
        holoMaterial = new Material(holoShader);

        // 2. 모든 렌더러에 적용
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            ApplyHologram(rend);
        }
    }

    void ApplyHologram(Renderer rend)
    {
        // 3. 원래 텍스처 가져오기
        var originalTex = rend.sharedMaterial.GetTexture("_MainTex");

        if (originalTex == null) return;

        // 4. MaterialPropertyBlock으로 파라미터 설정
        var block = new MaterialPropertyBlock();                                    // MaterialPropertyBlock은 머티리얼 값만 오브젝트 단위로 덮어씌우는 방법
        block.SetTexture("_OriginTexture", originalTex);

        // 오브젝트 크기에 따라 파라미터 조절
        var size = rend.bounds.size;

        float value = (size.magnitude / 1.732f);                                    // 1,1,1 사이즈를 기준으로 계산

        float scrollSpeed = 0.1f / value;
        scrollSpeed = Mathf.Min(0.1f, scrollSpeed);                                 // 최대 0.1로 설정

        float splitSpeed = 100 * value;
        splitSpeed = Mathf.Max(100, splitSpeed);                                    // 최소 100으로 설정

        // 파라미터 적용
        holoMaterial.SetFloat("_ScrollSpeed", scrollSpeed);
        holoMaterial.SetFloat("_Thickness", 10);
        holoMaterial.SetFloat("_SplitSpeed", splitSpeed);

        holoMaterial.SetFloat("_Rotation", enableVertical ? 1f : 0f);                      // 0 = 회전 X, 1 = 90도 회전

        // 5. 쉐이더 적용 + 파라미터 적용
        rend.material = holoMaterial;
        rend.SetPropertyBlock(block);
    }
}
