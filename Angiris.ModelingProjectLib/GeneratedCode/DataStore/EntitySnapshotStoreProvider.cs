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

	public abstract class EntitySnapshotStoreProvider : INoSQLDataProvider
	{
		public abstract void GetOrCreateDatabaseAsync();

		public abstract void GetOrCreateCollectionAsync();

		public abstract void CreateEntity();

		public abstract void ReadEntity();

		public abstract void UpdateEntity();

		public abstract void DeleteEntity();

		public abstract void QueryEntities();

	}
}

