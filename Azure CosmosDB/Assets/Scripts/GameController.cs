using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // These gets set in scene
    public DatabaseConnection Database;
    public Transform GamePanel;

    // User must select these
    public string NameOfCharacter;
    public List<string> OtherUsersToTrack = new List<string>();

    private bool CharacterOn;


    private void Start()
    {
        CharacterOn = false;

    }


    public void StartGame()
    {
        // Spawn trackers
        SpawnTracker(BasicInfo.InitialOwner);
        SpawnTracker(NameOfCharacter);

        // If specified other users to track, add a tracker for them
        foreach (var user in OtherUsersToTrack)
        {
            SpawnTracker(user);
        }


        // Start the game for this character
        CharacterOn = true;
        StartCoroutine(GameTime());
    }

    private IEnumerator GameTime()
    {
        System.Random rand = new System.Random();
        while (CharacterOn)
        {
            print("Playing the Game " + NameOfCharacter);
            //int loop = rand.Next(5, 2000);
            int loop = 1;
            for (int i = 0; i < loop; i++)
            {
                // Get a random number between 0th element and max element
                int randomIndex = rand.Next(0, (BasicInfo.DatabaseSize)-1);

                // Create the object to pass to database 
                Entity.FloatEntity entity = new Entity.FloatEntity(randomIndex, RandomFloat.NextFloat(), NameOfCharacter);

                try
                {
                    Utils.ReplaceItemAsync(Database.Client, entity).Wait();
                }
                catch (Exception e)
                {
                    print(e);
                }

            }
            yield return new WaitForSeconds(rand.Next(2, 5));
        }
    }


    private void SpawnTracker(string name)
    {
        var tracker = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Tracker"));
        tracker.transform.SetParent(GamePanel);

        tracker.name = name;
        tracker.GetComponent<Tracker>().SetName(name);

        tracker.GetComponent<Tracker>().StartTracking();

    }
}
