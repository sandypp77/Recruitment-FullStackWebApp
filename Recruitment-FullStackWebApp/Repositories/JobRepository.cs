using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;
using System;

namespace Recruitment_FullStackWebApp.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public JobRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString;
            _mapper = mapper;
        }

        public IList<Job> GetJobsByRecruiter(int recruiterId, string title, string location)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Jobs WHERE RecruiterId = @RecruiterId";
            if (!string.IsNullOrEmpty(title))
                sql += " AND Title LIKE @Title";

            if (!string.IsNullOrEmpty(location))
                sql += " AND Location LIKE @Location";

            sql += " ORDER BY CreatedAt DESC";
            return (IList<Job>)connection.Query<Job>(sql, new { RecruiterId = recruiterId, Title = $"%{title}%", Location = $"%{location}%" });
        }

        public IList<Job> GetAllJobs()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Jobs ORDER BY CreatedAt DESC";
            return (IList<Job>)connection.Query<Job>(sql, new { });
        }

        public PaginationJobDto GetAllJobsPagination(string title, string location, int pageNumber, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT COUNT(*) FROM Jobs 
                WHERE 1 = 1";

            if (!string.IsNullOrEmpty(title))
                query += " AND Title LIKE @Title";

            if (!string.IsNullOrEmpty(location))
                query += " AND Location LIKE @Location";

            query += @"
                ; 
                SELECT * FROM Jobs 
                WHERE 1 = 1";

            if (!string.IsNullOrEmpty(title))
                query += " AND Title LIKE @Title";

            if (!string.IsNullOrEmpty(location))
                query += " AND Location LIKE @Location";

            query += @"
                ORDER BY CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            using var multi = connection.QueryMultiple(query, new
            {
                Title = $"%{title}%",
                Location = $"%{location}%",
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });

            var totalCount = multi.ReadSingle<int>();
            var jobs = multi.Read<JobDto>().ToList();

            return new PaginationJobDto
            {
                Items = jobs,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public Job GetById(int jobId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Jobs WHERE Id = @JobId";
            return connection.QueryFirstOrDefault<Job>(sql, new { JobId = jobId });
        }

        public int Add(Job job)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            INSERT INTO Jobs (Title, Description, Location, Salary, Category, RecruiterId)
            VALUES (@Title, @Description, @Location, @Salary, @Category, @RecruiterId);
            SELECT CAST(SCOPE_IDENTITY() as int)";
            return connection.ExecuteScalar<int>(sql, job);
        }

        public void Update(Job job)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            UPDATE Jobs
            SET Title = @Title,
                Description = @Description,
                Location = @Location,
                Salary = @Salary,
                Category = @Category
            WHERE Id = @Id AND RecruiterId = @RecruiterId";
            connection.Execute(sql, job);
        }

        public void Delete(int jobId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "DELETE FROM Jobs WHERE Id = @JobId";
            connection.Execute(sql, new { JobId = jobId });
        }

        public IList<JobType> GetAllJobTypes()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobTypes ORDER BY CreatedAt DESC";
            return (IList<JobType>)connection.Query<JobType>(sql, new { });
        }

        public JobType GetJobTypeById(int jobTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobTypes WHERE Id = @JobTypeId";
            return connection.QueryFirstOrDefault<JobType>(sql, new { JobTypeId = jobTypeId });
        }

        public int AddJobType(JobType jobType)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            INSERT INTO JobTypes (Name, Description)
            VALUES (@Name, @Description);
            SELECT CAST(SCOPE_IDENTITY() as int)";
            return connection.ExecuteScalar<int>(sql, jobType);
        }

        public void UpdateJobType(JobType jobType)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            UPDATE JobTypes
            SET Name = @Name,
                Description = @Description
            WHERE Id = @Id";
            connection.Execute(sql, jobType);
        }

        public void DeleteJobType(int jobTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "DELETE FROM JobTypes WHERE Id = @JobTypeId";
            connection.Execute(sql, new { JobTypeId = jobTypeId });
        }

        public int ApplyForJob(JobApplication jobApplication)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            INSERT INTO JobApplications (JobId, ApplicantId, ApplicationDate, Status)
            VALUES (@JobId, @ApplicantId, @ApplicationDate, @Status);
            SELECT CAST(SCOPE_IDENTITY() as int);";
            return connection.ExecuteScalar<int>(sql, jobApplication);
        }

        public IList<JobApplication> GetApplicationsByApplicant(int applicantId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobApplications WHERE ApplicantId = @ApplicantId";
            return (IList<JobApplication>)connection.QueryFirstOrDefault<JobApplication>(sql, new { ApplicantId = applicantId });
        }

        public PaginationJobAppliedDto GetJobsAppliedByApplicant(string title, string location, int applicantId, int pageNumber, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT 
                    COUNT(*) 
                FROM 
                    JobApplications ja
                LEFT JOIN 
                    Jobs j ON j.Id = ja.JobId
                WHERE 
                    ja.ApplicantId = @ApplicantId";

            if (!string.IsNullOrEmpty(title))
                query += " AND j.Title LIKE @Title";

            if (!string.IsNullOrEmpty(location))
                query += " AND j.Location LIKE @Location";

            query += @";
                SELECT 
                    j.Id AS JobId,
                    j.Title,
                    j.Description,
                    j.Location,
                    j.Salary,
                    j.Category,
                    ja.Id AS JobApplicationId,
                    ja.ApplicantId,
                    ja.ApplicationDate,
                    ja.Status,
                    a.FullName AS ApplicantName,
                    a.Phone AS ApplicantPhone,
                    a.Address AS ApplicantAddress,
                    a.ResumeUrl,
                    a.Skills,
                    a.DateOfBirth
                FROM 
                    Jobs j
                LEFT JOIN 
                    JobApplications ja ON j.Id = ja.JobId
                LEFT JOIN 
                    Applicants a ON ja.ApplicantId = a.ApplicantId
                WHERE 
                    ja.ApplicantId = @ApplicantId";

            if (!string.IsNullOrEmpty(title))
                query += " AND j.Title LIKE @Title";

            if (!string.IsNullOrEmpty(location))
                query += " AND j.Location LIKE @Location";

            query += @"
                ORDER BY 
                    ja.ApplicationDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var offset = (pageNumber - 1) * pageSize;

            using var multi = connection.QueryMultiple(query, new
            {
                ApplicantId = applicantId,
                Title = $"%{title}%",
                Location = $"%{location}%",
                Offset = offset,
                PageSize = pageSize
            });

            // Fetch total count and paginated jobs
            var totalCount = multi.ReadSingle<int>();
            var jobs = multi.Read<JobWithApplicantsDto>().ToList();

            return new PaginationJobAppliedDto
            {
                Items = jobs,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };


        }

        public PaginationJobAppliedDto GetJobApplicants(int jobId, int pageNumber, int pageSize)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT 
                    COUNT(*) 
                FROM 
                    JobApplications ja
                WHERE 
                    ja.JobId = @JobId;
                SELECT 
                    j.Id AS JobId,
                    j.Title,
                    j.Description,
                    j.Location,
                    j.Salary,
                    j.Category,
                    ja.Id AS JobApplicationId,
                    ja.ApplicantId,
                    ja.ApplicationDate,
                    ja.Status,
                    a.FullName AS ApplicantName,
                    a.Phone AS ApplicantPhone,
                    a.Address AS ApplicantAddress,
                    a.ResumeUrl,
                    a.Skills,
                    a.DateOfBirth
                FROM 
                    Jobs j
                LEFT JOIN 
                    JobApplications ja ON j.Id = ja.JobId
                LEFT JOIN 
                    Applicants a ON ja.ApplicantId = a.ApplicantId
                WHERE 
                    ja.JobId = @JobId
                ORDER BY 
                    ja.ApplicationDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var offset = (pageNumber - 1) * pageSize;

            using var multi = connection.QueryMultiple(sql, new
            {
                JobId = jobId,
                Offset = offset,
                PageSize = pageSize
            });

            // Fetch total count and paginated jobs
            var totalCount = multi.ReadSingle<int>();
            var applicants = multi.Read<JobWithApplicantsDto>().ToList();

            return new PaginationJobAppliedDto
            {
                Items = applicants,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public void UpdateJobApplication(JobApplication jobApplication)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            UPDATE JobApplications
            SET Status = @Status
            WHERE Id = @Id";
            connection.Execute(sql, jobApplication);
        }

        public JobApplication GetJobApplicationById(int jobApplicationId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobApplications WHERE Id = @Id";
            return connection.QueryFirstOrDefault<JobApplication>(sql, new { Id = jobApplicationId });
        }
    }

}
