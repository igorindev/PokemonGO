using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Pokemon : MonoBehaviour
{
    public int id;
    public AsyncOperationHandle<GameObject> handler;

    private void OnDestroy()
    {
        World.instance.pokemons[id].loaded = false;

        Addressables.Release(handler);
    }
}
