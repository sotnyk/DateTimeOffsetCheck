using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DateTimeOffsetCheck
{
    class Program
    {
        static Database _db;

        static void Main(string[] args)
        {            
            //CheckDTOBehaviour();
            CheckDtoInDB();
            Console.ReadLine();
        }

        private static void CheckDtoInDB()
        {
            _db = new Database();
            CreateTable();
            PutValues();
            WriteValues();
        }

        private static void WriteValues()
        {
            Console.WriteLine();
            Console.WriteLine("As datetime model");
            foreach (var item in _db.Fetch<DateModel>(@"SELECT * FROM ""Dates"""))
            {
                Console.WriteLine($"Timestamp:{item.Timestamp}, TimestampZ:{item.TimestampZ}, {item.Description}");
                Console.WriteLine($"Kind:{item.Timestamp.Kind}, Kind:{item.TimestampZ.Kind}");
            }
        }

        private static void PutValues()
        {
            DateTime now = DateTime.UtcNow;
            var model = new DateModel()
            {
                Timestamp = now,
                TimestampZ = now,
                Description = "DateTime",
            };
            _db.Insert("Dates", model);

            DateTimeOffset nowZ = new DateTimeOffset(2017, 2, 24, 12, 12, 12, TimeSpan.FromHours(2));
            var modelZ = new DateModelZ()
            {
                Timestamp = nowZ,
                TimestampZ = nowZ,
                Description = "DateTimeOffset",
            };
            _db.Insert("Dates", modelZ);

            DateTimeOffset nowZoff = new DateTimeOffset(2017, 2, 24, 12, 12, 12, TimeSpan.FromHours(5));
            var modelZoff = new DateModelZ()
            {
                Timestamp = nowZoff,
                TimestampZ = nowZoff,
                Description = "DateTimeOffset",
            };

            _db.Insert("Dates", modelZoff);
        }

        private static void CreateTable()
        {
            _db.Execute(@"
DROP TABLE IF EXISTS ""Dates"";

CREATE TABLE ""Dates"" (
    ""Id"" UUID NOT NULL PRIMARY KEY,
    ""Timestamp"" TIMESTAMP NOT NULL,
    ""TimestampZ"" TIMESTAMP WITH TIME ZONE NOT NULL,
    ""Description"" TEXT
);
            ");
            Console.WriteLine("Table Dates has created.");
        }

        private static void CheckDTOBehaviour()
        {
            var now = DateTimeOffset.Now;
            var utcnow = DateTimeOffset.UtcNow;
            DateTime dtnow = now.DateTime;
            DateTime dtutcnow = utcnow.DateTime;

            Console.WriteLine($"now={now}, utcnow={utcnow}");
            Console.WriteLine($"now==utcnow: {now == utcnow}");
            Console.WriteLine($"now.UtcDateTime==utcnow.UtcDateTime: {now.UtcDateTime == utcnow.UtcDateTime}");
            Console.WriteLine($"now.UtcTicks={now.UtcTicks}, utcnow.UtcTicks={utcnow.UtcTicks}");
            Console.WriteLine();
            Console.WriteLine($"dtnow={dtnow}, dtutcnow={dtutcnow}");
            Console.WriteLine($"dtnow==dtutcnow: {dtnow == dtutcnow}");
            Console.WriteLine($"dtnow.Kind={dtnow.Kind}");
            Console.WriteLine($"dtutcnow.Kind={dtutcnow.Kind}");
            Console.WriteLine();
            now = new DateTimeOffset(2017, 2, 24, 12, 38, 0, TimeSpan.FromHours(2));
            utcnow = new DateTimeOffset(2017, 2, 24, 10, 38, 0, TimeSpan.FromHours(0));
            Console.WriteLine($"now={now}, utcnow={utcnow}");
            Console.WriteLine($"now==utcnow: {now == utcnow}");
            Console.WriteLine($"now.UtcDateTime==utcnow.UtcDateTime: {now.UtcDateTime == utcnow.UtcDateTime}");
        }
    }

    public class DateModelZ
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset Timestamp { get; set; }
        public DateTimeOffset TimestampZ { get; set; }
        public string Description { get; set; }
    }

    public class DateModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; }
        public DateTime TimestampZ { get; set; }
        public string Description { get; set; }
    }

}
