using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;

namespace DynamoDBLibrary.Methods
{
    public interface IGetCompanies
    {
        List<Document> Execute(string input);
    }
}