using Npgsql;
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
            //CheckDtoInDB();
            CheckDatetimeKind();
            Console.ReadLine();
        }

        private static void CheckDatetimeKind()
        {
            var local = new DateTime(2017, 02, 27, 12, 30, 00, DateTimeKind.Local);
            var utc = new DateTime(2017, 02, 27, 12, 30, 00, DateTimeKind.Utc);
            var unspecified = new DateTime(2017, 02, 27, 12, 30, 00, DateTimeKind.Unspecified);
            Console.WriteLine($"local      = {local}");
            Console.WriteLine($"utc        = {utc}");
            Console.WriteLine($"unpecified = {unspecified}");
            Console.WriteLine();
            Console.WriteLine($"local==utc: {local == utc}");
            Console.WriteLine($"local==unspecified: {local == unspecified}");
            Console.WriteLine($"utc==unspecified: {utc == unspecified}");
            Console.WriteLine();
            Console.WriteLine($"local-utc: {local - utc}");
            Console.WriteLine(local.ToString("r"));
            Console.WriteLine(utc.ToString("r"));

            DateTimeOffset localO = local;
            DateTimeOffset utcO = utc;
            DateTimeOffset unspecifiedO = unspecified;
            Console.WriteLine();
            Console.WriteLine($"localO      = {localO}");
            Console.WriteLine($"utcO        = {utcO}");
            Console.WriteLine($"unpecifiedO = {unspecifiedO}");
            Console.WriteLine();
            Console.WriteLine($"localO==utcO: {localO == utcO}");
            Console.WriteLine($"localO==unspecifiedO: {localO == unspecifiedO}");
            Console.WriteLine($"utcO==unspecifiedO: {utcO == unspecifiedO}");
            Console.WriteLine();
            Console.WriteLine($"localO-utcO: {localO - utcO}");
            Console.WriteLine(localO.ToString("r"));
            Console.WriteLine(utcO.ToString("r"));
        }

        private static void CheckDtoInDB()
        {
            _db = new Database();
            CreateTable();
            PutValues();
            FetchValues();
            DirectFetching();
        }

        private static void FetchValues()
        {
            Console.WriteLine();
            Console.WriteLine("As datetime model");
            foreach (var item in _db.Fetch<DateModel>(@"SELECT * FROM ""Dates"""))
            {
                Console.WriteLine($"Timestamp:{item.Timestamp}, TimestampZ:{item.TimestampZ}, {item.Description}");
                Console.WriteLine($"Kind:{item.Timestamp.Kind}, Kind:{item.TimestampZ.Kind}");
            }
        }

        /// <summary>
        /// Fetching using npgsql
        /// </summary>
        private static void DirectFetching()
        {
            Console.WriteLine();
            Console.WriteLine("Direct reading through npgsql");
            using (var conn = new NpgsqlConnection(_db.ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"SELECT * FROM ""Dates""", conn);

                // Execute the query and obtain a result set
                NpgsqlDataReader dr = command.ExecuteReader();

                var row = new List<object>();
                // Output rows
                while (dr.Read())
                {
                    row.Clear();
                    for (int i = 0; i < dr.FieldCount; ++i)
                        row.Add(dr[i]);
                    Console.WriteLine("Values: " + string.Join("\t", row.Select(col => col.ToString())));
                    Console.WriteLine("Values: " + string.Join("\t", row.Select(col => col.GetType().ToString())));
                    Console.WriteLine("-------------");
                }
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
