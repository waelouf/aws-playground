using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Scheduler;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace WeatherScheduledNotification
{
    public class WeatherScheduledNotificationStack : Stack
    {
        internal WeatherScheduledNotificationStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
              // ** Step 1: Create an API Gateway **
            var api = new RestApi(this, "WeatherApiGateway", new RestApiProps
            {
                RestApiName = "SchedulerApiGateway",
                Description = "API Gateway triggered by EventBridge Scheduler",
                DeployOptions = new StageOptions { StageName = "prod" }
            });

            // ** Step 2: Create a Lambda Function **
            var lambdaFunction = new Function(this, "WeatherLambda", new FunctionProps
            {
                FunctionName = "WeatherLambda",
                Runtime = Runtime.DOTNET_8, // Adjust if needed
                Handler = "WeatherLambdaFunction::WeatherLambdaFunction.Function::FunctionHandler",
                Code = Code.FromAsset("lambda\\WeatherLambdaFunction.zip"), // Ensure the 'lambda' directory exists with Lambda code
            });

            // ** Step 3: Create an SQS Queue **
            var queue = new Queue(this, "MySqsQueue", new QueueProps
            {
                VisibilityTimeout = Duration.Seconds(30)
            });

            // Grant Lambda permission to send messages to SQS
            queue.GrantSendMessages(lambdaFunction);

            // Add SQS as an event source for Lambda
            lambdaFunction.AddEventSource(new SqsEventSource(queue));

            // ** Step 4: Create an SNS Topic **
            var topic = new Topic(this, "MySnsTopic", new TopicProps
            {
                DisplayName = "My SNS Topic"
            });

            // Subscribe an email (modify as needed)
            topic.AddSubscription(new EmailSubscription("your-email@example.com"));

            // ** Step 5: Subscribe SQS to SNS **
            topic.AddSubscription(new SqsSubscription(queue));

            // ** Step 6: Configure API Gateway to call Lambda **
            var resource = api.Root.AddResource("trigger");
            resource.AddMethod("POST", new LambdaIntegration(lambdaFunction));

            // ** Step 7: Grant API Gateway Invoke Permission to EventBridge Scheduler **
            var schedulerRole = new Role(this, "SchedulerRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("scheduler.amazonaws.com")
            });

            schedulerRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "execute-api:Invoke" },
                Resources = new[] { $"{api.ArnForExecuteApi("POST", "/trigger", "prod")}" },
                Effect = Effect.ALLOW
            }));

            // ** Step 8: Create an EventBridge Scheduler to invoke API Gateway **
            var scheduler = new CfnSchedule(this, "MyEventBridgeScheduler", new CfnScheduleProps
            {
                ScheduleExpression = "rate(1 hour)", // Runs every hour
                FlexibleTimeWindow = new CfnSchedule.FlexibleTimeWindowProperty { Mode = "OFF" },
                Target = new CfnSchedule.TargetProperty
                {
                    Arn = $"{api.Url}trigger",
                    RoleArn = schedulerRole.RoleArn,
                    Input = ""
                }
            });

            // ** Outputs **
            new CfnOutput(this, "ApiInvokeUrl", new CfnOutputProps { Value = $"{api.Url}trigger" });
            new CfnOutput(this, "SqsQueueUrl", new CfnOutputProps { Value = queue.QueueUrl });
            new CfnOutput(this, "SnsTopicArn", new CfnOutputProps { Value = topic.TopicArn });
       
        }
    }
}
