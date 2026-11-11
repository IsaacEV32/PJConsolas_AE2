using System;
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
    List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();
    private IEnumerator Start()
    {

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
            {
                instancesTracked.Add(h.Result);
            }
        }
    }
    void CreateTurrets(ref List<AsyncOperationHandle<GameObject>> handles)
    {
        foreach (Transform p in positions)
        {
            var instParams = new InstantiationParameters(p.position, p.rotation * Quaternion.Euler(0, 180, 0), p);
            var newAsset = Addressables.InstantiateAsync(assetRefTracked, instParams);
            handles.Add(newAsset);
        }
    }
    void ReleaseTrackedResources()
    {
        foreach (var instance in instancesTracked)
        {
            if (instance == null)
            {
                continue;
            }
            Addressables.ReleaseInstance(instance);
        }
        instancesTracked.Clear();

        //Comprobamos que si hay algun handle que fallo o no se uso se libere
        foreach (var h in handles)
        {
            if (h.IsValid())
                Addressables.Release(h);
        }
        handles.Clear();
    }
    private void OnDisable()
    {
        ReleaseTrackedResources();
    }
}
