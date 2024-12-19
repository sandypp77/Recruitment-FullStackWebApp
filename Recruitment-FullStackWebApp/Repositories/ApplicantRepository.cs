using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Recruitment_FullStackWebApp.Models;

namespace Recruitment_FullStackWebApp.Repositories
{
    public class ApplicantRepository : IApplicantRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public ApplicantRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString;
            _mapper = mapper;
        }

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

        public Applicant GetApplicantProfile(int applicantId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Applicants WHERE ApplicantId = @ApplicantId ";
            return connection.QueryFirstOrDefault<Applicant>(sql, new { ApplicantId = applicantId });
        }

    }
}
