using System;
using DynamoDBLibrary.DBContext;
using DynamoDBLibrary.Methods;

namespace DynamoDBLibrary
{
    public static class DBUtilityFactory
    {
        public static IDynamoDBContext CreateDyanmoDBContext() => new LambdaDynamoDbContext();
        public static IPutEntryDate PutEntryDate(IDynamoDBContext db) => new InsertEntryDate(db);
        public static IGetCompanies GetCompanies(IDynamoDBContext db) => new GetCompanies(db);
        public static IGetEmployeeCompanyMapping GetEmployeeCompanyMapping(IDynamoDBContext db) => new GetEmployeeCompanyMapping(db);
        public static IGetEntries GetEntries(IDynamoDBContext db) => new GetEntries(db);
    }
}
