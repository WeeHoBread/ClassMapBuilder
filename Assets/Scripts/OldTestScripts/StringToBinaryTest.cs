using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringToBinaryTest : MonoBehaviour
{
    public string oldLine;
    public string binary;
    private string newLine;
    //private string testLine = "hey";
    //private int store;
    // Start is called before the first frame update
    void Start()
    {
        #region Array Binary
        //string line = "Chinosama";
        //byte[] bytes = Encoding.UTF8.GetBytes(oldLine);

        //for (int i = 0; i < bytes.Length; i++)
        //{
        //    Debug.Log("To binary: " + bytes[i]);
        //}
        //foreach (byte num in bytes)
        //{
        //    newLine += Convert.ToChar(num);
        //}
        //Debug.Log(newLine);
        //string newLine = Convert.ToBase64String(bytes);
        //Debug.Log("Back to string " + newLine);
        #endregion


        #region String binary
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    foreach (char c in oldLine)
        //    {
        //        binary += Convert.ToString(c, 2).PadLeft(8, '0');
        //    }

        //    List<Byte> byteList = new List<Byte>();

        //    for (int i = 0; i < binary.Length; i += 8)
        //    {
        //        byteList.Add(Convert.ToByte(binary.Substring(i, 8), 2));
        //    }
        //    newLine = Encoding.ASCII.GetString(byteList.ToArray());
        //    Debug.Log(newLine);
        //}
        //Debug.Log(binary);
        //foreach (char c in binary)
        //{
        //    newLine += Convert.ToChar(binary, 2);
        //}
        #endregion

        #region Failed Test 1
        //Serialize();
        //Deserialize();
        #endregion

        #region Next Test

        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach (char c in oldLine)
            {
                binary += Convert.ToString(c, 2).PadLeft(8, '0');
            }

            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(binary.Substring(i, 8), 2));
            }
            newLine = Encoding.ASCII.GetString(byteList.ToArray());
            Debug.Log(newLine);
        }
    }
    #region Serialize
    //static void Serialize()
    //{
    //    // Create a hashtable of values that will eventually be serialized.
    //    Hashtable addresses = new Hashtable();
    //    addresses.Add("Jeff", "123 Main Street, Redmond, WA 98052");
    //    addresses.Add("Fred", "987 Pine Road, Phila., PA 19116");
    //    addresses.Add("Mary", "PO Box 112233, Palo Alto, CA 94301");

    //    // To serialize the hashtable and its key/value pairs,
    //    // you must first open a stream for writing.
    //    // In this case, use a file stream.
    //    FileStream fs = new FileStream("DataFile.dat", FileMode.Create);

    //    // Construct a BinaryFormatter and use it to serialize the data to the stream.
    //    BinaryFormatter formatter = new BinaryFormatter();
    //    try
    //    {
    //        formatter.Serialize(fs, addresses);
    //    }
    //    catch (SerializationException e)
    //    {
    //        Debug.Log("Failed to serialize. Reason: " + e.Message);
    //        throw;
    //    }
    //    finally
    //    {
    //        fs.Close();
    //    }
    //}

    //static void Deserialize()
    //{
    //    // Declare the hashtable reference.
    //    Hashtable addresses = null;

    //    // Open the file containing the data that you want to deserialize.
    //    FileStream fs = new FileStream("DataFile.dat", FileMode.Open);
    //    try
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();

    //        // Deserialize the hashtable from the file and
    //        // assign the reference to the local variable.
    //        addresses = (Hashtable)formatter.Deserialize(fs);
    //    }
    //    catch (SerializationException e)
    //    {
    //        Debug.Log("Failed to deserialize. Reason: " + e.Message);
    //        throw;
    //    }
    //    finally
    //    {
    //        fs.Close();
    //    }

    //    // To prove that the table deserialized correctly,
    //    // display the key/value pairs.
    //    foreach (DictionaryEntry de in addresses)
    //    {
    //        Debug.Log("{0} lives at {1}." + de.Key + de.Value);
    //    }
    //}
    #endregion
}
