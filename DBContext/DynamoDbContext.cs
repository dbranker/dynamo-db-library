using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynamoDBLibrary.DBContext
{
    public class LambdaDynamoDbContext : IDynamoDBContext,IDisposable
    {
        readonly string awsRegion;
        readonly string awsKey;
        readonly string awsSecret;
        private AmazonDynamoDBClient _dbClient;
        public AmazonDynamoDBClient DbClient { get => _dbClient;}

        public LambdaDynamoDbContext()
        {
            awsRegion = Environment.GetEnvironmentVariable("AWS_REGION");
            awsKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            awsSecret = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            _dbClient =  new AmazonDynamoDBClient();
        }

        public void Dispose()
        {
            _dbClient.Dispose();
        }
    }
}
