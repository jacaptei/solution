using JaCaptei.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model {


    public class AgendaCRM {

        public dynamic values { get; set; }

        //public string date_start		{get;set;} = "";
        //public string due_date			{get;set;} = "";
        //public string time_start		{get;set;} = "";
        //public string time_end			{get;set;} = "";
        //public string duration_hours	{get;set;} = "";
        //public string activitytype		{get;set;} = "";
        //public string location			{get;set;} = "";
        //public string subject			{get;set;} = "";
        //public string assigned_user_id	{get;set;} = "";
        //public string eventstatus		{get;set;} = "";
        //public string visibility		{get;set;} = "";
        //public string taskpriority		{get;set;} = "";
        //public string description		{get;set;} = "";
        //public string parent_id			{get;set;} = "";

        public Parceiro usuario { get; set; } = new Parceiro();

    }



}
