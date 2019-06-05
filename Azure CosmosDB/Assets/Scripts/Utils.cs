using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using UnityEngine;

public class Utils : MonoBehaviour
{

    public static Task<ResourceResponse<Document>> CreateItemAsync(DocumentClient client, Entity.FloatEntity item, Uri collectionLink)
    {
        return client.CreateDocumentAsync(collectionLink, item);

    }

    public static Task<ResourceResponse<Document>> ReplaceItemAsync(DocumentClient client, Entity.FloatEntity item)
    {
        // Need a document Uri, UriFactory.CreateDocumentUri(BasicInfo.DatabaseName, BasicInfo.CollectionName, item.Id)
        return client.ReplaceDocumentAsync(
            UriFactory.CreateDocumentUri(BasicInfo.DatabaseName, BasicInfo.CollectionName, item.Index),
            item);
    }


  

   

    public static int QueryOwnerCountSync(DocumentClient client, Uri collectionLink, string owner)
    {

        string sqlOwner = "\"" + owner + "\"";

        var query = client.CreateDocumentQuery<Entity.FloatEntity>(collectionLink)
            .Where(e => e.Owner == owner).Count();
            

        try
        {
            // Return the count of query results
            return query;

        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }



        return -1;
    }


}
