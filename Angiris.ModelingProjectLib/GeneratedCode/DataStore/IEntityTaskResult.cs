﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace DataStore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public interface IEntityTaskResult 
	{
		object Status { get;set; }

		object TaskID { get;set; }

		object TaskResult { get;set; }

		object StatusData { get;set; }

		DateTime CreateTime { get;set; }

		DateTime FinishTime { get;set; }

		DateTime LastModifiedTime { get;set; }

	}
}

