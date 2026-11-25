using UnityEngine;

/// <summary>
///     PlayerData 프리팹에 붙어 있는 예전 세이브 컴포넌트. 기존 호출 코드 호환용으로만 남겨 둔다.
///     실제 저장/불러오기는 <see cref="SaveSystem" />이 담당한다.
/// </summary>
public class SaveData : MonoBehaviour
{
    public static SaveData instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public SaveFile Load()
    {
        return SaveSystem.Load();
    }

    public void Save()
    {
        SaveSystem.SaveCheckpoint();
    }

    public void ResetData()
    {
        SaveSystem.DeleteSave();
    }
}
