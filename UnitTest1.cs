using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;
using Neo4jClr;
using Neo4jClr.Nodes;
using Neo4jClr.Builders;

namespace Neo4jTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GivenProductWhenBuildThenProductReferenceShouldExist()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product12";

            var product = new ProductBuilder()
                .WithName(productName)
                .Build();

            var productNodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->product")
                .Where("product.ProductName  = '" + productName + "'")
                .Return<Node<Product>>("product")
                .Results
                .FirstOrDefault();

            Assert.IsTrue(productNodeTest != null && productNodeTest.Reference == product.Reference);
        }

        [TestMethod]
        public void GivenProductAndPipelineWhenBuiltThenProductShouldContainPipeline()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipelineName = "pipeline1";

            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline = new PipelineBuilder().WithName(pipelineName).WithProduct(product).Build();

            var pipelineNodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->product-[:PRODUCT_HAS_PIPELINE]->pipeline")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .Return<Node<Pipeline>>("pipeline")
                .Results.FirstOrDefault();

            Assert.IsTrue(pipelineNodeTest != null && pipelineNodeTest.Reference == pipeline.Reference);
        }

        [TestMethod]
        public void GivenProductAndPipelineAndStageWhenBuiltThenPipelineShouldContainStage()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipelineName = "pipeline1";
            const string stageName = "stage1";


            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline = new PipelineBuilder().WithName(pipelineName).WithProduct(product).Build();

            var stage = new StageBuilder().WithName(stageName).WithPipeline(pipeline).Build();

            var stageNodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match(
                    "root-[:ROOT_HAS_PRODUCT]->product-[:PRODUCT_HAS_PIPELINE]->pipeline-[:PIPELINE_CONTAINS_STAGE]->stage")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .Return<Node<Stage>>("stage")
                .Results.FirstOrDefault();

            Assert.IsTrue(stageNodeTest != null && stageNodeTest.Reference == stage.Reference);
        }

        [TestMethod]
        public void GivenProductAndTwoPipelinesAndOneStageWhenBuiltThenPipelineShouldContainStage()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipeline1Name = "pipeline1";
            const string pipeline2Name = "pipeline2";
            const string stageName = "stage1";

            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline1 = new PipelineBuilder().WithName(pipeline1Name).WithProduct(product).Build();

            var pipeline2 = new PipelineBuilder().WithName(pipeline2Name).WithProduct(product).Build();

            new StageBuilder().WithName(stageName).WithPipeline(pipeline1).Build();

            new StageBuilder().WithName(stageName).WithPipeline(pipeline2).Build();

            var stageNode1Test = Client.Instance().RootNode.StartCypher("root")
                .Match(
                    "root-[:ROOT_HAS_PRODUCT]->product-[:PRODUCT_HAS_PIPELINE]->pipeline-[:PIPELINE_CONTAINS_STAGE]->stage")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipeline1Name + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .Return<Node<Stage>>("stage")
                .Results.FirstOrDefault();

            var stageNode2Test = Client.Instance().RootNode.StartCypher("root")
                .Match(
                    "root-[:ROOT_HAS_PRODUCT]->product-[:PRODUCT_HAS_PIPELINE]->pipeline-[:PIPELINE_CONTAINS_STAGE]->stage")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipeline2Name + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .Return<Node<Stage>>("stage")
                .Results.FirstOrDefault();

            Assert.IsTrue(stageNode2Test != null &&
                          (stageNode1Test != null && stageNode1Test.Reference == stageNode2Test.Reference));
        }

        [TestMethod]
        public void GivenProductAndPipelinesAndStageAndJobWhenBuiltThenStageShouldContainJob()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipelineName = "pipeline1";
            const string stageName = "stage1";
            const string jobName = "job1";


            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline = new PipelineBuilder().WithName(pipelineName).WithProduct(product).Build();

            var stage = new StageBuilder().WithName(stageName).WithPipeline(pipeline).Build();

            var job = new JobBuilder().WithName(jobName).WithStage(stage).Build();

            var jobNodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->" +
                       "product-[:PRODUCT_HAS_PIPELINE]->" +
                       "pipeline-[:PIPELINE_CONTAINS_STAGE]->" +
                       "stage-[:STAGE_GIVEN_JOB]->job")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .And()
                .Where("job.JobName = '" + jobName + "'")
                .Return<Node<Job>>("job")
                .Results.FirstOrDefault();

            Assert.IsTrue(jobNodeTest != null && jobNodeTest.Reference == job.Reference);
        }

        [TestMethod]
        public void GivenProductAndPipelinesAndStageAndTwoJobsWhenBuiltThenStageShouldContainJob()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipelineName = "pipeline1";
            const string stageName = "stage1";
            const string job1Name = "job1";
            const string job2Name = "job2";


            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline = new PipelineBuilder().WithName(pipelineName).WithProduct(product).Build();

            var stage = new StageBuilder().WithName(stageName).WithPipeline(pipeline).Build();

            new JobBuilder().WithName(job1Name).WithStage(stage).Build();

            new JobBuilder().WithName(job2Name).WithStage(stage).Build();

            var job1NodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->" +
                       "product-[:PRODUCT_HAS_PIPELINE]->" +
                       "pipeline-[:PIPELINE_CONTAINS_STAGE]->" +
                       "stage-[:STAGE_GIVEN_JOB]->job")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .And()
                .Where("job.JobName = '" + job1Name + "'")
                .Return<Node<Job>>("job")
                .Results.FirstOrDefault();

            var job2NodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->" +
                       "product-[:PRODUCT_HAS_PIPELINE]->" +
                       "pipeline-[:PIPELINE_CONTAINS_STAGE]->" +
                       "stage-[:STAGE_GIVEN_JOB]->job")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .And()
                .Where("job.JobName = '" + job2Name + "'")
                .Return<Node<Job>>("job")
                .Results.FirstOrDefault();

            Assert.IsTrue(job2NodeTest != null && (job1NodeTest != null && job1NodeTest.Reference != job2NodeTest.Reference));
        }

        [TestMethod]
        public void GivenProductAndPipelinesAndStageAndTwoJobsAndAgentWhenBuiltThenStageShouldContainJob()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipelineName = "pipeline1";
            const string stageName = "stage1";
            const string job1Name = "job1";
            const string job2Name = "job2";
            const string agentName = "agent1";

            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline = new PipelineBuilder().WithName(pipelineName).WithProduct(product).Build();

            var stage = new StageBuilder().WithName(stageName).WithPipeline(pipeline).Build();

            var job1 = new JobBuilder().WithName(job1Name).WithStage(stage).Build();

            var job2 = new JobBuilder().WithName(job2Name).WithStage(stage).Build();

            new AgentBuilder().WithName(agentName).WithJob(job1).Build();

            new AgentBuilder().WithName(agentName).WithJob(job2).Build();
            
            var agent1NodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->" +
                       "product-[:PRODUCT_HAS_PIPELINE]->" +
                       "pipeline-[:PIPELINE_CONTAINS_STAGE]->" +
                       "stage-[:STAGE_GIVEN_JOB]->" +
                       "job-[:JOB_ASSIGNED_AGENT]->" +
                       "agent")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .And()
                .Where("job.JobName = '" + job1Name + "'")
                .And()
                .Where("agent.AgentName = '" + agentName + "'")
                .Return<Node<Agent>>("agent")
                .Results.FirstOrDefault();

            var agent2NodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->" +
                       "product-[:PRODUCT_HAS_PIPELINE]->" +
                       "pipeline-[:PIPELINE_CONTAINS_STAGE]->" +
                       "stage-[:STAGE_GIVEN_JOB]->" +
                       "job-[:JOB_ASSIGNED_AGENT]->" +
                       "agent")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .And()
                .Where("job.JobName = '" + job2Name + "'")
                .And()
                .Where("agent.AgentName = '" + agentName + "'")
                .Return<Node<Agent>>("agent")
                .Results.FirstOrDefault();

            Assert.IsTrue(agent2NodeTest != null && (agent1NodeTest != null && agent1NodeTest.Reference == agent2NodeTest.Reference));
        }

        [TestMethod]
        public void GivenProductAndPipelinesAndStageAndJobAndAgentAndBuildInfoWhenBuiltThenAgentShouldContainBuildAndInfo()
        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipelineName = "pipeline1";
            const string stageName = "stage1";
            const string jobName = "job1";
            const string agentName = "agent1";
            const int buildNumber = 123;
            var buildTimestamp = DateTime.Now;
            const int buildDuration = 10;
            const string buildStatus = "success";

            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline = new PipelineBuilder().WithName(pipelineName).WithProduct(product).Build();

            var stage = new StageBuilder().WithName(stageName).WithPipeline(pipeline).Build();

            var job1 = new JobBuilder().WithName(jobName).WithStage(stage).Build();

            var agent = new AgentBuilder().WithName(agentName).WithJob(job1).Build();

            var build = new BuildBuilder()
                .WithAgent(agent)
                .WithNumber(buildNumber)
                .WithTimestamp(buildTimestamp)
                .WithDuration(buildDuration)
                .WithStatus(buildStatus)
                .Build();

            var buildNodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->" +
                       "product-[:PRODUCT_HAS_PIPELINE]->" +
                       "pipeline-[:PIPELINE_CONTAINS_STAGE]->" +
                       "stage-[:STAGE_GIVEN_JOB]->" +
                       "job-[:JOB_ASSIGNED_AGENT]->" +
                       "agent-[:AGENT_RUNS_BUILD]->" +
                       "build")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .And()
                .Where("job.JobName = '" + jobName + "'")
                .And()
                .Where("agent.AgentName = '" + agentName + "'")
                .And()
                .Where("build.Number = " + buildNumber)
                .And()
                .Where("build.Status = '" + buildStatus + "'")
                .And()
                .Where("build.Timestamp = '" + buildTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                .And()
                .Where("build.Duration = " + buildDuration)
                .Return<Node<Build>>("build")
                .Results.FirstOrDefault();

            Assert.IsTrue(buildNodeTest != null && buildNodeTest.Reference == build.Reference);
        }

        [TestMethod]
        public void GivenProductAndPipelinesAndStageAndJobAndAgentAndBuildAndResultsWhenBuiltThenBuildShouldContainResults()

        {
            Client.Url = "http://localhost:7474/db/data";
            const string productName = "product1";
            const string pipelineName = "pipeline1";
            const string stageName = "stage1";
            const string jobName = "job1";
            const string agentName = "agent1";
            const int buildNumber = 123;
            var schedTimestamp = DateTime.Now;
            const int buildDuration = 100;
            const string buildStatus = "success";
            const int schedDuration = 10;
            var assTimestamp = DateTime.Now.AddMinutes(10);
            const int assDuration = 10;
            var prepTimestamp = DateTime.Now.AddMinutes(20);
            const int prepDuration = 10;
            var buildingTimestamp = DateTime.Now.AddMinutes(30);
            const int buildingDuration = 10;
            var completingTimestamp = DateTime.Now.AddMinutes(40);
            const int completingDuration = 10;
            var completedTimestamp = DateTime.Now.AddMinutes(40);

            var product = new ProductBuilder().WithName(productName).Build();

            var pipeline = new PipelineBuilder().WithName(pipelineName).WithProduct(product).Build();

            var stage = new StageBuilder().WithName(stageName).WithPipeline(pipeline).Build();

            var job1 = new JobBuilder().WithName(jobName).WithStage(stage).Build();

            var agent = new AgentBuilder().WithName(agentName).WithJob(job1).Build();

            var build = new BuildBuilder()
                .WithAgent(agent)
                .WithNumber(buildNumber)
                .WithTimestamp(schedTimestamp)
                .WithDuration(buildDuration)
                .WithStatus(buildStatus)
                .Build();

            var scheduled = new ResultItemBuilder()
                .WithBuild(build)
                .WithType("scheduled")
                .WithTimestamp(schedTimestamp)
                .WithDuration(schedDuration)
                .Build();

            var assigned = new ResultItemBuilder()
                .WithBuild(build)
                .WithType("assigned")
                .WithTimestamp(schedTimestamp)
                .WithDuration(schedDuration)
                .WithMovedFrom(scheduled)
                .Build();

            var prepared = new ResultItemBuilder()
                 .WithBuild(build)
                 .WithType("prepared")
                 .WithTimestamp(schedTimestamp)
                 .WithDuration(schedDuration)
                 .WithMovedFrom(assigned)
                 .Build();

            var built = new ResultItemBuilder()
                 .WithBuild(build)
                 .WithType("built")
                 .WithTimestamp(schedTimestamp)
                 .WithDuration(schedDuration)
                 .WithMovedFrom(prepared)
                 .Build();

            var completing = new ResultItemBuilder()
                 .WithBuild(build)
                 .WithType("completing")
                 .WithTimestamp(schedTimestamp)
                 .WithDuration(schedDuration)
                 .WithMovedFrom(built)
                 .Build();

            var completed = new ResultItemBuilder()
                 .WithBuild(build)
                 .WithType("completed")
                 .WithTimestamp(schedTimestamp)
                 .WithDuration(schedDuration)
                 .WithMovedFrom(completing)
                 .Build();

                //.WithAssigning(assTimestamp, assDuration)
                //.WithPreparing(prepTimestamp, prepDuration)
                //.WithBuilding(buildingTimestamp, buildingDuration)
                //.WithCompleting(completingTimestamp, completingDuration)
                //.WithCompleted(completedTimestamp)
                //.Build();

            var schedNodeTest = Client.Instance().RootNode.StartCypher("root")
                .Match("root-[:ROOT_HAS_PRODUCT]->" +
                       "product-[:PRODUCT_HAS_PIPELINE]->" +
                       "pipeline-[:PIPELINE_CONTAINS_STAGE]->" +
                       "stage-[:STAGE_GIVEN_JOB]->" +
                       "job-[:JOB_ASSIGNED_AGENT]->" +
                       "agent-[:AGENT_RUNS_BUILD]->" +
                       "build-[:BUILD_WAS_SCHEDULED]->" +
                       "sched")
                .Where("product.ProductName  = '" + productName + "'")
                .And()
                .Where("pipeline.PipelineName = '" + pipelineName + "'")
                .And()
                .Where("stage.StageName = '" + stageName + "'")
                .And()
                .Where("job.JobName = '" + jobName + "'")
                .And()
                .Where("agent.AgentName = '" + agentName + "'")
                .And()
                .Where("build.Number = " + buildNumber)
                .And()
                .Where("build.Status = '" + buildStatus + "'")
                .And()
                .Where("build.Timestamp = '" + schedTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                .And()
                .Where("build.Duration = " + buildDuration)
                .And()
                .Where("sched.Timestamp = '" + schedTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                //.And()
                //.Where("sched.Duration = " + schedDuration)
                //.And()
                //.Where("result.AssigningTimestamp = '" + assTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                //.And()
                //.Where("result.AssigningDuration = " + assDuration)
                //.And()
                //.Where("result.PreparingTimestamp = '" + prepTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                //.And()
                //.Where("result.PreparingDuration = " + prepDuration)
                //.And()
                //.Where("result.BuildingTimestamp = '" + buildingTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                //.And()
                //.Where("result.BuildingDuration = " + buildingDuration)
                //.And()
                //.Where("result.CompletingTimestamp = '" + completingTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                //.And()
                //.Where("result.CompletingDuration = " + completingDuration)
                //.And()
                //.Where("result.CompletedTimestamp = '" + completedTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz") + "'")
                .Return<Node<ResultItem>>("sched")
                .Results.FirstOrDefault();

            Assert.IsTrue(schedNodeTest != null && schedNodeTest.Reference == scheduled.Reference);
        }

        [TestMethod]
        public void GivenBuildInfoWhenSendThenTheBuildInfoMustBeSent()
        {
            var schedTimestamp = DateTime.Now;
            const int schedDuration = 10;
            var assTimestamp = DateTime.Now.AddMinutes(10);
            const int assDuration = 10;
            var prepTimestamp = DateTime.Now.AddMinutes(20);
            const int prepDuration = 10;
            var buildingTimestamp = DateTime.Now.AddMinutes(30);
            const int buildingDuration = 10;
            var completingTimestamp = DateTime.Now.AddMinutes(40);
            const int completingDuration = 10;
            var completedTimestamp = DateTime.Now.AddMinutes(40);

            var agentInfo = new AgentInfo("product1", "pipeline1", "stage1", "job1", "agent1");
            var buildInfo = new BuildInfo("success", 123, 100, DateTime.Now);
            var resultInfo = new ResultInfo(schedTimestamp, schedDuration,
                                            assTimestamp, assDuration,
                                            prepTimestamp, prepDuration,
                                            buildingTimestamp, buildingDuration,
                                            completingTimestamp, completingDuration,
                                            completedTimestamp);

            var sender = new SendBuildInfo("http://localhost:7474/db/data", agentInfo, buildInfo, resultInfo);
            sender.Send();

            Assert.IsTrue(sender.Send() == 1);
        }
    }
}
