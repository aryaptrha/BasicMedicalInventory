using Microsoft.Data.SqlClient;
using InitialSetupMVC.Data;
using InitialSetupMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace InitialSetupMVC.Repositories
{
    public class MedicineRepository
    {
        private readonly DbConnection _db;

        public MedicineRepository(DbConnection db)
        {
            _db = db;
        }

        public List<Medicine> GetAll()
        {
            var list = new List<Medicine>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetAllMedicines", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapMedicine(reader));
            }
            return list;
        }

        public Medicine? GetById(long id)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetMedicineById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapMedicine(reader);
            }
            return null;
        }

        public void Create(Medicine med)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_CreateMedicine", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@MedicineName", med.MedicineName);
            cmd.Parameters.AddWithValue("@StockQty", med.StockQty);
            cmd.Parameters.AddWithValue("@Price", med.Price);
            cmd.Parameters.AddWithValue("@ExpiredDate", med.ExpiredDate);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Update(Medicine med)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_UpdateMedicine", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", med.Id);
            cmd.Parameters.AddWithValue("@MedicineName", med.MedicineName);
            cmd.Parameters.AddWithValue("@StockQty", med.StockQty);
            cmd.Parameters.AddWithValue("@Price", med.Price);
            cmd.Parameters.AddWithValue("@ExpiredDate", med.ExpiredDate);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(long id)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_DeleteMedicine", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        private static Medicine MapMedicine(SqlDataReader reader)
        {
            return new Medicine
            {
                Id = reader.GetInt64("Id"),
                MedicineName = reader.GetString("MedicineName"),
                StockQty = reader.GetInt32("StockQty"),
                Price = reader.GetDecimal("Price"),
                ExpiredDate = DateTime.SpecifyKind(reader.GetDateTime("ExpiredDate"), DateTimeKind.Utc),
                CreatedAt = DateTime.SpecifyKind(reader.GetDateTime("CreatedAt"), DateTimeKind.Utc),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : DateTime.SpecifyKind(reader.GetDateTime("UpdatedAt"), DateTimeKind.Utc)
            };
        }
    }
}
