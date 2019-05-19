using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public bool IsOn = true;


    private DatabaseConnection cachedDataConnection;
    public DatabaseConnection DatabaseConnection
    {
        get
        {
            if(cachedDataConnection == null)
            {
                cachedDataConnection = GameObject.Find("DatabaseConnection").GetComponent<DatabaseConnection>();
            }
            return cachedDataConnection;
        }
    }


    private void Start()
    {
        StartCoroutine(RandomInteraction());
    }



    private IEnumerator RandomInteraction()
    {
        System.Random rand = new System.Random();
        while(IsOn)
        {
            yield return new WaitForSeconds(rand.Next(2, 60));
            //yield return new WaitForSeconds(1);
            //print("Looping");
            int loop = rand.Next(5, 2000);
            //int loop = 1;
            for (int i = 0; i < loop; i++)
            {
                DatabaseConnection.WriteRandomFloatToRandomElementInDatabase(rand);
            }

        }
    }
}
