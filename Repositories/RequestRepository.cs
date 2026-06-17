using Microsoft.Data.SqlClient;
using InitialSetupMVC.Data;
using InitialSetupMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace InitialSetupMVC.Repositories
{
    public class RequestRepository
    {
        private readonly DbConnection _db;

        public RequestRepository(DbConnection db)
        {
            _db = db;
        }

        public List<Request> GetAll()
        {
            var list = new List<Request>();
            using (var conn = _db.CreateConnection())
            {
                using var cmd = new SqlCommand("sp_GetAllRequests", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapRequestHeader(reader));
                }
            }

            foreach (var req in list)
            {
                using var conn = _db.CreateConnection();
                conn.Open();
                req.Details = GetDetailsByRequestId(req.Id, conn);
                req.Logs = GetLogsByRequestId(req.Id, conn);
            }

            return list;
        }

        public Request? GetById(long id)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetRequestById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var req = MapRequestHeader(reader);
                reader.Close();
                req.Details = GetDetailsByRequestId(req.Id, conn);
                req.Logs = GetLogsByRequestId(req.Id, conn);
                return req;
            }
            return null;
        }

        public void Create(Request req, ApprovalLog initialLog)
        {
            var detailsTable = new DataTable();
            detailsTable.Columns.Add("MedicineId", typeof(long));
            detailsTable.Columns.Add("Qty", typeof(int));
            detailsTable.Columns.Add("Price", typeof(decimal));

            foreach (var detail in req.Details)
            {
                detailsTable.Rows.Add(detail.MedicineId, detail.Qty, detail.Price);
            }

            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_CreateRequest", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RequestNumber", req.RequestNumber);
            cmd.Parameters.AddWithValue("@UserId", req.UserId);
            cmd.Parameters.AddWithValue("@Status", req.Status);
            cmd.Parameters.AddWithValue("@ActionBy", initialLog.ActionBy);
            cmd.Parameters.AddWithValue("@ActionType", initialLog.ActionType);
            cmd.Parameters.AddWithValue("@Remarks", initialLog.Remarks ?? (object)DBNull.Value);

            var detailsParam = cmd.Parameters.AddWithValue("@Details", detailsTable);
            detailsParam.SqlDbType = SqlDbType.Structured;
            detailsParam.TypeName = "RequestDetailType";

            conn.Open();
            req.Id = Convert.ToInt64(cmd.ExecuteScalar());
        }

        public void UpdateStatus(long requestId, string status, DateTime? adminApprovedAt, DateTime? distApprovedAt, DateTime? deliveredAt, ApprovalLog log)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_UpdateRequestStatus", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@AdminApprovedAt", (object?)adminApprovedAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistributionApprovedAt", (object?)distApprovedAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DeliveredAt", (object?)deliveredAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ActionBy", log.ActionBy);
            cmd.Parameters.AddWithValue("@ActionType", log.ActionType);
            cmd.Parameters.AddWithValue("@Remarks", log.Remarks ?? (object)DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateStatusAndDeductStock(long requestId, string status, DateTime? distApprovedAt, List<RequestDetail> details, ApprovalLog log)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_UpdateRequestStatusAndDeductStock", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@DistributionApprovedAt", (object?)distApprovedAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ActionBy", log.ActionBy);
            cmd.Parameters.AddWithValue("@ActionType", log.ActionType);
            cmd.Parameters.AddWithValue("@Remarks", log.Remarks ?? (object)DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<ApprovalLog> GetAllLogs()
        {
            var list = new List<ApprovalLog>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetAllApprovalLogs", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapApprovalLog(reader));
            }
            return list;
        }

        private List<RequestDetail> GetDetailsByRequestId(long requestId, SqlConnection conn)
        {
            var details = new List<RequestDetail>();
            using var cmd = new SqlCommand("sp_GetRequestDetailsByRequestId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                details.Add(new RequestDetail
                {
                    Id = reader.GetInt64("Id"),
                    RequestId = reader.GetInt64("RequestId"),
                    MedicineId = reader.GetInt64("MedicineId"),
                    MedicineName = reader.GetString("MedicineName"),
                    Qty = reader.GetInt32("Qty"),
                    Price = reader.GetDecimal("Price")
                });
            }
            return details;
        }

        private List<ApprovalLog> GetLogsByRequestId(long requestId, SqlConnection conn)
        {
            var logs = new List<ApprovalLog>();
            using var cmd = new SqlCommand("sp_GetApprovalLogsByRequestId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                logs.Add(new ApprovalLog
                {
                    Id = reader.GetInt64("Id"),
                    RequestId = reader.GetInt64("RequestId"),
                    RequestNumber = reader.GetString("RequestNumber"),
                    ActionBy = reader.GetInt64("ActionBy"),
                    ActionByName = reader.GetString("ActionByName"),
                    ActionByRole = reader.GetString("ActionByRole"),
                    ActionType = reader.GetString("ActionType"),
                    Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? string.Empty : reader.GetString("Remarks"),
                    ActionDate = DateTime.SpecifyKind(reader.GetDateTime("ActionDate"), DateTimeKind.Utc)
                });
            }
            return logs;
        }

        private static Request MapRequestHeader(SqlDataReader reader)
        {
            return new Request
            {
                Id = reader.GetInt64("Id"),
                RequestNumber = reader.GetString("RequestNumber"),
                UserId = reader.GetInt64("UserId"),
                UserFullName = reader.GetString("UserFullName"),
                Status = reader.GetString("Status"),
                RequestDate = DateTime.SpecifyKind(reader.GetDateTime("RequestDate"), DateTimeKind.Utc),
                AdminApprovedAt = reader.IsDBNull(reader.GetOrdinal("AdminApprovedAt")) ? null : DateTime.SpecifyKind(reader.GetDateTime("AdminApprovedAt"), DateTimeKind.Utc),
                DistributionApprovedAt = reader.IsDBNull(reader.GetOrdinal("DistributionApprovedAt")) ? null : DateTime.SpecifyKind(reader.GetDateTime("DistributionApprovedAt"), DateTimeKind.Utc),
                DeliveredAt = reader.IsDBNull(reader.GetOrdinal("DeliveredAt")) ? null : DateTime.SpecifyKind(reader.GetDateTime("DeliveredAt"), DateTimeKind.Utc)
            };
        }

        private static ApprovalLog MapApprovalLog(SqlDataReader reader)
        {
            return new ApprovalLog
            {
                Id = reader.GetInt64("Id"),
                RequestId = reader.GetInt64("RequestId"),
                RequestNumber = reader.GetString("RequestNumber"),
                ActionBy = reader.GetInt64("ActionBy"),
                ActionByName = reader.GetString("ActionByName"),
                ActionByRole = reader.GetString("ActionByRole"),
                ActionType = reader.GetString("ActionType"),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? string.Empty : reader.GetString("Remarks"),
                ActionDate = DateTime.SpecifyKind(reader.GetDateTime("ActionDate"), DateTimeKind.Utc)
            };
        }
    }
}
