using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    #region Exposed
    
    #endregion

    #region Main methods
    public static void SaveScoresList() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.scores";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, ScoresList.Instance);
        stream.Close();
    }

    public static ScoresList LoadScoresList() {
        string path = Application.persistentDataPath + "/player.scores";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ScoresList data = formatter.Deserialize(stream) as ScoresList;
            stream.Close();

            return data;
        } else return null;
    }

    public static void DestroySaveData() {
        string path = Application.persistentDataPath + "/player.scores";
        File.Delete(path);
    }
    #endregion

    #region Private & Protected
    
    #endregion
}
