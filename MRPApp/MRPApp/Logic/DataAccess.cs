using MRPApp.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
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

        internal static List<Model.Report> GetReportDatas(string startDate, string endDate, string plantCode)
        {
            var connString = ConfigurationManager.ConnectionStrings["MRPConnString"].ToString();
            var list = new List<Model.Report>();

            using(var conn = new SqlConnection(connString))
            {
                conn.Open(); //필수
                var sqlQuery = $@"SELECT sch.SchIdx, SCH.PlantCode, sch.SchAmount, prc.PrcDate,
	                                    prc.PrcOKAmount, prc.PrcFailAmount
                                    FROM Schedules AS sch
                                    INNER JOIN (
	                                SELECT smr.SchIdx, smr.PrcDate,
		                                    SUM(smr.PrcOk) AS PrcOKAmount, SUM(smr.PrcFail) AS PrcFailAmount
                                        FROM (
		                                    SELECT p.SchIdx, p.PrcDate, 
				                                    CASE p.PrcResult WHEN 1 THEN 1 ELSE 0 END AS PrcOk,
	                                                CASE p.PrcResult WHEN 0 THEN 1 ELSE 0 END AS PrcFail
		                                        FROM Process AS p
		                                    ) AS smr
	                                    GROUP BY smr.SchIdx, smr.PrcDate 
                                    ) AS prc
                                    ON sch.SchIdx = prc.SchIdx
                                WHERE sch.PlantCode = '{plantCode}'
                                    AND prc.PrcDate BETWEEN '{startDate}' AND '{endDate}'";
                var cmd = new SqlCommand(sqlQuery, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var tmp = new Report
                    {
                        SchIdx = (int)reader["SchIdx"],
                        PlantCode = reader["PlantCode"].ToString(),
                        PrcDate = DateTime.Parse(reader["PrcDate"].ToString()),
                        SchAmount=(int)reader["SchAmount"],
                        PrcOKAmount=(int)reader["PrcOKAmount"],
                        PrcFailAmount=(int)reader["PrcFailAmount"],
                    };
                    list.Add(tmp);
                }
            }
            return list;
        }
    }
}
