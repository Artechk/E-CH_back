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
        public string MiddleName { get; set; }
        public string Workplace { get; set; }

        // Список доступных временных слотов с полными датами (DateTime)
        public List<DateTime> AppointmentTimes { get; set; }
    }
}
