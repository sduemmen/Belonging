using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

public abstract class Controller : MonoBehaviour, IDataPersistence
{
    protected abstract void OnLoadCompleted();

    public virtual void LoadData(GameData data)
    {
        OnLoadCompleted();
    }

    public abstract void SaveData(ref GameData data);
}