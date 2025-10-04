USE [SbMessageContainerDatabase00001]

/*VIEW QUEUES TABLE*/
SELECT
     [QueuesTable].[EntityGroupId]
    ,[QueuesTable].[Id]
    ,[EntityLookupTable].[Name]
    ,[EntityLookupTable].[MessageCount]
    ,[EntityLookupTable].[ExpiryTime]
FROM [QueuesTable]
LEFT JOIN [EntityLookupTable] ON (
        [EntityLookupTable].[EntityGroupId] = [QueuesTable].[EntityGroupId]
    AND [EntityLookupTable].Id = [QueuesTable].[Id]
)
ORDER BY [EntityLookupTable].[Name]

/*VIEW TOPICS TABLE*/
SELECT
     [TopicsTable].[EntityGroupId]
    ,[TopicsTable].[Id]
    ,[EntityLookupTable].[Name]
    ,[EntityLookupTable].[MessageCount]
    ,[EntityLookupTable].[ExpiryTime]
FROM [TopicsTable]
LEFT JOIN [EntityLookupTable] ON (
        [EntityLookupTable].[EntityGroupId] = [TopicsTable].[EntityGroupId]
    AND [EntityLookupTable].Id = [TopicsTable].[Id]
)
ORDER BY [EntityLookupTable].[Name]

/*VIEW SUBSCRIPTIONS TABLE*/
SELECT
     [SubscriptionsTable].[EntityGroupId]
    ,[SubscriptionsTable].[Id]
    ,[SubscriptionsTable].[TopicId]
    ,[EntityLookupTable].[Name]
    ,[EntityLookupTable].[MessageCount]
    ,[EntityLookupTable].[ExpiryTime]
FROM [SubscriptionsTable]
LEFT JOIN [EntityLookupTable] ON (
        [EntityLookupTable].[EntityGroupId] = [SubscriptionsTable].[EntityGroupId]
    AND [EntityLookupTable].Id = [SubscriptionsTable].[Id]
)
ORDER BY [EntityLookupTable].[Name]

/*VIEW FILTERS TABLE*/
SELECT
     [FiltersTable].[EntityGroupId]
    ,[FiltersTable].[Id]
    ,[EntityLookupTable].[Name]
    ,[EntityLookupTable].[MessageCount]
    ,[EntityLookupTable].[ExpiryTime]
FROM [FiltersTable]
LEFT JOIN [EntityLookupTable] ON (
        [EntityLookupTable].[EntityGroupId] = [FiltersTable].[EntityGroupId]
    AND [EntityLookupTable].Id = [FiltersTable].[Id]
)
ORDER BY [EntityLookupTable].[Name]

/*VIEW LOGS TABLE FOR QUEUES*/
SELECT
     [QueuesTable].[EntityGroupId]
    ,[QueuesTable].[Id]
    ,[EntityLookupTable].[Name]    
    ,[LogsTable].[CreatedTime]
    ,[LogsTable].[EntityId]
    ,[LogsTable].[ParentEntityId]
    ,[LogsTable].[UpdateVersion]
FROM [QueuesTable]
LEFT JOIN [EntityLookupTable] ON (
        [EntityLookupTable].[EntityGroupId] = [QueuesTable].[EntityGroupId]
    AND [EntityLookupTable].Id = [QueuesTable].[Id]
)
LEFT JOIN [LogsTable] ON (
        [LogsTable].[EntityGroupId] = [EntityLookupTable].[EntityGroupId]
)
ORDER BY [EntityLookupTable].[Name]

/*VIEW MESSAGES TABLE FOR QUEUES*/
SELECT
     [QueuesTable].[EntityGroupId]
    ,[QueuesTable].[Id]
    ,[EntityLookupTable].[Name]    
    ,[MessagesTable].[SubqueueType]
    ,[MessagesTable].[LastUpdatedTime]
    ,[MessagesTable].[Body]
FROM [QueuesTable]
LEFT JOIN [EntityLookupTable] ON (
        [EntityLookupTable].[EntityGroupId] = [QueuesTable].[EntityGroupId]
    AND [EntityLookupTable].Id = [QueuesTable].[Id]
)
LEFT JOIN [MessagesTable] ON (
    [MessagesTable].[EntityGroupId] = [QueuesTable].[EntityGroupId]
)
ORDER BY [EntityLookupTable].[Name]