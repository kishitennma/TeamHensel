
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialRecipe
{
    public string InputMaterialID;//入力素材のID
    public string OutputMaterialID;//加工後の素材ID
    public int InputAmount;//必要数
    public int OutputAmount;//生産数
}

public class MaterialProcessing : MonoBehaviour
{
    [SerializeField] private TextAsset recipe_csv;
    [SerializeField] private Item[] item_prefabs;
    private readonly Dictionary<string, MaterialRecipe> recipes =
    new Dictionary<string, MaterialRecipe>(
        StringComparer.Ordinal
    );//キー（文字）で対応するデータを検索する

    private void Awake()
    {
        LoadRecipes();
    }

    public void LoadRecipes()
    {
        recipes.Clear();//登録されているレシピの削除

        if (recipe_csv == null)
        {
            Debug.LogError(
                "MaterialConverter: CSVが設定されていません。"
            );
            return;
        }

        string[] lines = recipe_csv.text.Split(
            new[] { "\r\n", "\n", "\r" },
            StringSplitOptions.RemoveEmptyEntries
        );

        //CSVのヘッダーが一致しているか確認
        if (lines.Length == 0 ||
            lines[0].Trim() !=
            "InputMaterialID,OutputMaterialID,InputAmount,OutputAmount")
        {
            Debug.LogError("CSVのヘッダーが不正です");
            return;
        }

        //２行目以降
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length != 4)
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: 列数が不正です"
                );
                continue;
            }

            string inputId = columns[0].Trim();
            string outputId = columns[1].Trim();

            if (string.IsNullOrEmpty(inputId) ||
                string.IsNullOrEmpty(outputId))
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: 素材IDが空です"
                );
                continue;
            }

            //文字列→数値に変換する
            if (!int.TryParse(columns[2].Trim(),
                    out int inputAmount) ||
                !int.TryParse(columns[3].Trim(),
                    out int outputAmount) ||
                inputAmount <= 0 ||
                outputAmount <= 0)
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: 素材数が不正です"
                );
                continue;
            }

            if (recipes.ContainsKey(inputId))
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: 入力素材IDが重複しています: {inputId}"
                );
                continue;
            }

            recipes.Add(inputId, new MaterialRecipe
            {
                InputMaterialID = inputId,
                OutputMaterialID = outputId,
                InputAmount = inputAmount,
                OutputAmount = outputAmount
            });
        }

        Debug.Log($"レシピ読み込み完了: {recipes.Count}件");

        Debug.Log(
    "【読み込み側】InstanceID：" + GetInstanceID()
);
        foreach (var recipe in recipes)
        {
            Debug.Log("登録されたレシピID：" + recipe.Key);
        }
    }

    //素材IDからレシピを取得する
    public bool TryGetRecipe(
    string inputMaterialId,
    out MaterialRecipe recipe)
    {
        Debug.Log("===== レシピ検索 =====");
        Debug.Log("InstanceID：" + GetInstanceID());
        Debug.Log("検索ID：" + inputMaterialId);
        Debug.Log("登録件数：" + recipes.Count);

        foreach (var r in recipes)
        {
            Debug.Log("登録ID：" + r.Key);
        }

        bool result = recipes.TryGetValue(inputMaterialId, out recipe);

        Debug.Log("検索結果：" + result);

        return result;
    }
    // ItemIDから対応するPrefabを取得する
    public bool TryGetItemPrefab(
        Item.ItemID itemID,
        out Item prefab)
    {
        prefab = null;

        foreach (Item item in item_prefabs)
        {
            if (item == null) continue;

            if (item.GetItemID() == itemID)
            {
                prefab = item;
                return true;
            }
        }

        Debug.LogWarning("Prefabが見つかりません：" + itemID);
        return false;
    }



}
