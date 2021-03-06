using System;
using DynamoDBLibrary.DBContext;
using Amazon.DynamoDBv2.DocumentModel;
using CasinoLibrary.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DynamoDBLibrary.Methods
{
    public class InsertEntryDate : IPutEntryDate
    {
        readonly string tableComapnyEntryValue = "CompanyEntryValues";
        readonly string tableCompanyEntryMapping = "CompanyEntryMapping";
        private IDynamoDBContext _dbContext;
        public InsertEntryDate(IDynamoDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        //TODO: Make Dynamic based upon new fields
        //TODO: Make Company type <T> instead

        //InsertEntry is responsible for checking if the entry is new. It will create a new version of that 
        public void Execute(Guid CompanyID, Guid EmployeeID, DateTime entryDate, CompanyEntry companyEntry)
        {
            //Set Table
            Table entryCompanyMappingTable = Table.LoadTable(_dbContext.DbClient, tableCompanyEntryMapping);
            Table entryCompanyMappingValues = Table.LoadTable(_dbContext.DbClient, tableComapnyEntryValue);
            //query clause
            var companyQueryFilter = new QueryFilter("CompanyID", QueryOperator.Equal, CompanyID.ToString());
            companyQueryFilter.AddCondition("EntryDate", QueryOperator.Equal, entryDate.ToString("MM/dd/yyyy"));
            QueryOperationConfig companyQueryConfig = new QueryOperationConfig()
            {
                AttributesToGet = new List<string> { "EntryID", "IsActive" },
                ConsistentRead = true,
                Filter = companyQueryFilter
            };
            //Search
            var search = entryCompanyMappingTable.Query(companyQueryConfig);
            var json = JsonConvert.SerializeObject(companyEntry);
            if (search.Count == 0)
            {
                //Add to both Company                 
                var guid = Guid.NewGuid();
                var newEntry = new Document();
                var newEntryMapping = new Document();
                newEntry["EntryID"] = guid.ToString();
                newEntry["EntryDate"] = entryDate.ToString("MM/dd/yyyy");
                newEntry["DateCreatedUTC"] = DateTime.UtcNow.ToString();
                newEntry["Values"] = json;
                newEntryMapping["EntryID"] = guid.ToString();
                newEntryMapping["CompanyID"] = CompanyID.ToString();
                newEntryMapping["EntryDate"] = entryDate.ToString("MM/dd/yyyy");
                newEntryMapping["EmployeeID"] = EmployeeID.ToString();
                newEntryMapping["IsActive"] = new DynamoDBBool(true);
                entryCompanyMappingTable.PutItemAsync(newEntry).GetAwaiter().GetResult();
                entryCompanyMappingTable.PutItemAsync(newEntryMapping).GetAwaiter().GetResult();

            }
            else
            {
                var matches = search.Matches.Where(x => x["IsActive"] == new DynamoDBBool(true) && x["Values"] == json);
                if (matches.Count() == 0)
                {
                    //Deactivate older record
                    var book = new Document();
                    book["EntryID"] = search.Matches.Where(x => x["IsActive"] == new DynamoDBBool(true)).FirstOrDefault()["EntryID"].ToString();
                    book["IsActive"] = new DynamoDBBool(false);
                    UpdateItemOperationConfig updateConfig = new UpdateItemOperationConfig
                    {
                        ReturnValues = ReturnValues.None
                    };
                    entryCompanyMappingTable.UpdateItemAsync(book, updateConfig).GetAwaiter().GetResult();
                    // and push new record out
                    var guid = Guid.NewGuid();
                    var newEntry = new Document();
                    var newEntryMapping = new Document();
                    newEntry["EntryID"] = guid.ToString();
                    newEntry["EntryDate"] = entryDate.ToString("MM/dd/yyyy");
                    newEntry["DateCreatedUTC"] = DateTime.UtcNow.ToString();
                    newEntry["Values"] = json;
                    newEntryMapping["EntryID"] = guid.ToString();
                    newEntryMapping["CompanyID"] = CompanyID.ToString();
                    newEntryMapping["EntryDate"] = entryDate.ToString("MM/dd/yyyy");
                    newEntryMapping["EmployeeID"] = EmployeeID.ToString();
                    newEntryMapping["isActive"] = new DynamoDBBool(true);
                    entryCompanyMappingTable.PutItemAsync(newEntry);
                    entryCompanyMappingTable.PutItemAsync(newEntryMapping);
                }
                else
                {
                    //Deactivate older record
                    var oldBook = new Document();
                    oldBook["EntryID"] = search.Matches.Where(x => x["IsActive"] == new DynamoDBBool(true)).FirstOrDefault()["EntryID"].ToString();
                    oldBook["IsActive"] = new DynamoDBBool(false);
                    UpdateItemOperationConfig updateConfig = new UpdateItemOperationConfig
                    {
                        ReturnValues = ReturnValues.None
                    };
                    entryCompanyMappingTable.UpdateItemAsync(oldBook, updateConfig).GetAwaiter().GetResult();
                    //update new record
                    var newBook = new Document();
                    newBook["EntryID"] = matches.FirstOrDefault()["EntryID"].ToString();
                    newBook["IsActive"] = new DynamoDBBool(true);
                    entryCompanyMappingTable.UpdateItemAsync(newBook, updateConfig).GetAwaiter().GetResult();

                }
            }
        }
    }
}
