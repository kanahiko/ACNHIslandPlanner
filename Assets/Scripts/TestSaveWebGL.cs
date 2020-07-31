using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestSaveWebGL : MonoBehaviour
{
    
    public void Save()
    {

        if (!Directory.Exists("idbfs/acnhinslanddesigner"))
        {
            Directory.CreateDirectory("idbfs/acnhinslanddesigner");
        }
        using (StreamWriter write = new StreamWriter("/idbfs/acnhinslanddesigner/test.txt"))
        {
            write.WriteLine("test test test");
            WebGLExtensions.SyncFs();
        }
    }

    public void Load2()
    {
        if (!Directory.Exists("idbfs/acnhinslanddesigner"))
        {
            Directory.CreateDirectory("idbfs/acnhinslanddesigner");
        }
        string filename = "/idbfs/acnhinslanddesigner/test.txt";
        try
        {
            string content = File.ReadAllText(filename);
            Debug.Log("file read: " + content);
        }
        catch (System.IO.IsolatedStorage.IsolatedStorageException e)
        {
            File.WriteAllText(filename,
                "This is a test");
            // call FS.syncfs to actualy save pending fs changes to IndexedDB
            Application.ExternalEval("FS.syncfs(false, function (err) {})");
            Debug.Log("file saved.");
        }
    }

    public void Save2()
    {
        if (!Directory.Exists("idbfs/acnhinslanddesigner"))
        {
            Directory.CreateDirectory("idbfs/acnhinslanddesigner");
        }
        string filename = "/idbfs/acnhinslanddesigner/test.txt";
        File.WriteAllText(filename,
            "This is a test");
        // call FS.syncfs to actualy save pending fs changes to IndexedDB
        Application.ExternalEval("FS.syncfs(false, function (err) {})");
        Debug.Log("file saved.");
    }
    public void Load()
    {

        if (Directory.Exists("idbfs/acnhinslanddesigner"))
        {
            using (StreamReader read = new StreamReader("/idbfs/acnhinslanddesigner/test.txt"))
            {
                Debug.Log(read.ReadLine());
            }
        }
    }
}
