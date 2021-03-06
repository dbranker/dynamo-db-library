using System;
using DynamoDBLibrary.DBContext;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Threading;
using Amazon.DynamoDBv2.Model;
using System.Linq;

namespace DynamoDBLibrary.Methods
{
    public class GetEntries : IGetEntries
    {

        readonly string tableCompanyEntryMapping = "CompanyEntryMapping";
        readonly string tableEntryValues = "CompanyEntryValues";

        private IDynamoDBContext _dbContext;
        public GetEntries(IDynamoDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        private bool isBetween(string entry, DateTime stdt, DateTime enddt)
        {
            DateTime parsed;
            if (!DateTime.TryParse(entry, out parsed))
                return false;
            if (stdt > enddt)
                return parsed >= enddt && parsed <= stdt;
            else
                return parsed >= stdt && parsed <= enddt;
        }
        public List<Document> Execute(Guid companyid, Guid employeeID, DateTime startdt, DateTime enddt)
        {
            //Set Timeout
            CancellationTokenSource tokenKillAsync = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var token = tokenKillAsync.Token;
            QueryRequest request = new QueryRequest();
            Table entryTable = Table.LoadTable(_dbContext.DbClient, tableEntryValues);

            #region Grab Entries By Company
            Dictionary<string, Condition> keyCondition = new Dictionary<string, Condition> {
                         {"CompanyID",  new Condition
                                        {
                                        ComparisonOperator = "EQ",
                                        AttributeValueList = new List<AttributeValue>() {  new AttributeValue {S = companyid.ToString().ToUpper()} }
                             }
                    }
            };
            request = new QueryRequest();
            request.TableName = tableCompanyEntryMapping;
            //request.ExclusiveStartKey = startKey;
            request.AttributesToGet = new List<string> { "EntryID", "EntryDate" };
            request.KeyConditions = keyCondition;
            var searchEntriesResults = _dbContext.DbClient.QueryAsync(request, token).GetAwaiter().GetResult().Items.Select(x => Document.FromAttributeMap(x)).ToList();
            #endregion

            var searchableEntries = searchEntriesResults.Where(x => isBetween(x["EntryDate"], startdt, enddt)).ToList();
            List<Document> finalDocs = new List<Document>();
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "EntryID", "EntryDate", "EntryValues" },
                ConsistentRead = true
            };
            //TODO: create multiple threads for exec here
            foreach (var doc in searchableEntries)
            {
                finalDocs.Add(entryTable.GetItemAsync(doc["EntryID"].ToString().ToUpper(), doc["EntryDate"].ToString(), config).GetAwaiter().GetResult());
            }
            //Search           
            return finalDocs;
        }
    }
}
