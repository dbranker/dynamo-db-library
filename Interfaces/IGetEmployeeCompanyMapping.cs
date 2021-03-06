using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;

namespace DynamoDBLibrary.Methods
{
    public interface IGetEmployeeCompanyMapping
    {
        List<Document> Execute(Guid employeeID);
    }
}