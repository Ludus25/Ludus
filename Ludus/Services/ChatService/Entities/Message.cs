namespace ChatService.Entities;
public class Message
{
    public int Id { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string GameId { get; set; } = string.Empty; 
}
