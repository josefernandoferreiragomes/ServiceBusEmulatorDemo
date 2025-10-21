using System.Text.Json.Serialization;

public class TopicConfig
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Properties")]
    public TopicProperties Properties { get; set; } = new();

    [JsonPropertyName("Subscriptions")]
    public List<SubscriptionConfig> Subscriptions { get; set; } = new();
}

public class TopicProperties
{
    [JsonPropertyName("DefaultMessageTimeToLive")]
    public string DefaultMessageTimeToLive { get; set; } = string.Empty;

    [JsonPropertyName("DuplicateDetectionHistoryTimeWindow")]
    public string DuplicateDetectionHistoryTimeWindow { get; set; } = string.Empty;

    [JsonPropertyName("RequiresDuplicateDetection")]
    public bool RequiresDuplicateDetection { get; set; } = false;
}

public class SubscriptionConfig
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Properties")]
    public SubscriptionProperties Properties { get; set; } = new();

    [JsonPropertyName("Rules")]
    public List<RuleConfig> Rules { get; set; } = new();
}

public class SubscriptionProperties
{
    private const int MAX_DELIVERY_COUNT = 10;

    [JsonPropertyName("DeadLetteringOnMessageExpiration")]
    public bool DeadLetteringOnMessageExpiration { get; set; } = false;

    [JsonPropertyName("DefaultMessageTimeToLive")]
    public string DefaultMessageTimeToLive { get; set; } = string.Empty;

    [JsonPropertyName("LockDuration")]
    public string LockDuration { get; set; } = string.Empty;

    [JsonPropertyName("MaxDeliveryCount")]
    public int MaxDeliveryCount { get; set; } = MAX_DELIVERY_COUNT;

    [JsonPropertyName("ForwardDeadLetteredMessagesTo")]
    public string ForwardDeadLetteredMessagesTo { get; set; } = string.Empty;

    [JsonPropertyName("ForwardTo")]
    public string ForwardTo { get; set; } = string.Empty;

    [JsonPropertyName("RequiresSession")]
    public bool RequiresSession { get; set; } = false;
}

public class RuleConfig
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Properties")]
    public RuleProperties Properties { get; set; } = new();
}

public class RuleProperties
{
    [JsonPropertyName("FilterType")]
    public string FilterType { get; set; } = string.Empty;

    [JsonPropertyName("CorrelationFilter")]
    public CorrelationFilter CorrelationFilter { get; set; } = new();

    [JsonPropertyName("SqlFilter")]
    public SqlFilter SqlFilter { get; set; } = new();

    [JsonPropertyName("Action")]
    public SqlAction Action { get; set; } = new();
}

public class CorrelationFilter
{
    // optional system-level correlation fields
    [JsonPropertyName("ContentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("CorrelationId")]
    public string CorrelationId { get; set; } = string.Empty;

    [JsonPropertyName("Label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("MessageId")]
    public string MessageId { get; set; } = string.Empty;

    [JsonPropertyName("ReplyTo")]
    public string ReplyTo { get; set; } = string.Empty;

    [JsonPropertyName("ReplyToSessionId")]
    public string ReplyToSessionId { get; set; } = string.Empty;

    [JsonPropertyName("SessionId")]
    public string SessionId { get; set; } = string.Empty;

    [JsonPropertyName("To")]
    public string To { get; set; } = string.Empty;

    // optional user-defined properties
    [JsonPropertyName("Properties")]
    public Dictionary<string, string> Properties { get; set; } = new();
}

public class SqlFilter
{
    [JsonPropertyName("SqlExpression")]
    public string SqlExpression { get; set; } = string.Empty;
}

public class SqlAction
{
    [JsonPropertyName("SqlExpression")]
    public string SqlExpression { get; set; } = string.Empty;
}
