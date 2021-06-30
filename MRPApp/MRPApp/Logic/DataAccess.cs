using MRPApp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPApp.Logic
{
    public class DataAccess
    {
        //setting tbl에서 데이터 가져오기
        public static List<Settings> GetSettings()
        {
            List<Model.Settings> list;

            using(var ctx=new MRPEntities())
                list = ctx.Settings.ToList(); //select
          
            return list;
        }

        internal static int SetSettings(Settings item)
        {
            using(var ctx = new MRPEntities())
            {
                ctx.Settings.AddOrUpdate(item); //insert or update
                return ctx.SaveChanges(); //commit
            }
        }

        internal static int DelSettings(Settings item)
        {
            using(var ctx=new MRPEntities())
            {
                var obj = ctx.Settings.Find(item.BasicCode); //검색한 실제 데이터 삭제를 위함
                ctx.Settings.Remove(obj); //delete
                return ctx.SaveChanges();
            }
        }

        internal static List<Schedules> GetSchedules()
        {
            List<Model.Schedules> list;

            using (var ctx = new MRPEntities())
                list = ctx.Schedules.ToList(); //select

            return list;
        }

        internal static int SetSchedule(Schedules item)
        {
            using (var ctx = new MRPEntities())
            {
                ctx.Schedules.AddOrUpdate(item); //select
                return ctx.SaveChanges();
            }
        }

        internal static List<Process> GetProcesses()
        {
            List<Model.Process> list;
            using (var ctx = new MRPEntities())
                list = ctx.Process.ToList();//SELECT
            return list;
        }

        internal static int SetProcess(Process item)
        {
            using (var ctx=new MRPEntities())
            {
                ctx.Process.AddOrUpdate(item); //INSERT || UPDATE
                return ctx.SaveChanges(); //COMMIT
            }
        }
    }
}
