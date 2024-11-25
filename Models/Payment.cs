using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace E_CH_back.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AddressId { get; set; }

        [BsonRepresentation(BsonType.String)] // Сохраняется в MongoDB как строка
        [JsonConverter(typeof(JsonStringEnumConverter))] // Для сериализации JSON
        public PaymentType PaymentType { get; set; }

        public double Amount { get; set; }

        public DateTime PaymentDate { get; set; }
    }

    public enum PaymentType
    {
        Rent,   // Квартплата
        Water,  // Вода
        Gas,    // Газ
        Heating // Отопление
    }
}
