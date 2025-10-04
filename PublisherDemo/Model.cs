using System.Text.Json.Serialization;

public class TopicConfig
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Properties")]
    public TopicProperties Properties { get; set; }

    [JsonPropertyName("Subscriptions")]
    public List<SubscriptionConfig> Subscriptions { get; set; } = new();
}

public class TopicProperties
{
    [JsonPropertyName("DefaultMessageTimeToLive")]
    public string DefaultMessageTimeToLive { get; set; }

    [JsonPropertyName("DuplicateDetectionHistoryTimeWindow")]
    public string DuplicateDetectionHistoryTimeWindow { get; set; }

    [JsonPropertyName("RequiresDuplicateDetection")]
    public bool RequiresDuplicateDetection { get; set; }
}

public class SubscriptionConfig
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Properties")]
    public SubscriptionProperties Properties { get; set; }

    [JsonPropertyName("Rules")]
    public List<RuleConfig> Rules { get; set; } = new();
}

public class SubscriptionProperties
{
    [JsonPropertyName("DeadLetteringOnMessageExpiration")]
    public bool DeadLetteringOnMessageExpiration { get; set; }

    [JsonPropertyName("DefaultMessageTimeToLive")]
    public string DefaultMessageTimeToLive { get; set; }

    [JsonPropertyName("LockDuration")]
    public string LockDuration { get; set; }

    [JsonPropertyName("MaxDeliveryCount")]
    public int MaxDeliveryCount { get; set; }

    [JsonPropertyName("ForwardDeadLetteredMessagesTo")]
    public string ForwardDeadLetteredMessagesTo { get; set; }

    [JsonPropertyName("ForwardTo")]
    public string ForwardTo { get; set; }

    [JsonPropertyName("RequiresSession")]
    public bool RequiresSession { get; set; }
}

public class RuleConfig
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Properties")]
    public RuleProperties Properties { get; set; }
}

public class RuleProperties
{
    [JsonPropertyName("FilterType")]
    public string FilterType { get; set; }

    [JsonPropertyName("CorrelationFilter")]
    public CorrelationFilter CorrelationFilter { get; set; }

    [JsonPropertyName("SqlFilter")]
    public SqlFilter SqlFilter { get; set; }

    [JsonPropertyName("Action")]
    public SqlAction Action { get; set; }
}

public class CorrelationFilter
{
    // optional system-level correlation fields
    [JsonPropertyName("ContentType")]
    public string ContentType { get; set; }

    [JsonPropertyName("CorrelationId")]
    public string CorrelationId { get; set; }

    [JsonPropertyName("Label")]
    public string Label { get; set; }

    [JsonPropertyName("MessageId")]
    public string MessageId { get; set; }

    [JsonPropertyName("ReplyTo")]
    public string ReplyTo { get; set; }

    [JsonPropertyName("ReplyToSessionId")]
    public string ReplyToSessionId { get; set; }

    [JsonPropertyName("SessionId")]
    public string SessionId { get; set; }

    [JsonPropertyName("To")]
    public string To { get; set; }

    // optional user-defined properties
    [JsonPropertyName("Properties")]
    public Dictionary<string, string> Properties { get; set; } = new();
}

public class SqlFilter
{
    [JsonPropertyName("SqlExpression")]
    public string SqlExpression { get; set; }
}

public class SqlAction
{
    [JsonPropertyName("SqlExpression")]
    public string SqlExpression { get; set; }
}
