using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Recruitment_FullStackWebApp.Models;

namespace Recruitment_FullStackWebApp.Repositories
{
    /// <summary>
    /// Repository class for managing applicant profiles.
    /// </summary>

    public class ApplicantRepository : IApplicantRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicantRepository"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public ApplicantRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new applicant profile in the database.
        /// </summary>
        /// <param name="applicant">The applicant object containing the profile details.</param>
        /// <returns>The ID of the newly created applicant profile.</returns>
        public async Task<int> CreateProfile(Applicant applicant)
        {
            using var connection = new SqlConnection(_connectionString);
            const string query = @"
            INSERT INTO Applicants 
            (FullName, Phone, Address, ResumeUrl, Skills, DateOfBirth, ApplicantId)
            VALUES 
            (@FullName, @Phone, @Address, @ResumeUrl, @Skills, @DateOfBirth, @ApplicantId);
            SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(query, applicant);
        }

        /// <summary>
        /// Updates an existing applicant profile in the database.
        /// </summary>
        /// <param name="applicant">The applicant object containing the updated profile details.</param>
        /// <returns>The updated applicant object.</returns>
        public async Task<Applicant> UpdateProfile(Applicant applicant)
        {
            using var connection = new SqlConnection(_connectionString);
            const string query = @"
            UPDATE Applicants
            SET FullName = @FullName,
                Phone = @Phone,
                Address = @Address,
                ResumeUrl = @ResumeUrl,
                Skills = @Skills,
                DateOfBirth = @DateOfBirth
            WHERE Id = @Id";

            await connection.ExecuteAsync(query, new
            {
                applicant.FullName,
                applicant.Phone,
                applicant.Address,
                applicant.ResumeUrl,
                applicant.Skills,
                applicant.DateOfBirth,
                applicant.Id
            });

            return applicant;
        }

        /// <summary>
        /// Retrieves an applicant profile by ID.
        /// </summary>
        /// <param name="applicantId">The ID of the applicant.</param>
        /// <returns>
        public Applicant GetApplicantProfile(int applicantId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Applicants WHERE ApplicantId = @ApplicantId ";
            return connection.QueryFirstOrDefault<Applicant>(sql, new { ApplicantId = applicantId });
        }

    }
}
