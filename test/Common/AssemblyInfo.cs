// Set the orderer
[assembly: TestCollectionOrderer("SecretNET.Tests.Common.PriorityOrderer", "SecretNET.Tests")]

// Need to turn off test parallelization so we can validate the run order
[assembly: CollectionBehavior(DisableTestParallelization = true)]