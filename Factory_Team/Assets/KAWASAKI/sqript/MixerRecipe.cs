using System;
using System.Collections.Generic;
using UnityEngine;

// クラフトレシピデータ
[System.Serializable]
public class CraftRecipe
{
    public Item.ItemID Input1;
    public Item.ItemID Input2;
    public Item.ItemID Output;
}

public class MixerRecipe : MonoBehaviour
{

    [SerializeField] private TextAsset recipe_csv;

    // レシピを検索するDictionary
    private readonly Dictionary<string, CraftRecipe> recipes =
        new Dictionary<string, CraftRecipe>(StringComparer.Ordinal);

    private void Awake()
    {
        LoadRecipes();
    }

    // CSV読み込み
    public void LoadRecipes()
    {
        recipes.Clear();

        if (recipe_csv == null)
        {
            Debug.LogError("MixerRecipe: CSVが設定されていません。");
            return;
        }

        string[] lines = recipe_csv.text.Split(
            new[] { "\r\n", "\n", "\r" },
            StringSplitOptions.RemoveEmptyEntries
        );

        // CSVヘッダー確認
        if (lines.Length == 0 ||
            lines[0].Trim() != "Input1,Input2,Output")
        {
            Debug.LogError("クラフトCSVのヘッダーが不正です");
            return;
        }

        // 2行目以降を読み込む
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length != 3)
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: 列数が不正です"
                );
                continue;
            }

            string input1_text = columns[0].Trim();
            string input2_text = columns[1].Trim();
            string output_text = columns[2].Trim();

            // 空欄チェック
            if (string.IsNullOrEmpty(input1_text) ||
                string.IsNullOrEmpty(input2_text) ||
                string.IsNullOrEmpty(output_text))
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: アイテムIDが空です"
                );
                continue;
            }

            // 文字列からItemIDに変換
            bool valid1 = Enum.TryParse(
                input1_text, out Item.ItemID id1);

            bool valid2 = Enum.TryParse(
                input2_text, out Item.ItemID id2);

            bool valid3 = Enum.TryParse(
                output_text, out Item.ItemID output_id);

            if (!valid1 || !valid2 || !valid3 ||
                id1 == Item.ItemID.None ||
                id2 == Item.ItemID.None ||
                output_id == Item.ItemID.None)
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: アイテムIDが不正です"
                );
                continue;
            }

            // 2つの素材IDから検索キーを作成
            string key = MakeRecipeKey(id1, id2);

            // レシピ重複チェック（素材順が逆でも重複扱い）
            if (recipes.ContainsKey(key))
            {
                Debug.LogWarning(
                    $"CSV {i + 1}行目: レシピが重複しています"
                );
                continue;
            }

            // レシピ登録
            recipes.Add(key, new CraftRecipe
            {
                Input1 = id1,
                Input2 = id2,
                Output = output_id
            });
        }

        Debug.Log($"クラフトレシピ読み込み完了: {recipes.Count}件");
    }

    // 2つの素材IDから検索キーを作成
    private string MakeRecipeKey(
        Item.ItemID id1,
        Item.ItemID id2)
    {
        // IDの順番を統一してキーを作る
        if ((int)id1 > (int)id2)
        {
            Item.ItemID temp = id1;
            id1 = id2;
            id2 = temp;
        }

        return id1 + "_" + id2;
    }

    // 2つの素材IDからレシピを取得する
    public bool TryGetRecipe(
        Item.ItemID id1,
        Item.ItemID id2,
        out CraftRecipe recipe)
    {
        // 検索キーを作成
        string key = MakeRecipeKey(id1, id2);

        Debug.Log("===== クラフトレシピ検索 =====");
        Debug.Log("検索素材1：" + id1);
        Debug.Log("検索素材2：" + id2);
        Debug.Log("検索キー：" + key);
        Debug.Log("登録レシピ数：" + recipes.Count);

        // 登録されているレシピのキーをすべて表示
        foreach (var r in recipes)
        {
            Debug.Log("登録キー：" + r.Key);
            Debug.Log(
                "登録レシピ：" +
                r.Value.Input1 + " + " +
                r.Value.Input2 + " → " +
                r.Value.Output
            );
        }

        // レシピ検索
        bool result = recipes.TryGetValue(key, out recipe);

        Debug.Log("検索結果：" + result);

        if (result)
        {
            Debug.Log("完成品：" + recipe.Output);
        }
        else
        {
            Debug.LogWarning("レシピが見つかりません：" + key);
        }

        return result;
    }
}
