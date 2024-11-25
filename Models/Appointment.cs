﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace E_CH_back.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } // ID пользователя

        [BsonRepresentation(BsonType.ObjectId)]
        public string DoctorId { get; set; } // ID врача

        public DateTime AppointmentTime { get; set; } // Время записи
    }

    public class BookAppointmentRequest
    {
        public string UserId { get; set; }
        public string DoctorId { get; set; }
        public DateTime AppointmentTime { get; set; } // Полный DateTime (с датой и временем)
    }
}
