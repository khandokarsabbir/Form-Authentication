using System;
using System.Collections.Generic;
using System.Linq;
using RunrentSales.Model;
using RunrentSales.Repository;

namespace RunrentSales.Domain.Services
{
    public class ActivityService
    {
        private ContactDataAccess cda;
        
        public ActivityService() 
        {
            this.cda = new ContactDataAccess();
        }

        public List<Activity> GetActivitiesForActivityGrid(int page, int RelationItemID, byte ActivityRelationType, int rows, string sidx, string sord, string searchField, string searchOper, string searchString, bool _search, ref int totalRecord)
        {
            List<Activity> oActivities = null;
            if (_search)
                oActivities = GetActivityHistorySearch(RelationItemID, searchField, searchOper, searchString, (byte)ActivityHistoryStatus.Activity, ActivityRelationType);
            else
                oActivities = GetActivityHistoryByItemID(RelationItemID, (byte)ActivityHistoryStatus.Activity, ActivityRelationType);

            totalRecord = oActivities.Count;
            if (rows > totalRecord)
                page = 1;

            oActivities = GetActivityHistoySortPage(oActivities, page, rows, sord, sidx);

            return oActivities;
        }

        public List<Activity> GetActivitiesForHistoryGrid(int page, int RelationItemID, byte ActivityRelationType, int rows, string sidx, string sord, string searchField, string searchOper, string searchString, bool _search, ref int totalRecord)
        {
            List<Activity> oActivities = null;
            if (_search)
                oActivities = GetActivityHistorySearch(RelationItemID, searchField, searchOper, searchString, (byte)ActivityHistoryStatus.Activity, ActivityRelationType);
            else
                oActivities = GetActivityHistoryByItemID(RelationItemID, (byte)ActivityHistoryStatus.History, ActivityRelationType);

            totalRecord = oActivities.Count;
            if (rows > totalRecord)
                page = 1;

            oActivities = GetActivityHistoySortPage(oActivities, page, rows, sord, sidx);

            return oActivities;
        }

        public string GetDurationFormat(int minnutes)
        {
            string durationText = "";

            if (minnutes / 60 != 0)
                durationText = (minnutes / 60).ToString() + " hour ";

            durationText += (minnutes % 60).ToString() + " minutes";

            return durationText;
        }

        public List<Activity> GetActivityHistorySearch(int itemID, string sField, string sOper, string sString, byte status, byte relType)
        {
            List<Activity> oActivities = new List<Activity>();
            List<Activity> oActivityList = cda.GetActivityHistoryByItemID(itemID, status, relType);
            try
            {

                if (sField == "TypeID")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.ActivityType.Name.ToUpper() == sString.ToUpper()).ToList();

                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.ActivityType.Name.ToUpper().Contains(sString.ToUpper())).ToList();

                }

                else if (sField == "Date")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.Date == DateTime.Parse(sString)).ToList();
                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.Date.ToShortDateString().Contains(sString)).ToList();
                }

                else if (sField == "Time")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.Time == sString).ToList();
                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.Time.Contains(sString)).ToList();
                }
                else if (sField == "ResultID")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.ActivityResult.Name.ToUpper() == sString.ToUpper()).ToList();
                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.ActivityResult.Name.ToUpper().Contains(sString.ToUpper())).ToList();
                }

                else if (sField == "Duration")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.Duration == Convert.ToInt32(sString)).ToList();
                }
                else if (sField == "Regarding")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.Regards.ToUpper() == sString.ToUpper()).ToList();
                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.Regards.ToUpper().Contains(sString.ToUpper())).ToList();
                }
                else if (sField == "Location")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.Location.ToUpper() == sString.ToUpper()).ToList();
                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.Location.ToUpper().Contains(sString.ToUpper())).ToList();
                }
                else if (sField == "PriorityID")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.ActivityPriority.Name.ToUpper() == sString.ToUpper()).ToList();
                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.ActivityPriority.Name.ToUpper().Contains(sString.ToUpper())).ToList();
                }

                if (sField == "Regards")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.ActivityRegarding.Name.ToUpper() == sString.ToUpper()).ToList();

                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.ActivityRegarding.Name.ToUpper().Contains(sString.ToUpper())).ToList();

                }

                else if (sField == "Details")
                {
                    if (sOper == "eq")
                        oActivities = oActivityList.Where(c => c.Regards.ToUpper() == sString.ToUpper()).ToList();
                    else if (sOper == "cn")
                        oActivities = oActivityList.Where(c => c.Regards.ToUpper().Contains(sString.ToUpper())).ToList();
                }

                else if (sField == "ContactParticipant")
                {
                    List<ActivityParticipant> oParticipant = null;
                    if (sOper == "eq")
                    {
                        oParticipant = cda.GetActivityParticipantsByContactName(sString, sOper);
                    }
                    else if (sOper == "cn")
                    {
                        oParticipant = cda.GetActivityParticipantsByContactName(sString, sOper);
                    }
                    var query = (from p in oParticipant
                                 join c in oActivityList on p.ActivityID equals c.ID
                                 select c).ToList();
                    oActivities = query;


                }
                return oActivities;
            }
            catch
            {
                return oActivities;
            }

        }
        
        public List<Activity> GetActivityHistoySortPage(List<Activity> Activities, int page, int pageRecord, string sortOrder, string sortBy)
        {
            try
            {
                int startPosition = page * pageRecord - pageRecord;

                if (sortOrder == "asc")
                {
                    switch (sortBy)
                    {
                        case "ID":
                            var c0 = (from p in Activities orderby p.ID select p).Skip(startPosition).Take(pageRecord);
                            return c0.ToList();
                        case "TypeID":
                            var c1 = (from p in Activities orderby p.ActivityType.Name select p).Skip(startPosition).Take(pageRecord);
                            return c1.ToList();
                        case "Date":
                            var c2 = (from p in Activities orderby p.Date select p).Skip(startPosition).Take(pageRecord);
                            return c2.ToList();
                        case "Duration":
                            var c4 = (from p in Activities orderby p.Duration select p).Skip(startPosition).Take(pageRecord);
                            return c4.ToList();
                        case "PriorityID":
                            var c6 = (from p in Activities orderby p.PriorityID select p).Skip(startPosition).Take(pageRecord);
                            return c6.ToList();
                        case "ResultID":
                            var c7 = (from p in Activities orderby p.ActivityResult.Name select p).Skip(startPosition).Take(pageRecord);
                            return c7.ToList();
                        case "RegardingID":
                            var c8 = (from p in Activities orderby p.ActivityRegarding.Name select p).Skip(startPosition).Take(pageRecord);
                            return c8.ToList();
                        case "Time":
                            var c9 = (from p in Activities orderby p.Time select p).Skip(startPosition).Take(pageRecord);
                            return c9.ToList();
                        case "Location":
                            var c10 = (from p in Activities orderby p.Location select p).Skip(startPosition).Take(pageRecord);
                            return c10.ToList();

                    }
                }
                else
                {
                    switch (sortBy)
                    {
                        case "ID":
                            var c0 = (from p in Activities orderby p.ID descending select p).Skip(startPosition).Take(pageRecord);
                            return c0.ToList();
                        case "TypeID":
                            var c1 = (from p in Activities orderby p.ActivityType.Name descending select p).Skip(startPosition).Take(pageRecord);
                            return c1.ToList();

                        case "Date":
                            var c2 = (from p in Activities orderby p.Date descending select p).Skip(startPosition).Take(pageRecord);
                            return c2.ToList();
                        case "Duration":
                            var c4 = (from p in Activities orderby p.Duration descending select p).Skip(startPosition).Take(pageRecord);
                            return c4.ToList();
                        case "PriorityID":
                            var c6 = (from p in Activities orderby p.PriorityID descending select p).Skip(startPosition).Take(pageRecord);
                            return c6.ToList();
                        case "ResultID":
                            var c7 = (from p in Activities orderby p.ActivityResult.Name descending select p).Skip(startPosition).Take(pageRecord);
                            return c7.ToList();

                        case "RegardingID":
                            var c8 = (from p in Activities orderby p.ActivityRegarding.Name descending select p).Skip(startPosition).Take(pageRecord);
                            return c8.ToList();
                        case "Time":
                            var c9 = (from p in Activities orderby p.Time descending select p).Skip(startPosition).Take(pageRecord);
                            return c9.ToList();
                        case "Location":
                            var c10 = (from p in Activities orderby p.Location descending select p).Skip(startPosition).Take(pageRecord);
                            return c10.ToList();

                    }
                }
                return new List<Activity>();
            }
            catch
            { return Activities; }
        }

        public bool addToOrganize(int id, string start, string end, string title, string location, string color)
        {
            OrganizeDataAccess oda = new OrganizeDataAccess();
            OrganizeSchedular organizeSchedular = new OrganizeSchedular();

            organizeSchedular = null;

            DateTime startDate;
            DateTime lastDate;

            try
            {
                startDate = Convert.ToDateTime(start);
            }
            catch
            {
                startDate = System.DateTime.Now;
            }

            try
            {
                lastDate = Convert.ToDateTime(end);
            }

            catch
            {
                lastDate = startDate.AddHours(1);
            }

            organizeSchedular = new OrganizeSchedular();
            organizeSchedular.ID = Guid.NewGuid().ToString();
            organizeSchedular.Title = title;
            organizeSchedular.InstanceID = ProjectHelpers.RunrentInstanceId;
            organizeSchedular.ActivityId = id;
            organizeSchedular.OpenUserID = ProjectHelpers.RunrentOpenUserID;
            organizeSchedular.StartSchedule = startDate;
            organizeSchedular.EndSchedule = lastDate;
            organizeSchedular.Color = color;
            organizeSchedular.Location = location;
            organizeSchedular.LastUpdatedDate = DateTime.Now;
            organizeSchedular.CreateDate = DateTime.Now;
            oda.AddEvent(organizeSchedular);
            try
            {
                oda.Save();
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool updateToOrganize(int ActivityId, string start, string end, string title, string body, string color)
        {
            OrganizeDataAccess oda = new OrganizeDataAccess();
            List<OrganizeSchedular> organizeSchedular = new List<OrganizeSchedular>();
            organizeSchedular = oda.GetEventsByActivityID(ActivityId);
            oda.DeleteEvents(organizeSchedular);
            bool success = addToOrganize(ActivityId, start, end, title, body, color);
            oda.Save();
            return success;
        }

        public void AddActivityParticipant(string contacts, int activityID)
        {
            string[] contactIDs = contacts.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ActivityParticipant oParticipant;
            foreach (string contactID in contactIDs)
            {
                oParticipant = new ActivityParticipant();
                oParticipant.ActivityID = activityID;
                oParticipant.ContactID = Convert.ToInt32(contactID);
                oParticipant.PropertyID = null;
                DatabaseHelper.Insert<ActivityParticipant>(oParticipant);
            }
        }

        public void DeleteActivities(string ids)
        {
            OrganizeDataAccess oda = new OrganizeDataAccess();
            string[] aId = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int tempID = 0;
            Activity oActivity;
            foreach (string id in aId)
            {
                try
                {
                    tempID = Convert.ToInt32(id);
                }
                catch
                {
                    continue;
                }
                oActivity = cda.GetActivityByID(tempID);
                cda.DeleteActivityParticipants(oActivity.ActivityParticipants.ToList());
                List<OrganizeSchedular> events = oda.GetEventsByActivityID(oActivity.ID);
                oda.DeleteEvents(events);
                cda.DeleteActivity(oActivity);
                cda.Save();
            }
        }

        public void DeleteActivityParticipant(int activityID)
        {
            try
            {
                List<ActivityParticipant> oParticipants = cda.GetActivityParticipantByActivityID(activityID);
                if (oParticipants != null)
                {
                    cda.DeleteActivityParticipants(oParticipants);
                    cda.Save();
                }
            }
            catch { }

        }

        public Activity GetActivityByID(int id)
        {
            return cda.GetActivityByID(id);
        }

        public List<ActivityParticipant> GetActivityParticipantByActivityID(int id)
        {
            return cda.GetActivityParticipantByActivityID(id);
        }

        public void MoveActivitiesToHistory(string ids)
        {
            string[] aId = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Activity oActivity;
            foreach (string activityID in aId)
            {
                oActivity = cda.GetActivityByID(Convert.ToInt32(activityID));
                oActivity.Status = (byte)ActivityHistoryStatus.History;
                cda.Save();
            }
        }

        public void AddActivity(Activity oActivity)
        {
            cda.AddActivity(oActivity);
        }

        public List<Activity> GetActivityHistoryByItemID(int RelationItemID, byte status, byte ActivityRelationType)
        {
            return cda.GetActivityHistoryByItemID(RelationItemID, status, ActivityRelationType);
        }

        public string GetActivityTypeNameByID(int? typeID)
        {
            return cda.GetActivityTypeNameByID(typeID);
        }

        public string GetActivityResultNameByID(int? resultID)
        {
            return cda.GetActivityResultNameByID(resultID);
        }

        public string GetActivityRegardingNameByID(int? regardingID)
        {
            return cda.GetActivityRegardingNameByID(regardingID);
        }

        public void Save()
        {
            cda.Save();
        }

        public ActivityPriority GetActivityPriorityById(int id)
        {
            return cda.GetActivityPriorityById(id);
        }

        public ActivityType GetActivityTypeByID(int typeId)
        {
            return cda.GetActivityTypeByID(typeId);
        }
    }

    public enum ActivityHistoryStatus : byte
    {
        Activity = 1,
        History = 2
    }

    public enum ActivityHistoryRelationType : byte
    {
        Contact = 1,
        SUProperty = 2,
        MUProperty = 3
    }
}
