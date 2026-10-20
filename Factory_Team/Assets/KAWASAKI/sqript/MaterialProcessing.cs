using System;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class MaterialRecipe
{
    public string InputMaterialID;//入力素材のID
    public string OutputMaterialID;//加工後の素材ID
    public int InputAmount;//必要数
    public int OutputAmount;//生産数


}

public class MaterialProcessing : MonoBehaviour
{
    [SerializeField] private TextAsset recipeCsv;

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
        recipes.Clear();    //登録されているレシピの削除

        if (recipeCsv == null)
        {
            Debug.LogError(
                "MaterialConverter: CSVが設定されていません。"
            );
            return;
        }

        string[] lines = recipeCsv.text.Split(
            new[] { "\r\n", "\n", "\r" },
             StringSplitOptions.RemoveEmptyEntries);


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
    }

    public bool TryConvert(
       string inputMaterialId,
       int inputCount,
       out string outputMaterialId,
       out int outputCount,
       out int remainder)
    {
        //初期化
        outputMaterialId = null;
        outputCount = 0;
        remainder = inputCount;
        //素材IDが空、0の場合は加工しない
        if (string.IsNullOrEmpty(inputMaterialId) ||
            inputCount <= 0)
        {
            return false;
        }
        //素材名から対応するレシピを検索
        if (!recipes.TryGetValue(
                inputMaterialId, out MaterialRecipe recipe))
        {
            return false;
        }

        int batches = inputCount / recipe.InputAmount;//batch:加工できる回数

        //加工が可能かの確認
        if (batches <= 0)
        {
            return false;
        }

        outputMaterialId = recipe.OutputMaterialID;//出力素材のID
        outputCount = batches * recipe.OutputAmount;//変換数×一回の生産量
        remainder = inputCount % recipe.InputAmount;//余り

        return true;
    }


}

