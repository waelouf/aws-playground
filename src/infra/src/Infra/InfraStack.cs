using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace Infra
{
    public class InfraStack : Stack
    {
        internal InfraStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            var lambda = new Function(this, "scrapper-lambda", new FunctionProps()
            {
                FunctionName = "scrapper-poc",
                Runtime = Runtime.DOTNET_8,
                Handler = "Scrapper.Lambda::Scrapper.Lambda.Function::FunctionHandler",
                Code = Code.FromAsset(
                    @"E:\CODE\cloud-scrapper\src\infra\src\Scrapper.Lambda\src\Scrapper.Lambda\bin\Release\net8.0\Scrapper.Lambda.zip"),
                Timeout = Duration.Seconds(30)
            });

            var userTable = new Table(this, "user-table", new TableProps
            {
                TableName = "User",
                PartitionKey = new Attribute
                {
                    Name = "Id",
                    Type = AttributeType.STRING
                },
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            userTable.GrantReadWriteData(lambda);
        }
    }
}
