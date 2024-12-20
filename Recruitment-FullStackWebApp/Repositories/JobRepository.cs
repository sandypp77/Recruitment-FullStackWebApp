using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;
using System;

namespace Recruitment_FullStackWebApp.Repositories
{
    /// <summary>
    /// Repository for managing job-related operations, including CRUD operations and job applications.
    /// </summary>
    public class JobRepository : IJobRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRepository"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="mapper">The object mapper for DTO conversions.</param>
        public JobRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of jobs filtered by recruiter, title, and location.
        /// </summary>
        /// <param name="recruiterId">The ID of the recruiter.</param>
        /// <param name="title">The title filter for the job.</param>
        /// <param name="location">The location filter for the job.</param>
        /// <returns>A list of <see cref="Job"/> objects.</returns>
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

        /// <summary>
        /// Retrieves all jobs from the database.
        /// </summary>
        /// <returns>A list of all <see cref="Job"/> objects.</returns>
        public IList<Job> GetAllJobs()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Jobs ORDER BY CreatedAt DESC";
            return (IList<Job>)connection.Query<Job>(sql, new { });
        }

        /// <summary>
        /// Retrieves paginated jobs filtered by title and location.
        /// </summary>
        /// <param name="title">The title filter for jobs.</param>
        /// <param name="location">The location filter for jobs.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A <see cref="PaginationJobDto"/> containing the paginated jobs.</returns>
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

        /// <summary>
        /// Retrieves a job by its unique identifier.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>The <see cref="Job"/> object if found; otherwise, null.</returns>
        public Job GetById(int jobId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Jobs WHERE Id = @JobId";
            return connection.QueryFirstOrDefault<Job>(sql, new { JobId = jobId });
        }

        /// <summary>
        /// Adds a new job to the database.
        /// </summary>
        /// <param name="job">The job entity to add.</param>
        /// <returns>The unique identifier of the newly created job.</returns>
        public int Add(Job job)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            INSERT INTO Jobs (Title, Description, Location, Salary, JobTypeId, RecruiterId)
            VALUES (@Title, @Description, @Location, @Salary, @JobTypeId, @RecruiterId);
            SELECT CAST(SCOPE_IDENTITY() as int)";
            return connection.ExecuteScalar<int>(sql, job);
        }

        /// <summary>
        /// Updates an existing job in the database.
        /// </summary>
        /// <param name="job">The job entity with updated values.</param>
        public void Update(Job job)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            UPDATE Jobs
            SET Title = @Title,
                Description = @Description,
                Location = @Location,
                Salary = @Salary,
                JobTypeId = @JobTypeId
            WHERE Id = @Id AND RecruiterId = @RecruiterId";
            connection.Execute(sql, job);
        }

        /// <summary>
        /// Deletes a job from the database by its ID.
        /// </summary>
        /// <param name="jobId">The ID of the job to delete.</param>
        public void Delete(int jobId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "DELETE FROM Jobs WHERE Id = @JobId";
            connection.Execute(sql, new { JobId = jobId });
        }

        /// <summary>
        /// Retrieves all job types from the database, ordered by their creation date.
        /// </summary>
        /// <returns>A list of all job types.</returns>
        public IList<JobType> GetAllJobTypes()
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobTypes ORDER BY CreatedAt DESC";
            return (IList<JobType>)connection.Query<JobType>(sql, new { });
        }

        /// <summary>
        /// Retrieves a job type by its unique ID.
        /// </summary>
        /// <param name="jobTypeId">The ID of the job type to retrieve.</param>
        /// <returns>The job type entity, or null if not found.</returns>
        public JobType GetJobTypeById(int jobTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobTypes WHERE Id = @JobTypeId";
            return connection.QueryFirstOrDefault<JobType>(sql, new { JobTypeId = jobTypeId });
        }

        /// <summary>
        /// Adds a new job type to the database.
        /// </summary>
        /// <param name="jobType">The job type entity to add.</param>
        /// <returns>The unique identifier of the newly created job type.</returns>
        public int AddJobType(JobType jobType)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            INSERT INTO JobTypes (Name, Description)
            VALUES (@Name, @Description);
            SELECT CAST(SCOPE_IDENTITY() as int)";
            return connection.ExecuteScalar<int>(sql, jobType);
        }

        /// <summary>
        /// Updates an existing job type in the database.
        /// </summary>
        /// <param name="jobType">The job type entity with updated values.</param>
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

        /// <summary>
        /// Deletes a job type from the database by its ID.
        /// </summary>
        /// <param name="jobTypeId">The ID of the job type to delete.</param>
        public void DeleteJobType(int jobTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "DELETE FROM JobTypes WHERE Id = @JobTypeId";
            connection.Execute(sql, new { JobTypeId = jobTypeId });
        }

        /// <summary>
        /// Adds a job application to the database.
        /// </summary>
        /// <param name="jobApplication">The job application entity to add.</param>
        /// <returns>The unique identifier of the newly created job application.</returns>
        public int ApplyForJob(JobApplication jobApplication)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            INSERT INTO JobApplications (JobId, ApplicantId, ApplicationDate, Status)
            VALUES (@JobId, @ApplicantId, @ApplicationDate, @Status);
            SELECT CAST(SCOPE_IDENTITY() as int);";
            return connection.ExecuteScalar<int>(sql, jobApplication);
        }

        /// <summary>
        /// Retrieves all job applications made by a specific applicant.
        /// </summary>
        /// <param name="applicantId">The ID of the applicant.</param>
        /// <returns>A list of job applications made by the applicant.</returns>
        public IList<JobApplication> GetApplicationsByApplicant(int applicantId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobApplications WHERE ApplicantId = @ApplicantId";
            return (IList<JobApplication>)connection.QueryFirstOrDefault<JobApplication>(sql, new { ApplicantId = applicantId });
        }

        /// <summary>
        /// Retrieves a paginated list of jobs applied for by an applicant, with filtering by title and location.
        /// </summary>
        /// <param name="title">The job title to filter by (optional).</param>
        /// <param name="location">The job location to filter by (optional).</param>
        /// <param name="applicantId">The ID of the applicant.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>A paginated DTO containing jobs applied for by the applicant.</returns>
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
                    j.JobTypeId,
                    jt.Name AS JobTypeName, 
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
                    JobTypes jt ON j.JobTypeId = jt.Id
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

        /// <summary>
        /// Retrieves a paginated list of applicants for a specific job.
        /// </summary>
        /// <param name="jobId">The ID of the job.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>A paginated DTO containing job applicants.</returns>
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
                    j.JobTypeId,
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

        /// <summary>
        /// Updates the status of a job application in the database.
        /// </summary>
        /// <param name="jobApplication">The job application entity with the updated status.</param>
        public void UpdateJobApplication(JobApplication jobApplication)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"
            UPDATE JobApplications
            SET Status = @Status
            WHERE Id = @Id";
            connection.Execute(sql, jobApplication);
        }

        /// <summary>
        /// Retrieves a job application by its unique ID.
        /// </summary>
        /// <param name="jobApplicationId">The ID of the job application to retrieve.</param>
        /// <returns>The job application entity, or null if not found.</returns>
        public JobApplication GetJobApplicationById(int jobApplicationId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM JobApplications WHERE Id = @Id";
            return connection.QueryFirstOrDefault<JobApplication>(sql, new { Id = jobApplicationId });
        }
    }

}
