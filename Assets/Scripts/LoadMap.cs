using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
public class LoadMap : MonoBehaviour
{
    [SerializeField] List<Transform> positions = new List<Transform>();
    public AssetReferenceGameObject assetRefTracked; // Identify the asset
    List<GameObject> instancesTracked = new List<GameObject>();
    private IEnumerator Start()
    {
        List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();

        CreateTurrets(ref handles);

        // Espera a que TODOS terminen
        foreach (var h in handles)
        {
            if (!h.IsDone)
                yield return h;
        }

        foreach (var h in handles)
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
                instancesTracked.Add(h.Result);
        }
    }
    void CreateTurrets(ref List<AsyncOperationHandle<GameObject>> handles)
    {
        foreach (Transform p in positions)
        {
            var instParams = new InstantiationParameters(p.position, p.rotation * Quaternion.Euler(0, 180, 0), p);
            handles.Add(Addressables.InstantiateAsync(assetRefTracked, instParams));
        }
    }
    void ReleaseTrackedResources()
    {
        foreach (GameObject instances in instancesTracked)
        {
            Addressables.ReleaseInstance(instances);
        }
    }
    void OnDestroy()
    {
        ReleaseTrackedResources();
    }
    private void OnDisable()
    {
        ReleaseTrackedResources();
    }
}
