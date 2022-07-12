using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon : MonoBehaviour
{
    public int id;

    private void OnDestroy()
    {
        World.instance.pokemons[id].loaded = false;
    }
}
