using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using UnityEngine;

public class DatabaseConnection : MonoBehaviour
{


    private DocumentClient client;
    public DocumentClient Client
    {
        get
        {
            if(client == null)
            {
                client = new DocumentClient(new Uri(BasicInfo.EndPointUrl), BasicInfo.PrimaryKey);
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
                documentCollectionLink = UriFactory.CreateDocumentCollectionUri(BasicInfo.DatabaseName, BasicInfo.CollectionName);
            }
            return documentCollectionLink;
        }
    }


    // Initialize database
    private void Start()
    {
        InitializeDatabase();

    }


    public void InitializeDatabase()
    {
        // Check to see if database exist
        Database database = Client.CreateDatabaseQuery().Where(db => db.Id == BasicInfo.DatabaseName).AsEnumerable().FirstOrDefault();
        if (database == null)
        {
            // Create our database
            Client.CreateDatabaseAsync(new Database { Id = BasicInfo.DatabaseName }).Wait();
            database = Client.CreateDatabaseQuery().Where(db => db.Id == BasicInfo.DatabaseName).AsEnumerable().FirstOrDefault();
        }

        // Create our collection for float entities
        var collection = Client.CreateDocumentCollectionQuery(database.SelfLink).Where(col => col.Id == BasicInfo.CollectionName).AsEnumerable().FirstOrDefault();
        if (collection == null)
        {
            Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(BasicInfo.DatabaseName),
                        new DocumentCollection { Id = BasicInfo.CollectionName/*, PartitionKey =*/  },
                        new RequestOptions { OfferThroughput = 1000 }).Wait();
        }


        // Create a leases collection for change feed
        var leases = Client.CreateDocumentCollectionQuery(database.SelfLink).Where(col => col.Id == "leases").AsEnumerable().FirstOrDefault();
        if (leases == null)
        {
            Client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(BasicInfo.DatabaseName),
                new DocumentCollection { Id = "leases" },
                new RequestOptions { OfferThroughput = 1000 }).Wait();
        }



    }

    // Initialize database with 50000 element array of floats and store them in database
    public void PopulateDatabase()
    {
        print("Populating Database...");

        try
        {
            // Query the db to see if already populated
            var query = client.CreateDocumentQuery<Entity.FloatEntity>(DocumentCollectionLink).Count();
            if(query  == BasicInfo.DatabaseSize)
            {
                print("Database already populated!");
                return;
            }
            else if (query != 0)
            {
                ResetDatabase();
                PopulateDatabase();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }


        StartCoroutine(AddEntities());
        print("Done!");
    }

    private IEnumerator AddEntities()
    {
        for (int i = 0; i < BasicInfo.DatabaseSize; i++)
        {
            // Create a new entity
            Entity.FloatEntity entity = new Entity.FloatEntity(i, RandomFloat.NextFloat(), BasicInfo.InitialOwner);

            // Add entity to database
            Utils.CreateItemAsync(Client, entity, DocumentCollectionLink).Wait();

            // Keep Unity in control of thread so the scene doesn't freeze up
            yield return new WaitForEndOfFrame();

        }
    }


    public void ResetDatabase()
    {
        // Delete Collection of floats
        Client.DeleteDocumentCollectionAsync(DocumentCollectionLink);

        // Reinitialize it 
        InitializeDatabase();

    }


}
