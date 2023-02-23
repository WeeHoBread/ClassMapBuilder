using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Test_Multiplayer : MonoBehaviour
{

    public void GetRandomUser(string fileName)
    {
        //We can use this to get random file, first we get a file index
        string saveFileName = "";
        int totalSaves = 0;
        while (File.Exists(fileName + "_" + totalSaves + ".txt"))
        {
            totalSaves++;
            saveFileName = fileName + "_" + totalSaves;
        }

        print("Total Save Files : " + totalSaves);

        int randomInt = Random.Range(0, totalSaves);
        string finalSaveFileName = fileName + "_" + randomInt;

        SaveSystem.Load(finalSaveFileName);
    } //This function is deprecated...

    public void LoadGameSave()
    {
        /*
        if (File.Exists(Grid_Save_Folder + fileName + "." + Save_Extention))
        {
            string saveString = File.ReadAllText(Grid_Save_Folder + fileName + "." + Save_Extention);
            return saveString;
        }
        else
        {
            return null;
        }*/
    }
}
