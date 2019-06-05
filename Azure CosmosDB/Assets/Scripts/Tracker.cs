using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tracker : MonoBehaviour
{
    public Text NameText;
    public Text ScoreText;

    private bool track;


    private DatabaseConnection cachedDatabase;
    public DatabaseConnection Database
    {
        get
        {
            if (cachedDatabase == null)
            {
                cachedDatabase = GameObject.Find("/DatabaseConnection").GetComponent<DatabaseConnection>();
            }
            return cachedDatabase;
        }
    }



    public void SetName(string name)
    {
        NameText.text = name;
    }

    public void StartTracking()
    {
        print("Tracking - " + NameText.text);

        track = true;
        ScoreText.text = "0";
        StartCoroutine(Track(NameText.text));
    }

    public void StopTracking()
    {
        track = false;
        ScoreText.text = "-1";
    }

    private IEnumerator Track(string owner)
    {
        System.Random rand = new System.Random();
        while (track)
        {
            ScoreText.text = Utils.QueryOwnerCountSync(Database.Client, Database.DocumentCollectionLink, owner).ToString();

            yield return new WaitForSeconds(rand.Next(10));
        }
    }
}
