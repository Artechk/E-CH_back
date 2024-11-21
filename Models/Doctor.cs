using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace E_CH_back.Models
{
    public class Doctor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName {  get; set; }
        public string Workplace { get; set; }
        public List<TimeSlot> AvailableTimes { get; set; } = new();
    }

    public class TimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
