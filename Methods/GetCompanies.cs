using System;
using DynamoDBLibrary.DBContext;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Threading;
using Amazon.DynamoDBv2.Model;
using System.Linq;
using Newtonsoft.Json;

namespace DynamoDBLibrary.Methods
{
    public class GetCompanies : IGetCompanies
    {
        readonly string tableCompany = "Company";
        private IDynamoDBContext _dbContext;
        public GetCompanies(IDynamoDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        //Look for 
        public List<Document> Execute(string input)
        {
            //Setup Table
            Table companyTable = Table.LoadTable(_dbContext.DbClient, tableCompany);

            #region INIT REQUEST PARAMS
            ScanRequest request = new ScanRequest();
            request.TableName = "Company";
            request.AttributesToGet = new List<string> { "CompanyID", "CompanyName" };
            #endregion

            CancellationTokenSource tokenKillAsync = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var token = tokenKillAsync.Token;
            var scanResponse = _dbContext.DbClient.ScanAsync(request, token).GetAwaiter().GetResult().Items;
            var finalList = new List<Document>();
            foreach (var dic in scanResponse)
            {
                finalList.Add(Document.FromAttributeMap(dic));
            }

            return finalList;
        }
    }
}
