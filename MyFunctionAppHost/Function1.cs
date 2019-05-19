using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MyFunctionAppHost
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([CosmosDBTrigger( // This is a trigger that is activated from the Azure Cosmos ChangeFeed
            databaseName: "VirtualCove",  // Type in database name if different
            collectionName: "FloatsTest", // Type in name of collection if different
            ConnectionStringSetting = "", // Get the Connection strings from the storage account hooked up to your Azure Portal (https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#get-your-storage-connection-strings)
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}


/*
 * The trigger doesn't indicate whether a document was updated or inserted, 
 * it just provides the document itself. If you need to handle updates and
 * inserts differently, you could do that by implementing timestamp fields
 * for insertion or update.
 *
 */
 
