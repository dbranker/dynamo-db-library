using System;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CasinoLibrary.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using DynamoDBLibrary.DBContext;
using System.Threading;

namespace DynamoDBLibrary.Methods
{
    public class GetEmployeeCompanyMapping : IGetEmployeeCompanyMapping
    {
        readonly string tableCompanyEmployeeMappying = "CompanyEmployeeMapping";
        private IDynamoDBContext _dbContext;
        public GetEmployeeCompanyMapping(IDynamoDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        //InsertEntry is responsible for checking if the entry is new. It will create a new version of that 
        public List<Document> Execute(Guid employeeID)
        {
            //Set Table
            Table companyEmployeeTable = Table.LoadTable(_dbContext.DbClient, tableCompanyEmployeeMappying);
            #region INIT REQUEST PARAMS
            // Create the key conditions from hashKey and condition          
            Dictionary<string, Condition> keyCondition = new Dictionary<string, Condition> {
                         {"EmployeeID",  new Condition
                                        {
                                        ComparisonOperator = "EQ",
                                        AttributeValueList = new List<AttributeValue>
                                        {
                                            new AttributeValue { S = employeeID.ToString() }
                                        }
                             }
                    }
            };
            QueryRequest request = new QueryRequest();
            request.TableName = tableCompanyEmployeeMappying;
            //request.ExclusiveStartKey = startKey;
            request.AttributesToGet = new List<string> { "CompanyID" };
            request.KeyConditions = keyCondition;
            #endregion
            CancellationTokenSource tokenKillAsync = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var token = tokenKillAsync.Token;
            var searchResults = _dbContext.DbClient.QueryAsync(request, token).GetAwaiter().GetResult().Items;

            return searchResults.Select(x => Document.FromAttributeMap(x)).ToList();
        }
    }
}
 