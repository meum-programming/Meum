using UnityEditor;

public partial class MeumSaveDataCreater
{
    [MenuItem("Assets/Create/MeumSaveData")]
    public static void CreateDataSheetAssetFile()
    {
        MeumSaveData asset = CustomAssetUtility.CreateAsset<MeumSaveData>();
        EditorUtility.SetDirty(asset);
    }
}