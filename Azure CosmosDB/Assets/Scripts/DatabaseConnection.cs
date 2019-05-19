using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using UnityEngine;
using Newtonsoft.Json;

public class DatabaseConnection : MonoBehaviour
{
    private const int INITIAL_SIZE = 50000;
    private const string ID_ADDITIVE = " Float Index";

    public const string DATABASE_NAME = "VirtualCove";
    public const string COLLECTION_NAME = "FloatsTest";


    public string EndPointUrl = string.Empty;
    public string PrimaryKey = string.Empty;




    private DocumentClient client;
    public DocumentClient Client
    {
        get
        {
            if(client == null)
            {
                client = new DocumentClient(new Uri(EndPointUrl), PrimaryKey);
            }
            return client;
        }
    }

    private Uri documentCollectionLink;
    public Uri DocumentCollectionLink
    {
        get
        {
            if(documentCollectionLink == null)
            {
                documentCollectionLink = UriFactory.CreateDocumentCollectionUri(DATABASE_NAME, COLLECTION_NAME);
            }
            return documentCollectionLink;
        }
    }


    private int numUsers;

    // Initialize database
    private void Start()
    {

        // Initialize Connection to client
        client = new DocumentClient(new Uri(EndPointUrl), PrimaryKey);

        // Create our database
        client.CreateDatabaseAsync(new Database { Id = DATABASE_NAME });

        // Create our collection for float entities
        client.CreateDocumentCollectionAsync(
                    UriFactory.CreateDatabaseUri(DATABASE_NAME),
                    new DocumentCollection { Id = COLLECTION_NAME },
                    new RequestOptions { OfferThroughput = 1000 });

        // Create a leases collection for change feed
        client.CreateDocumentCollectionAsync(
            UriFactory.CreateDatabaseUri(DATABASE_NAME),
            new DocumentCollection { Id = "leases" },
            new RequestOptions { OfferThroughput = 1000 });


        // create and keep a reference to the collection link between our collection and database
        documentCollectionLink = UriFactory.CreateDocumentCollectionUri(DATABASE_NAME, COLLECTION_NAME);


        // Initalize the number of users to 0
        numUsers = 0;

    }



    // Create a new user
    public void NewUser()
    {
        // If this is the first user created, initialize the database
        if (numUsers == 0)
        {
            InitializeDatabase();
        }

        // Create the gameobject from the prefab "User"
        GameObject user = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("User"));

        // Set the position so you can see how many users are there
        user.transform.position = new Vector3((numUsers * 3) - 8, 0, 2);

        //Increment numUsers 
        numUsers++;

    }

    // Initialize database with 50000 element array of floats and store them in database
    public void InitializeDatabase()
    {

        FloatEntity entity;

        // Create 50,000 float entities
        for (int i = 0; i < INITIAL_SIZE; i++) // index has to start at 1 this time
        {
            // Get a random float value
            float f = RandomFloat.NextFloat();

            entity = new FloatEntity(i, f);
            CreateItemAsync( entity, documentCollectionLink);
        }

    }

    public async void WriteRandomFloatToRandomElementInDatabase(System.Random rand)
    {

        // Get a random number between 0th element and max element
        int randomIndex = rand.Next(0, INITIAL_SIZE);

        // Change this element to be a random element
        float randomValue = RandomFloat.NextFloat();

        // Create the object to pass to database 
        FloatEntity entity = new FloatEntity(randomIndex, randomValue);

        try
        {
            await UpdateItemAsync(entity);
        }
        catch(Exception e)
        {
            print(e);
        }

    }



    public Task<ResourceResponse<Document>> CreateItemAsync(FloatEntity item, Uri collectionLink)
    {
        return client.CreateDocumentAsync(collectionLink, item);

    }

    public async Task<Document> UpdateItemAsync(FloatEntity item)
    {
        return await client.UpsertDocumentAsync(documentCollectionLink, item);
    }


    public void ResetDatabase()
    {
        // Delete Collection of floats
        var collectionLink = UriFactory.CreateDocumentCollectionUri(DATABASE_NAME, COLLECTION_NAME);
        client.DeleteDocumentCollectionAsync(collectionLink);

        // Recreate it
        client.CreateDocumentCollectionAsync(
            UriFactory.CreateDatabaseUri(DATABASE_NAME),
            new DocumentCollection { Id = COLLECTION_NAME },
            new RequestOptions { OfferThroughput = 1000 });

        // If there are users make a new database for them to interact with
        if (numUsers > 0)
        {
            // Then initialize it again
            InitializeDatabase();
        }
    }


    public class FloatEntity : Resource
    {
        [JsonProperty(PropertyName ="value")]
        public string Value { get; set; }

        // Define the Id and Value
        public FloatEntity(int index, float floatValue)
        {
            this.Id = index.ToString() + ID_ADDITIVE;
            this.Value = floatValue.ToString();
        }
    }

}
