using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;
using UnityEngine;

public class KanaDataLoader : MonoBehaviour
{
    private string jsonFilePath = "Assets/Resources/Data/KanaData.json";
    private string connectionString = "Server=127.0.0.1;Database=KanaPractice;User=root;Password=itshardtoguess;";

    void Start()
    {
        LoadKanaDataToDatabase();
    }

    private void LoadKanaDataToDatabase()
    {
        // Step 1: Read JSON File
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/KanaData");

        if (jsonFile == null)
        {
            Debug.LogError($"JSON file not found in: {jsonFilePath}");
            return;
        }

        // Deserialize JSON using KanaListWrapper
        KanaManager.KanaListWrapper kanaWrapper;
        try
        {
            kanaWrapper = JsonUtility.FromJson<KanaManager.KanaListWrapper>(jsonFile.text);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to deserialize JSON: {ex.Message}");
            return;
        }

        if (kanaWrapper == null || kanaWrapper.kana == null || kanaWrapper.kana.Count == 0)
        {
            Debug.LogError("No Kana data found in the JSON file.");
            return;
        }

        List<Kana> kanaList = kanaWrapper.kana;

        // Step 2: Insert Kana Data into the Database
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                foreach (var kana in kanaList)
                {
                    // Check if the Kana already exists in the database
                    string checkQuery = "SELECT COUNT(*) FROM KanaCharacter WHERE KanaSymbol = @KanaSymbol AND Romaji = @Romaji AND Type = @Type";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@KanaSymbol", kana.Symbol);
                        checkCmd.Parameters.AddWithValue("@Romaji", kana.Romaji);
                        checkCmd.Parameters.AddWithValue("@Type", kana.Type);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        // If it doesn't exist, insert it
                        if (count == 0)
                        {
                            string insertQuery = @"INSERT INTO KanaCharacter (KanaSymbol, Romaji, Type)
                                               VALUES (@KanaSymbol, @Romaji, @Type)";
                            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@KanaSymbol", kana.Symbol);
                                insertCmd.Parameters.AddWithValue("@Romaji", kana.Romaji);
                                insertCmd.Parameters.AddWithValue("@Type", kana.Type);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                Debug.Log("Kana data successfully loaded into the database.");
            }
            catch (MySqlException ex)
            {
                Debug.LogError($"MySQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex.Message}");
            }
        }
    }
}