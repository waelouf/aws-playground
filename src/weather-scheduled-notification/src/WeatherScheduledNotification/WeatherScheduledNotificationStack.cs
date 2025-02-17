using System;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
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
            var stage = "prod";
            // ** Step 1: Create an SQS Queue **
            var queue = new Queue(this, "weather-notifications-queue", new QueueProps
            {
                VisibilityTimeout = Duration.Seconds(30)
            });
            
              // ** Step 2: Create an API Gateway **
            var api = new RestApi(this, "WeatherApiGateway", new RestApiProps
            {
                RestApiName = "SchedulerApiGateway",
                Description = "API Gateway triggered by EventBridge Scheduler",
                DeployOptions = new StageOptions { StageName = stage }
            });
            
            // ** Step 4: Create an SNS Topic **
            var topic = new Topic(this, "WeatherSNSTopic", new TopicProps
            {
                DisplayName = "Weather Notification Topic"
            });

            // ** Step 3: Create a Lambda Function **
            var lambdaPublisherFunction = new Function(this, "WeatherLambda", new FunctionProps
            {
                FunctionName = "WeatherLambda",
                Runtime = Runtime.DOTNET_8, // Adjust if needed
                Handler = "WeatherLambdaFunction::WeatherLambdaFunction.Function::FunctionHandler",
                Code = Code.FromAsset(
                    "lambda\\WeatherLambdaFunction.zip"), // Ensure the 'lambda' directory exists with Lambda code
                Environment = new Dictionary<string, string>
                {
                    { "WeatherApi", System.Environment.GetEnvironmentVariable("WeatherApi") },
                    { "QUEUE_URL", queue.QueueUrl }
                },
                Timeout = Duration.Seconds(30)
            });
            
            var lambdaConsumerFunction = new Function(this, "WeatherLambdaConsumer", new FunctionProps
            {
                FunctionName = "WeatherLambdaConsumer",
                Runtime = Runtime.DOTNET_8, // Adjust if needed
                Handler = "WeatherSqsConsumerLambdaFunction::WeatherSqsConsumerLambdaFunction.Function::FunctionHandler",
                Code = Code.FromAsset(
                    "lambda\\WeatherLambdaConsumer.zip"), // Ensure the 'lambda' directory exists with Lambda code
                Environment = new Dictionary<string, string>
                {
                    { "TOPIC_ARN", topic.TopicArn }
                },
                Timeout = Duration.Seconds(30)
            });

            // Grant Lambda permission to send messages to SQS
            queue.GrantSendMessages(lambdaPublisherFunction);

            // Add SQS as an event source for Lambda
            lambdaPublisherFunction.AddEventSource(new SqsEventSource(queue));

           

            // Subscribe an email (modify as needed)
            var email = System.Environment.GetEnvironmentVariable("TEST_EMAIL");
            topic.AddSubscription(new EmailSubscription(email));

            // ** Step 5: Subscribe SQS to SNS **
            queue.GrantConsumeMessages(lambdaConsumerFunction);
            topic.GrantPublish(lambdaConsumerFunction);
            topic.AddSubscription(new SqsSubscription(queue));

            lambdaConsumerFunction.AddEventSource(new SqsEventSource(queue, new SqsEventSourceProps
            {
                BatchSize = 10, Enabled = true
            }));
            
            
            // ** Step 6: Configure API Gateway to call Lambda **
            var resource = api.Root.AddResource("trigger");
            resource.AddMethod("POST", new LambdaIntegration(lambdaPublisherFunction));

            // ** Step 7: Grant API Gateway Invoke Permission to EventBridge Scheduler **
            var schedulerRole = new Role(this, "SchedulerRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("scheduler.amazonaws.com")
            });

            schedulerRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "execute-api:Invoke" },
                Resources = new[] { $"{api.ArnForExecuteApi("POST", "/trigger", stage)}" },
                Effect = Effect.ALLOW
            }));
            
            // ** Step 8: Create an EventBridge Scheduler to invoke API Gateway **
            var rule = new Rule(this, "weather-notification-schedule-rule", new RuleProps
            {
                Schedule = Schedule.Expression("rate(5 minutes)") // Run every 5 minutes
            });

            rule.AddTarget(new LambdaFunction(lambdaPublisherFunction));
            lambdaPublisherFunction.GrantInvoke(new ServicePrincipal("events.amazonaws.com"));



            // ** Outputs **
            new CfnOutput(this, "ApiInvokeUrl", new CfnOutputProps { Value = $"{api.Url}trigger" });
            new CfnOutput(this, "SqsQueueUrl", new CfnOutputProps { Value = queue.QueueUrl });
            new CfnOutput(this, "SnsTopicArn", new CfnOutputProps { Value = topic.TopicArn });
       
        }
    }
}
