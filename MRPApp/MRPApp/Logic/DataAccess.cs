﻿using MRPApp.Model;
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
            List<Model.Settings> settings;

            using(var ctx=new MRPEntities())
                settings = ctx.Settings.ToList(); //select
          
            return settings;
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
    }
}