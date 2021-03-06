using Amazon.DynamoDBv2;
namespace DynamoDBLibrary.DBContext
{
    public interface IDynamoDBContext
    {
        AmazonDynamoDBClient DbClient { get; }
        void Dispose();
    }
}