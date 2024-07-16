using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileHandler : IFileHandler
{
    private string DataDirPath = "";
    private string DataFileName = "";
   

    public FileHandler(string dataDirPath, string dataFileName) 
    {
        this.DataDirPath = dataDirPath;
        this.DataFileName = dataFileName;
    }

    public PlayerSaveData Load() 
    {
        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(DataDirPath, DataFileName);
        PlayerSaveData loadedData = null;
        if (File.Exists(fullPath)) 
        {
            try 
            {
                // load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
            
                // deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<PlayerSaveData>(dataToLoad);
            }
            catch (Exception e) 
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(PlayerSaveData data) 
    {
        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(DataDirPath, DataFileName);
        try 
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

}