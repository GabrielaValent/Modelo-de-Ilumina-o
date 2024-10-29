using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    Ray ray;
    Texture2D tex;
    public float cameraSize;
    public Transform light1;
    [Range(0, 30)]
    public float specular;
    [Range(0, 1)]
    public float ambiente;
    public Color backgroundColor = Color.black;
    public Color lightColor = Color.white;
    [Range(0, 2)]
    public float globalIntensity = 1.0f;

    void Start()
    {
        ray = new Ray(transform.position, Vector3.forward);
        Renderer rend = GetComponent<Renderer>();
        tex = new Texture2D(256, 256);
        tex.filterMode = FilterMode.Bilinear;
        rend.material.mainTexture = tex;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("RenderScene");
        }
        Debug.DrawRay(ray.origin, ray.direction * 15.0f, Color.red, 0.5f);
    }

    IEnumerator RenderScene()
    {
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float px = ((float)x / tex.width) * cameraSize - cameraSize * 0.5f;
                float py = ((float)y / tex.height) * cameraSize - cameraSize * 0.5f;
                ray.origin = new Vector3(px, py, 0);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Color c = BlinnPhong(hit);
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, backgroundColor);
                }
            }
        }
        tex.Apply();
        yield return null;
    }

    Color BlinnPhong(RaycastHit hit)
    {
        Color hitColor = hit.transform.GetComponent<MeshRenderer>().material.color;
        Vector3 L = (light1.position - hit.point).normalized;
        Vector3 N = hit.normal;
        Vector3 V = (transform.position - hit.point).normalized;
        Vector3 H = (L + V).normalized;
        float NdotH = Mathf.Max(0, Vector3.Dot(N, H));
        float diff = Mathf.Max(0, Vector3.Dot(N, L));
        float spec = Mathf.Pow(NdotH, (specular * 2 + 1));

        Color finalColor = hitColor * (ambiente + diff * globalIntensity) * lightColor;
        finalColor += spec * lightColor * globalIntensity;

        return finalColor;
    }
}

