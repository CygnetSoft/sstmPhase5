using AutoMapper;
using SSTM.Core;
using SSTM.Core.ActivityLog;
using SSTM.Core.Assessment_Paper;
using SSTM.Core.Central_CourseSharing;
using SSTM.Core.Centralized_Course;
using SSTM.Core.Config;
using SSTM.Core.Course;
using SSTM.Core.Course_Reminder;
using SSTM.Core.CourseAssignment;
using SSTM.Core.CourseDocRemarks;
using SSTM.Core.CourseDocument;
using SSTM.Core.CourseDocVersion;
using SSTM.Core.CourseDownload;
using SSTM.Core.CourseSharing;
using SSTM.Core.CourseStatus;
using SSTM.Core.CourseTrackers;
using SSTM.Core.Developer_Monitor_Timer;
using SSTM.Core.ExceptionLog;
using SSTM.Core.IntroPage;
using SSTM.Core.MainCourse;
using SSTM.Core.Role;
using SSTM.Core.SubCourse;
using SSTM.Core.TrainerQPUpload;
using SSTM.Core.TrainerUploadDocument;
using SSTM.Core.TrainingCenter;
using SSTM.Core.TrainnerMacAddress;
using SSTM.Core.User;
using SSTM.Models.ActivityLog;
using SSTM.Models.Assessment_Paper;
using SSTM.Models.Central_CourseSharing;
using SSTM.Models.Centralized_Course;
using SSTM.Models.Config;
using SSTM.Models.Course;
using SSTM.Models.Course_Reminder;
using SSTM.Models.CourseAssignment;
using SSTM.Models.CourseDocRemarks;
using SSTM.Models.CourseDocument;
using SSTM.Models.CourseDocVersion;
using SSTM.Models.CourseDownload;
using SSTM.Models.CourseSharing;
using SSTM.Models.CourseStatus;
using SSTM.Models.CourseTrackers;
using SSTM.Models.Developer_Monitor_Timer;
using SSTM.Models.ExceptionLog;
using SSTM.Models.MainCourseModel;
using SSTM.Models.QPRequest;
using SSTM.Models.Report;
using SSTM.Models.Role;
using SSTM.Models.StudentNotification;
using SSTM.Models.SubCourse;
using SSTM.Models.TrainerQPUpload;
using SSTM.Models.TrainerUploadDocumentModel;
using SSTM.Models.TrainingCenter;
using SSTM.Models.TrainnerMacAddress;
using SSTM.Models.User;

namespace SSTM.Helpers.AutoMapping
{
    public static class AutoMapperStartupTask
    {
        public static void Execute()
        {
            #region Activity Log
            Mapper.CreateMap<ActivityLogModel, ActivityLog>();

            Mapper.CreateMap<ActivityLog, ActivityLogModel>();
            #endregion

            #region Config
            Mapper.CreateMap<ConfigModel, Config>();

            Mapper.CreateMap<Config, ConfigModel>();
            #endregion

            #region Course
            Mapper.CreateMap<CourseModel, Course>();

            Mapper.CreateMap<Course, CourseModel>();
            #endregion

            #region Course Document
            Mapper.CreateMap<CourseDocumentModel, CourseDocument>();

            Mapper.CreateMap<CourseDocument, CourseDocumentModel>();
            #endregion

            #region CourseStatus
            Mapper.CreateMap<CourseStatusModel, CourseStatus>();

            Mapper.CreateMap<CourseStatus, CourseStatusModel>();
            #endregion

            #region Exception Log
            Mapper.CreateMap<ExceptionLogModel, ExceptionLog>();

            Mapper.CreateMap<ExceptionLog, ExceptionLogModel>();
            #endregion

            #region Role
            Mapper.CreateMap<RoleModel, Role>();

            Mapper.CreateMap<Role, RoleModel>();
            #endregion

            #region Training Center
            Mapper.CreateMap<TrainingCenterModel, TrainingCenter>();

            Mapper.CreateMap<TrainingCenter, TrainingCenterModel>();
            #endregion

            #region User
            Mapper.CreateMap<UserModel, User>();

            Mapper.CreateMap<User, UserModel>();
            #endregion

            #region Course Doc Remarks
            Mapper.CreateMap<CourseDocRemarksModel, CourseDocRemarks>();

            Mapper.CreateMap<CourseDocRemarks, CourseDocRemarksModel>();
            #endregion

            #region Course Sharing
            Mapper.CreateMap<CourseSharingModel, CourseSharing>();

            Mapper.CreateMap<CourseSharing, CourseSharingModel>();
            #endregion

            #region Course Assignment
            Mapper.CreateMap<CourseAssignmentModel, CourseAssignment>();

            Mapper.CreateMap<CourseAssignment, CourseAssignmentModel>();
            #endregion

            #region Course Document Version
            Mapper.CreateMap<CourseDocVersionModel, CourseDocVersion>();

            Mapper.CreateMap<CourseDocVersion, CourseDocVersionModel>();
            #endregion

            #region Main Course
            Mapper.CreateMap<MainCourseModel, MainCourse>();

            Mapper.CreateMap<MainCourse, MainCourseModel>();
            #endregion

            #region Sub Course
            Mapper.CreateMap<SubCourseModel, SubCourse>();

            Mapper.CreateMap<SubCourse, SubCourseModel>();
            #endregion

            #region Course Trackers
            Mapper.CreateMap<CourseTrackersModel, CourseTrackers>();

            Mapper.CreateMap<CourseTrackers, CourseTrackersModel>();
            #endregion

            #region Course Download
            Mapper.CreateMap<CourseDownloadUserModel, CourseDownloadUser>();

            Mapper.CreateMap<CourseDownloadUser, CourseDownloadUserModel>();
            #endregion
            #region Trainer Upload Document
            Mapper.CreateMap<TrainerUploadDocumentModel, TrainerUploadDocument>();

            Mapper.CreateMap<TrainerUploadDocument, TrainerUploadDocumentModel>();
            #endregion

            #region Trainner Mac Address
            Mapper.CreateMap<TrainnerMacAddressModel, TrainnerMacAddress>();

            Mapper.CreateMap<TrainnerMacAddress, TrainnerMacAddressModel>();
            #endregion
            #region Trainer QP Upload
            Mapper.CreateMap<TrainerQPUploadModel, TrainerQPUpload>();

            Mapper.CreateMap<TrainerQPUpload, TrainerQPUploadModel>();
            #endregion
            #region TrainerQP Shared Student
            Mapper.CreateMap<TrainerQP_Shared_StudentModel, TrainerQP_Shared_Student>();

            Mapper.CreateMap<TrainerQP_Shared_Student, TrainerQP_Shared_StudentModel>();
            #endregion
            #region TrainerQP Level Approval
            Mapper.CreateMap<TrainerQP_Level_ApprovalModel, TrainerQP_Level_Approval>();

            Mapper.CreateMap<TrainerQP_Level_Approval, TrainerQP_Level_ApprovalModel>();
            #endregion
            #region  Course Reminder
            Mapper.CreateMap<CourseReminderModel,CourseReminder >();

            Mapper.CreateMap<CourseReminder, CourseReminderModel>();
            #endregion

            #region  Course Renewal
            Mapper.CreateMap<CourseRenewalModel, CourseRenewal>();

            Mapper.CreateMap<CourseRenewal, CourseRenewalModel>();
            #endregion

            #region  Course Reminder Latter Undertaking
            Mapper.CreateMap<Course_Reminder_Latter_UndertakingModel, Course_Reminder_Latter_Undertaking>();

            Mapper.CreateMap<Course_Reminder_Latter_Undertaking, Course_Reminder_Latter_UndertakingModel>();
            #endregion

            #region  New Course tracker
            Mapper.CreateMap<NewCourseTrackingDataModel, NewCourseTrackingData>();

            Mapper.CreateMap<NewCourseTrackingData, NewCourseTrackingDataModel>();
            #endregion

            #region  QP Request
            Mapper.CreateMap<QPRequestModel, QPRequest>();

            Mapper.CreateMap<QPRequest, QPRequestModel>();
            #endregion

            #region Report
            Mapper.CreateMap<ReportModel, Report>();

            Mapper.CreateMap<Report, ReportModel>();
            #endregion

            #region StudentNotification

            Mapper.CreateMap<StudentNotificationModel, StudentNotification>();

            Mapper.CreateMap<StudentNotification, StudentNotificationModel>();

            #endregion

            #region StudentNotification

            Mapper.CreateMap<Models.RiskAssessmentDeclarationModel, RiskAssessmentDeclaration>();

            Mapper.CreateMap<RiskAssessmentDeclaration, Models.RiskAssessmentDeclarationModel>();

            #endregion

            #region Assessment Paper

            Mapper.CreateMap<AssessmentPaperModel, AssessmentPaper>();

            Mapper.CreateMap<AssessmentPaper, AssessmentPaperModel>();

            #endregion

            #region DeveloperMonitorTimer

            Mapper.CreateMap<DeveloperMonitorTimerModel, DeveloperMonitorTimer>();

            Mapper.CreateMap<DeveloperMonitorTimer, DeveloperMonitorTimerModel>();

            #endregion


            #region Centralized_Master

            Mapper.CreateMap<Centralized_MasterModel, Centralized_Master>();

            Mapper.CreateMap<Centralized_Master, Centralized_MasterModel>();

            #endregion

            #region Centralized_HistoryModel

            Mapper.CreateMap<Centralized_HistoryModel, Centralized_History>();

            Mapper.CreateMap<Centralized_History, Centralized_HistoryModel>();

            #endregion

            #region Centralized_CourseModel

            Mapper.CreateMap<Centralized_CourseModel, Centralized_Course>();

            Mapper.CreateMap<Centralized_Course, Centralized_CourseModel>();

            #endregion

            #region Centralized_Document_filesModel

            Mapper.CreateMap<Centralized_Document_filesModel, Centralized_Document_files>();

            Mapper.CreateMap<Centralized_Document_files, Centralized_Document_filesModel>();

            #endregion

            

             #region Central_CourseSharingModel

            Mapper.CreateMap<Central_CourseSharingModel, Central_CourseSharing>();

            Mapper.CreateMap<Central_CourseSharing, Central_CourseSharingModel>();

            #endregion

        }
    }
}