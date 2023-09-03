using AutoMapper;
using SSTM.Core.ActivityLog;
using SSTM.Core.Config;
using SSTM.Core.Course;
using SSTM.Core.CourseAssignment;
using SSTM.Core.CourseDocRemarks;
using SSTM.Core.CourseDocument;
using SSTM.Core.CourseDocVersion;
using SSTM.Core.CourseSharing;
using SSTM.Core.CourseStatus;
using SSTM.Core.ExceptionLog;
using SSTM.Core.MainCourse;
using SSTM.Core.Role;
using SSTM.Core.TrainingCenter;
using SSTM.Core.User;
using SSTM.Models.ActivityLog;
using SSTM.Models.Config;
using SSTM.Models.Course;
using SSTM.Models.CourseAssignment;
using SSTM.Models.CourseDocRemarks;
using SSTM.Models.CourseDocument;
using SSTM.Models.CourseDocVersion;
using SSTM.Models.CourseSharing;
using SSTM.Models.CourseStatus;
using SSTM.Models.ExceptionLog;
using SSTM.Models.Role;
using SSTM.Models.MainCourseModel;
using SSTM.Models.TrainingCenter;
using SSTM.Models.User;
using SSTM.Core.SubCourse;
using SSTM.Models.SubCourse;
using SSTM.Models.CourseTrackers;
using SSTM.Core.CourseTrackers;
using SSTM.Models.CourseDownload;
using SSTM.Core.CourseDownload;
using SSTM.Models.TrainerUploadDocumentModel;
using SSTM.Core.TrainerUploadDocument;
using SSTM.Models.TrainnerMacAddress;
using SSTM.Core.TrainnerMacAddress;
using SSTM.Models.TrainerQPUpload;
using SSTM.Core.TrainerQPUpload;
using SSTM.Models.Course_Reminder;
using SSTM.Core.Course_Reminder;
using SSTM.Models.QPRequest;
using SSTM.Core;
using SSTM.Models.Report;
using SSTM.Models.StudentNotification;
using SSTM.Models.IntroPage;
using SSTM.Core.Assessment_Paper;
using SSTM.Models.Assessment_Paper;
using SSTM.Models.Developer_Monitor_Timer;
using SSTM.Core.Developer_Monitor_Timer;
using SSTM.Models.Centralized_Course;
using SSTM.Core.Centralized_Course;
using SSTM.Models.Central_CourseSharing;
using SSTM.Core.Central_CourseSharing;

namespace SSTM.Helpers.AutoMapping
{
    public static class MappingExtensions
    {
        #region Activity Log
        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return Mapper.Map<ActivityLog, ActivityLogModel>(entity);
        }

        public static ActivityLog ToEntity(this ActivityLogModel model)
        {
            return Mapper.Map<ActivityLogModel, ActivityLog>(model);
        }
        #endregion

        #region Config
        public static ConfigModel ToModel(this Config entity)
        {
            return Mapper.Map<Config, ConfigModel>(entity);
        }

        public static Config ToEntity(this ConfigModel model)
        {
            return Mapper.Map<ConfigModel, Config>(model);
        }
        #endregion

        #region Course
        public static CourseModel ToModel(this Course entity)
        {
            return Mapper.Map<Course, CourseModel>(entity);
        }

        public static Course ToEntity(this CourseModel model)
        {
            return Mapper.Map<CourseModel, Course>(model);
        }
        #endregion

        #region Course Document
        public static CourseDocumentModel ToModel(this CourseDocument entity)
        {
            return Mapper.Map<CourseDocument, CourseDocumentModel>(entity);
        }

        public static CourseDocument ToEntity(this CourseDocumentModel model)
        {
            return Mapper.Map<CourseDocumentModel, CourseDocument>(model);
        }
        #endregion

        #region Course Status
        public static CourseStatusModel ToModel(this CourseStatus entity)
        {
            return Mapper.Map<CourseStatus, CourseStatusModel>(entity);
        }

        public static CourseStatus ToEntity(this CourseStatusModel model)
        {
            return Mapper.Map<CourseStatusModel, CourseStatus>(model);
        }
        #endregion

        #region Exception Log
        public static ExceptionLogModel ToModel(this ExceptionLog entity)
        {
            return Mapper.Map<ExceptionLog, ExceptionLogModel>(entity);
        }

        public static ExceptionLog ToEntity(this ExceptionLogModel model)
        {
            return Mapper.Map<ExceptionLogModel, ExceptionLog>(model);
        }
        #endregion

        #region Role
        public static RoleModel ToModel(this Role entity)
        {
            return Mapper.Map<Role, RoleModel>(entity);
        }

        public static Role ToEntity(this RoleModel model)
        {
            return Mapper.Map<RoleModel, Role>(model);
        }
        #endregion

        #region Training Center
        public static TrainingCenterModel ToModel(this TrainingCenter entity)
        {
            return Mapper.Map<TrainingCenter, TrainingCenterModel>(entity);
        }

        public static TrainingCenter ToEntity(this TrainingCenterModel model)
        {
            return Mapper.Map<TrainingCenterModel, TrainingCenter>(model);
        }
        #endregion

        #region User
        public static UserModel ToModel(this User entity)
        {
            return Mapper.Map<User, UserModel>(entity);
        }

        public static User ToEntity(this UserModel model)
        {
            return Mapper.Map<UserModel, User>(model);
        }
        #endregion

        #region Course Doc Remarks
        public static CourseDocRemarksModel ToModel(this CourseDocRemarks entity)
        {
            return Mapper.Map<CourseDocRemarks, CourseDocRemarksModel>(entity);
        }

        public static CourseDocRemarks ToEntity(this CourseDocRemarksModel model)
        {
            return Mapper.Map<CourseDocRemarksModel, CourseDocRemarks>(model);
        }
        #endregion

        #region Course Sharing
        public static CourseSharingModel ToModel(this CourseSharing entity)
        {
            return Mapper.Map<CourseSharing, CourseSharingModel>(entity);
        }

        public static CourseSharing ToEntity(this CourseSharingModel model)
        {
            return Mapper.Map<CourseSharingModel, CourseSharing>(model);
        }
        #endregion

        #region Course Sharing
        public static CourseAssignmentModel ToModel(this CourseAssignment entity)
        {
            return Mapper.Map<CourseAssignment, CourseAssignmentModel>(entity);
        }

        public static CourseAssignment ToEntity(this CourseAssignmentModel model)
        {
            return Mapper.Map<CourseAssignmentModel, CourseAssignment>(model);
        }
        #endregion

        #region Course Document Version
        public static CourseDocVersionModel ToModel(this CourseDocVersion entity)
        {
            return Mapper.Map<CourseDocVersion, CourseDocVersionModel>(entity);
        }

        public static CourseDocVersion ToEntity(this CourseDocVersionModel model)
        {
            return Mapper.Map<CourseDocVersionModel, CourseDocVersion>(model);
        }
        #endregion

        #region Main Course
        public static MainCourseModel ToModel(this MainCourse entity)
        {
            return Mapper.Map<MainCourse, MainCourseModel>(entity);
        }

        public static MainCourse ToEntity(this MainCourseModel model)
        {
            return Mapper.Map<MainCourseModel, MainCourse>(model);
        }
        #endregion

        #region Sub Course
        public static SubCourseModel ToModel(this SubCourse entity)
        {
            return Mapper.Map<SubCourse, SubCourseModel>(entity);
        }

        public static SubCourse ToEntity(this SubCourseModel model)
        {
            return Mapper.Map<SubCourseModel, SubCourse>(model);
        }
        #endregion

        #region Course Trackers
        public static CourseTrackersModel ToModel(this CourseTrackers entity)
        {
            return Mapper.Map<CourseTrackers, CourseTrackersModel>(entity);
        }

        public static CourseTrackers ToEntity(this CourseTrackersModel model)
        {
            return Mapper.Map<CourseTrackersModel, CourseTrackers>(model);
        }
        #endregion

        #region Course Download
        public static CourseDownloadUserModel ToModel(this CourseDownloadUser entity)
        {
            return Mapper.Map<CourseDownloadUser, CourseDownloadUserModel>(entity);
        }

        public static CourseDownloadUser ToEntity(this CourseDownloadUserModel model)
        {
            return Mapper.Map<CourseDownloadUserModel, CourseDownloadUser>(model);
        }
        #endregion
        #region Trainer Upload Document
        public static TrainerUploadDocumentModel ToModel(this TrainerUploadDocument entity)
        {
            return Mapper.Map<TrainerUploadDocument, TrainerUploadDocumentModel>(entity);
        }

        public static TrainerUploadDocument ToEntity(this TrainerUploadDocumentModel model)
        {
            return Mapper.Map<TrainerUploadDocumentModel, TrainerUploadDocument>(model);
        }
        #endregion

        #region Trainner Mac Address
        public static TrainnerMacAddressModel ToModel(this TrainnerMacAddress entity)
        {
            return Mapper.Map<TrainnerMacAddress, TrainnerMacAddressModel>(entity);
        }

        public static TrainnerMacAddress ToEntity(this TrainnerMacAddressModel model)
        {
            return Mapper.Map<TrainnerMacAddressModel, TrainnerMacAddress>(model);
        }
        #endregion

        #region TrainerQPUpload
        public static TrainerQPUploadModel ToModel(this TrainerQPUpload entity)
        {
            return Mapper.Map<TrainerQPUpload, TrainerQPUploadModel>(entity);
        }

        public static TrainerQPUpload ToEntity(this TrainerQPUploadModel model)
        {
            return Mapper.Map<TrainerQPUploadModel, TrainerQPUpload>(model);
        }
        #endregion
        #region TrainerQPUpload
        public static TrainerQP_Shared_StudentModel ToModel(this TrainerQP_Shared_Student entity)
        {
            return Mapper.Map<TrainerQP_Shared_Student, TrainerQP_Shared_StudentModel>(entity);
        }

        public static TrainerQP_Shared_Student ToEntity(this TrainerQP_Shared_StudentModel model)
        {
            return Mapper.Map<TrainerQP_Shared_StudentModel, TrainerQP_Shared_Student>(model);
        }
        #endregion

        #region TrainerQP Level Approval
        public static TrainerQP_Level_ApprovalModel ToModel(this TrainerQP_Level_Approval entity)
        {
            return Mapper.Map<TrainerQP_Level_Approval, TrainerQP_Level_ApprovalModel>(entity);
        }

        public static TrainerQP_Level_Approval ToEntity(this TrainerQP_Level_ApprovalModel model)
        {
            return Mapper.Map<TrainerQP_Level_ApprovalModel, TrainerQP_Level_Approval>(model);
        }
        #endregion

        #region Course Reminder
        public static CourseReminderModel ToModel(this CourseReminder entity)
        {
            return Mapper.Map<CourseReminder, CourseReminderModel>(entity);
        }
        public static CourseReminder ToEntity(this CourseReminderModel model)
        {
            return Mapper.Map<CourseReminderModel, CourseReminder>(model);
        }
        #endregion

        #region Course Renewal
        public static CourseRenewalModel ToModel(this CourseRenewal entity)
        {
            return Mapper.Map<CourseRenewal, CourseRenewalModel>(entity);
        }
        public static CourseRenewal ToEntity(this CourseRenewalModel model)
        {
            return Mapper.Map<CourseRenewalModel, CourseRenewal>(model);
        }
        #endregion

        #region Course Reminder Latter Undertaking
        public static Course_Reminder_Latter_UndertakingModel ToModel(this Course_Reminder_Latter_Undertaking entity)
        {
            return Mapper.Map<Course_Reminder_Latter_Undertaking, Course_Reminder_Latter_UndertakingModel>(entity);
        }
        public static Course_Reminder_Latter_Undertaking ToEntity(this Course_Reminder_Latter_UndertakingModel model)
        {
            return Mapper.Map<Course_Reminder_Latter_UndertakingModel, Course_Reminder_Latter_Undertaking>(model);
        }
        #endregion


        #region New course Tracker
        public static NewCourseTrackingDataModel ToModel(this NewCourseTrackingData entity)
        {
            return Mapper.Map<NewCourseTrackingData, NewCourseTrackingDataModel>(entity);
        }
        public static NewCourseTrackingData ToEntity(this NewCourseTrackingDataModel model)
        {
            return Mapper.Map<NewCourseTrackingDataModel, NewCourseTrackingData>(model);
        }
        #endregion

        #region QP Request
        public static QPRequestModel ToModel(this QPRequest entity)
        {
            return Mapper.Map<QPRequest, QPRequestModel>(entity);
        }
        public static QPRequest ToEntity(this QPRequestModel model)
        {
            return Mapper.Map<QPRequestModel, QPRequest>(model);
        }
        #endregion

        #region Report
        public static ReportModel ToModel(this Report entity)
        {
            return Mapper.Map<Report, ReportModel>(entity);
        }

        public static Report ToEntity(this ReportModel model)
        {
            return Mapper.Map<ReportModel, Report>(model);
        }
        #endregion
        

        public static StudentNotificationModel ToModel(this StudentNotification entity)
        {
            return Mapper.Map<StudentNotification, StudentNotificationModel>(entity);
        }

        public static StudentNotification ToEntity(this StudentNotificationModel model)
        {
            return Mapper.Map<StudentNotificationModel, StudentNotification>(model);
        }

        #region AssessmentPaper
        public static AssessmentPaperModel ToModel(this AssessmentPaper entity)
        {
            return Mapper.Map<AssessmentPaper, AssessmentPaperModel>(entity);
        }

        public static AssessmentPaper ToEntity(this AssessmentPaperModel model)
        {
            return Mapper.Map<AssessmentPaperModel, AssessmentPaper>(model);
        }
        #endregion


        #region DeveloperMonitorTimer
        public static DeveloperMonitorTimerModel ToModel(this DeveloperMonitorTimer entity)
        {
            return Mapper.Map<DeveloperMonitorTimer, DeveloperMonitorTimerModel>(entity);
        }

        public static DeveloperMonitorTimer ToEntity(this DeveloperMonitorTimerModel model)
        {
            return Mapper.Map<DeveloperMonitorTimerModel, DeveloperMonitorTimer>(model);
        }
        #endregion


        #region Centralized_Master
        public static Centralized_MasterModel ToModel(this Centralized_Master entity)
        {
            return Mapper.Map<Centralized_Master, Centralized_MasterModel>(entity);
        }

        public static Centralized_Master ToEntity(this Centralized_MasterModel model)
        {
            return Mapper.Map<Centralized_MasterModel, Centralized_Master>(model);
        }
        #endregion

        #region Centralized_HistoryModel
        public static Centralized_HistoryModel ToModel(this Centralized_History entity)
        {
            return Mapper.Map<Centralized_History, Centralized_HistoryModel>(entity);
        }

        public static Centralized_History ToEntity(this Centralized_HistoryModel model)
        {
            return Mapper.Map<Centralized_HistoryModel, Centralized_History>(model);
        }
        #endregion

        #region Centralized_CourseModel
        public static Centralized_CourseModel ToModel(this Centralized_Course entity)
        {
            return Mapper.Map<Centralized_Course, Centralized_CourseModel>(entity);
        }

        public static Centralized_Course ToEntity(this Centralized_CourseModel model)
        {
            return Mapper.Map<Centralized_CourseModel, Centralized_Course>(model);
        }
        #endregion

        #region Centralized_Document_filesModel
        public static Centralized_Document_filesModel ToModel(this Centralized_Document_files entity)
        {
            return Mapper.Map<Centralized_Document_files, Centralized_Document_filesModel>(entity);
        }

        public static Centralized_Document_files ToEntity(this Centralized_Document_filesModel model)
        {
            return Mapper.Map<Centralized_Document_filesModel, Centralized_Document_files>(model);
        }
        #endregion



        #region Central_CourseSharingModel
        public static Central_CourseSharingModel ToModel(this Central_CourseSharing entity)
        {
            return Mapper.Map<Central_CourseSharing, Central_CourseSharingModel>(entity);
        }

        public static Central_CourseSharing ToEntity(this Central_CourseSharingModel model)
        {
            return Mapper.Map<Central_CourseSharingModel, Central_CourseSharing>(model);
        }
        #endregion
    }
}