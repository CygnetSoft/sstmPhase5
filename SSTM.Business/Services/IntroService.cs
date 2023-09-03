using SSTM.Business.Interfaces;
using SSTM.Core.IntroPage;
using SSTM.Data.Infrastructure;
using SSTM.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class IntroService : RepositoryBase<StudentQP>, IIntroService
    {
        public IntroService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        /// <summary>
        /// Add Student Model Multiple Choice Question.
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public string AddStudentMultipleChoiceQp(List<StudentQP> student)
        {
            try
            {
                string response = string.Empty;
                ExecuteSqlCommand("delete from [sstmo].[StudentQP] where ChapterId = " + student.First().ChapterId + " and CourseId = " + student.First().CourseId + "");
                student.ForEach(entity =>
                {
                    entity.Qp_Doc_Name = !string.IsNullOrEmpty(entity.Qp_Doc_Name) ? UtilityHelper.Encrypt(entity.Qp_Doc_Name) : null;
                    entity.Choice_A_Url = !string.IsNullOrEmpty(entity.Choice_A_Url) ? UtilityHelper.Encrypt(entity.Choice_A_Url) : null;
                    entity.Choice_B_Url = !string.IsNullOrEmpty(entity.Choice_B_Url) ? UtilityHelper.Encrypt(entity.Choice_B_Url) : null;
                    entity.Choice_C_Url = !string.IsNullOrEmpty(entity.Choice_C_Url) ? UtilityHelper.Encrypt(entity.Choice_C_Url) : null;
                    entity.Choice_D_Url = !string.IsNullOrEmpty(entity.Choice_D_Url) ? UtilityHelper.Encrypt(entity.Choice_D_Url) : null;
                    entity.Choice_A = !string.IsNullOrEmpty(entity.Choice_A) ? entity.Choice_A : null;
                    entity.Choice_B = !string.IsNullOrEmpty(entity.Choice_B) ? entity.Choice_B : null;
                    entity.Choice_C = !string.IsNullOrEmpty(entity.Choice_C) ? entity.Choice_C : null;
                    entity.Choice_D = !string.IsNullOrEmpty(entity.Choice_D) ? entity.Choice_D : null;
                    entity.CreatedOn = DateTime.Now;
                    Add(entity);
                    response = "Added";
                });
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<StudentQP>> GetStudentMCQ(long courseId, long chapterId)
        {
            try
            {
                List<StudentQP> studentQp = await Table.Where(x => x.IsActive && x.CourseId == courseId && x.ChapterId == chapterId).ToListAsync();
                studentQp.ToList().ForEach(x =>
                {
                    x.Qp_Doc_Name = (bool)x.IsQp ? string.Concat(UtilityHelper.amazonlinkUrl, "StudentMCQPapers/", UtilityHelper.Decrypt(x.Qp_Doc_Name)) : null;
                    x.Choice_A_Url = x.Is_Url_Choice_A ? string.Concat(UtilityHelper.amazonlinkUrl, "StudentMCQPapers/", UtilityHelper.Decrypt(x.Choice_A_Url)) : null;
                    x.Choice_B_Url = x.Is_Url_Choice_B ? string.Concat(UtilityHelper.amazonlinkUrl, "StudentMCQPapers/", UtilityHelper.Decrypt(x.Choice_B_Url)) : null;
                    x.Choice_C_Url = x.Is_Url_Choice_C ? string.Concat(UtilityHelper.amazonlinkUrl, "StudentMCQPapers/", UtilityHelper.Decrypt(x.Choice_C_Url)) : null;
                    x.Choice_D_Url = x.Is_Url_Choice_D ? string.Concat(UtilityHelper.amazonlinkUrl, "StudentMCQPapers/", UtilityHelper.Decrypt(x.Choice_D_Url)) : null;
                });
                return studentQp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<object> GetAllCourse()
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(string.Concat(ConfigurationManager.AppSettings["li_ApiServices"], "AllCourseJSON"))
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(string.Empty).Result;
                if (response.IsSuccessStatusCode)
                {
                    string dataObjects = await response.Content.ReadAsStringAsync();
                    string couseJson = dataObjects.ToString();
                    StringBuilder array_jsonFromat = new StringBuilder();
                    array_jsonFromat.Append("[");
                    array_jsonFromat.Append(couseJson);
                    array_jsonFromat.Append("]");
                    return new { Status = (int)response.StatusCode, Result = array_jsonFromat.ToString() };
                }
                else
                {
                    return new { Status = (int)response.StatusCode, Result = string.Empty };
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
