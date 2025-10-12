namespace ChatService.Entities;
public class Message
{
    public int Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // promenjeno sa Text
    public DateTime SentAt { get; set; }                // promenjeno sa Timestamp
}
