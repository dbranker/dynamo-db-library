using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;

namespace DynamoDBLibrary.Methods
{
    public interface IGetEntries
    {
        List<Document> Execute(Guid companyid, Guid employeeID, DateTime startdt, DateTime enddt);
    }
}