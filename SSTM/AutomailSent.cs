using SSTM.Business.Interfaces;
using SSTM.Core.Course_Reminder;
using SSTM.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSTM
{
    public class AutomailSent
    {
        private readonly IConfigService _IConfigService;
        private readonly ICourseReminderService _ICourseReminderService;
        private readonly IUserService _IUserService;
        public AutomailSent(IConfigService IConfigService, ICourseReminderService ICourseReminderService,
          IUserService IUserService)
        {
            _IConfigService = IConfigService;
            _ICourseReminderService = ICourseReminderService;
            _IUserService = IUserService;
        }
      
    }
}