using UnityEngine;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * <summary>
 * <para xml:lang="en">Manager class allocating default material on plane prefab.</para>
 * <para xml:lang="ko">plane prefab에 기본 머터리얼을 할당하기 위한 매니저 클래스 입니다.</para>
 * </summary>
 */
[ExecuteInEditMode]
public class MaterialManager : MonoBehaviour
{
#nullable enable
    private Renderer? planeRenderer;
#nullable disable
    void Awake()
    {
        planeRenderer = GetComponent<Renderer>();
        ApplyMaterial();
    }

    void Update()
    {
        if (planeRenderer != null && planeRenderer.sharedMaterial == null)
        {
            planeRenderer = GetComponent<Renderer>();
            ApplyMaterial();
        }

        SetConstantData();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        ApplyMaterial();
        SetConstantData();
    }
#endif

    void ApplyMaterial()
    {
        if (planeRenderer == null)
        {
            planeRenderer = GetComponent<Renderer>();
            if (planeRenderer == null)
            {
                Debug.LogError("Plane Renderer is not found on ad prefab.");
                return;
            }
        }

#nullable enable
        Material? material = new Material(Shader.Find("EasterAd/DefaultShader"));
#nullable disable
        if (material == null)
        {
            Debug.LogError("Material creation fail.");
            return;
        }

        planeRenderer.sharedMaterial = material;
    }

    private void SetConstantData()
    {
        Vector3 data = new Vector3(
            gameObject.transform.lossyScale.x,
            gameObject.transform.lossyScale.z,
            1.0f
        );

        ETA.Plane plane = GetComponent<ETA.Plane>();
        if (plane != null && plane.Client != null)
        {
            ETA_Implementation.ItemStatus status = plane.Client.GetStatus();
            bool hideLogo = status == ETA_Implementation.ItemStatus.Loaded || status == ETA_Implementation.ItemStatus.Impressed;
            data.z = hideLogo ? 0.0f : 1.0f;
        }

        planeRenderer!.sharedMaterial.SetVector("_ConstantData", data);
    }
}