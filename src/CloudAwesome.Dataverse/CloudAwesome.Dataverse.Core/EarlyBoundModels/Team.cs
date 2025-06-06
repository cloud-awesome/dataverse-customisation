#pragma warning disable CS1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudAwesome.Dataverse.Core.EarlyBoundModels
{
	
	
	/// <summary>
	/// Collection of system users that routinely collaborate. Teams can be used to simplify record sharing and provide team members with common access to organization data when team members belong to different Business Units.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("team")]
	public partial class Team : Microsoft.Xrm.Sdk.Entity
	{
		
		/// <summary>
		/// Available fields, a the time of codegen, for the team entity
		/// </summary>
		public partial class Fields
		{
			public const string AdministratorId = "administratorid";
			public const string AdministratorIdName = "administratoridname";
			public const string AdministratorIdYomiName = "administratoridyominame";
			public const string AzureActiveDirectoryObjectId = "azureactivedirectoryobjectid";
			public const string BusinessUnitId = "businessunitid";
			public const string BusinessUnitIdName = "businessunitidname";
			public const string CreatedBy = "createdby";
			public const string CreatedByName = "createdbyname";
			public const string CreatedByYomiName = "createdbyyominame";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string CreatedOnBehalfByName = "createdonbehalfbyname";
			public const string CreatedOnBehalfByYomiName = "createdonbehalfbyyominame";
			public const string DelegatedAuthorizationId = "delegatedauthorizationid";
			public const string DelegatedAuthorizationIdName = "delegatedauthorizationidname";
			public const string Description = "description";
			public const string EmailAddress = "emailaddress";
			public const string ExchangerAte = "exchangerate";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string IsDefault = "isdefault";
			public const string IsDefaultName = "isdefaultname";
			public const string IsSasTokenSet = "issastokenset";
			public const string IsSasTokenSetName = "issastokensetname";
			public const string MembershipType = "membershiptype";
			public const string MembershipTypeName = "membershiptypename";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedByName = "modifiedbyname";
			public const string ModifiedByYomiName = "modifiedbyyominame";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string ModifiedOnBehalfByName = "modifiedonbehalfbyname";
			public const string ModifiedOnBehalfByYomiName = "modifiedonbehalfbyyominame";
			public const string Name = "name";
			public const string OrganizationId = "organizationid";
			public const string OrganizationIdName = "organizationidname";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string ProcessId = "processid";
			public const string QueueId = "queueid";
			public const string QueueIdName = "queueidname";
			public const string RegardingObjectId = "regardingobjectid";
			public const string SasToken = "sastoken";
			public const string ShareLinkQualifier = "sharelinkqualifier";
			public const string StageId = "stageid";
			public const string SystemManaged = "systemmanaged";
			public const string SystemManagedName = "systemmanagedname";
			public const string Team_CustomApi = "Team_CustomApi";
			public const string Team_EnvironmentVariableDefinition = "Team_EnvironmentVariableDefinition";
			public const string Team_Msdyn_CustomApiRuleSetConfiguration = "Team_Msdyn_CustomApiRuleSetConfiguration";
			public const string Team_Msdyn_SlaKpi = "Team_Msdyn_SlaKpi";
			public const string Team_SlaBase = "Team_SlaBase";
			public const string Team_Workflow = "Team_Workflow";
			public const string TeamId = "teamid";
			public const string Id = "teamid";
			public const string TeamRoles_Association = "teamroles_association";
			public const string TeamTemplateId = "teamtemplateid";
			public const string TeamType = "teamtype";
			public const string TeamTypeName = "teamtypename";
			public const string TransactionCurrencyId = "transactioncurrencyid";
			public const string TransactionCurrencyIdName = "transactioncurrencyidname";
			public const string TraversedPath = "traversedpath";
			public const string VersionNumber = "versionnumber";
			public const string YomiName = "yominame";
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public Team(System.Guid id) : 
				base(EntityLogicalName, id)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public Team(string keyName, object keyValue) : 
				base(EntityLogicalName, keyName, keyValue)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public Team(Microsoft.Xrm.Sdk.KeyAttributeCollection keyAttributes) : 
				base(EntityLogicalName, keyAttributes)
		{
		}
		
		public const string AlternateKeys = "azureactivedirectoryobjectid,membershiptype";
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public Team() : 
				base(EntityLogicalName)
		{
		}
		
		public const string PrimaryIdAttribute = "teamid";
		
		public const string PrimaryNameAttribute = "name";
		
		public const string EntitySchemaName = "Team";
		
		public const string EntityLogicalName = "team";
		
		public const string EntityLogicalCollectionName = "teams";
		
		public const string EntitySetName = "teams";
		
		/// <summary>
		/// Unique identifier of the user primary responsible for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("administratorid")]
		public Microsoft.Xrm.Sdk.EntityReference AdministratorId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("administratorid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("administratorid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("administratoridname")]
		public string AdministratorIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("administratorid"))
				{
					return this.FormattedValues["administratorid"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["administratorid"] = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("administratoridyominame")]
		public string AdministratorIdYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("administratorid"))
				{
					return this.FormattedValues["administratorid"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["administratorid"] = value;
			}
		}
		
		/// <summary>
		/// The object Id for a group.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("azureactivedirectoryobjectid")]
		public System.Nullable<System.Guid> AzureActiveDirectoryObjectId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("azureactivedirectoryobjectid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("azureactivedirectoryobjectid", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the business unit with which the team is associated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessunitid")]
		public Microsoft.Xrm.Sdk.EntityReference BusinessUnitId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("businessunitid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("businessunitid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessunitidname")]
		public string BusinessUnitIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("businessunitid"))
				{
					return this.FormattedValues["businessunitid"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["businessunitid"] = value;
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("createdby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyname")]
		public string CreatedByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["createdby"] = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyyominame")]
		public string CreatedByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["createdby"] = value;
			}
		}
		
		/// <summary>
		/// Date and time when the team was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		public System.Nullable<System.DateTime> CreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("createdon", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who created the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("createdonbehalfby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyname")]
		public string CreatedOnBehalfByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["createdonbehalfby"] = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyyominame")]
		public string CreatedOnBehalfByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["createdonbehalfby"] = value;
			}
		}
		
		/// <summary>
		/// The delegated authorization context for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("delegatedauthorizationid")]
		public Microsoft.Xrm.Sdk.EntityReference DelegatedAuthorizationId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("delegatedauthorizationid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("delegatedauthorizationid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("delegatedauthorizationidname")]
		public string DelegatedAuthorizationIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("delegatedauthorizationid"))
				{
					return this.FormattedValues["delegatedauthorizationid"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["delegatedauthorizationid"] = value;
			}
		}
		
		/// <summary>
		/// Description of the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("description")]
		public string Description
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("description");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("description", value);
			}
		}
		
		/// <summary>
		/// Email address for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("emailaddress")]
		public string EmailAddress
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("emailaddress");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("emailaddress", value);
			}
		}
		
		/// <summary>
		/// Exchange rate for the currency associated with the team with respect to the base currency.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("exchangerate")]
		public System.Nullable<decimal> ExchangerAte
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("exchangerate");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("exchangerate", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the data import or data migration that created this record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("importsequencenumber")]
		public System.Nullable<int> ImportSequenceNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("importsequencenumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("importsequencenumber", value);
			}
		}
		
		/// <summary>
		/// Information about whether the team is a default business unit team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isdefault")]
		public System.Nullable<bool> IsDefault
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("isdefault");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("isdefault", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isdefaultname")]
		public string IsDefaultName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("isdefault"))
				{
					return this.FormattedValues["isdefault"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["isdefault"] = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("issastokenset")]
		public System.Nullable<bool> IsSasTokenSet
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("issastokenset");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("issastokenset", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("issastokensetname")]
		public string IsSasTokenSetName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("issastokenset"))
				{
					return this.FormattedValues["issastokenset"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["issastokenset"] = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("membershiptype")]
		public virtual Team_MembershipType? MembershipType
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((Team_MembershipType?)(EntityOptionSetEnum.GetEnum(this, "membershiptype")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("membershiptype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("membershiptypename")]
		public string MembershipTypeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("membershiptype"))
				{
					return this.FormattedValues["membershiptype"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["membershiptype"] = value;
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("modifiedby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyname")]
		public string ModifiedByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["modifiedby"] = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyyominame")]
		public string ModifiedByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["modifiedby"] = value;
			}
		}
		
		/// <summary>
		/// Date and time when the team was last modified.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("modifiedon", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who last modified the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("modifiedonbehalfby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyname")]
		public string ModifiedOnBehalfByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["modifiedonbehalfby"] = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyyominame")]
		public string ModifiedOnBehalfByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["modifiedonbehalfby"] = value;
			}
		}
		
		/// <summary>
		/// Name of the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("name")]
		public string Name
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("name");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("name", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the organization associated with the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationid")]
		public System.Nullable<System.Guid> OrganizationId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("organizationid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("organizationid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationidname")]
		public string OrganizationIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("organizationid"))
				{
					return this.FormattedValues["organizationid"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["organizationid"] = value;
			}
		}
		
		/// <summary>
		/// Date and time that the record was migrated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overriddencreatedon")]
		public System.Nullable<System.DateTime> OverriddenCreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overriddencreatedon");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("overriddencreatedon", value);
			}
		}
		
		/// <summary>
		/// Shows the ID of the process.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("processid")]
		public System.Nullable<System.Guid> ProcessId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("processid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("processid", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the default queue for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueid")]
		public Microsoft.Xrm.Sdk.EntityReference QueueId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("queueid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("queueid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueidname")]
		public string QueueIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("queueid"))
				{
					return this.FormattedValues["queueid"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["queueid"] = value;
			}
		}
		
		/// <summary>
		/// Choose the record that the team relates to.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("regardingobjectid")]
		public Microsoft.Xrm.Sdk.EntityReference RegardingObjectId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("regardingobjectid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("regardingobjectid", value);
			}
		}
		
		/// <summary>
		/// Sas Token for Team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("sastoken")]
		public string SasToken
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("sastoken");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("sastoken", value);
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("sharelinkqualifier")]
		public string ShareLinkQualifier
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("sharelinkqualifier");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("sharelinkqualifier", value);
			}
		}
		
		/// <summary>
		/// Shows the ID of the stage.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("stageid")]
		public System.Nullable<System.Guid> StageId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("stageid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("stageid", value);
			}
		}
		
		/// <summary>
		/// Select whether the team will be managed by the system.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("systemmanaged")]
		public System.Nullable<bool> SystemManaged
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("systemmanaged");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("systemmanaged", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("systemmanagedname")]
		public string SystemManagedName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("systemmanaged"))
				{
					return this.FormattedValues["systemmanaged"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["systemmanaged"] = value;
			}
		}
		
		/// <summary>
		/// Unique identifier for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("teamid")]
		public System.Nullable<System.Guid> TeamId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("teamid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("teamid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("teamid")]
		public override System.Guid Id
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return base.Id;
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.TeamId = value;
			}
		}
		
		/// <summary>
		/// Shows the team template that is associated with the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("teamtemplateid")]
		public Microsoft.Xrm.Sdk.EntityReference TeamTemplateId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("teamtemplateid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("teamtemplateid", value);
			}
		}
		
		/// <summary>
		/// Select the team type.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("teamtype")]
		public virtual Team_TeamType? TeamType
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((Team_TeamType?)(EntityOptionSetEnum.GetEnum(this, "teamtype")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("teamtype", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("teamtypename")]
		public string TeamTypeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("teamtype"))
				{
					return this.FormattedValues["teamtype"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["teamtype"] = value;
			}
		}
		
		/// <summary>
		/// Unique identifier of the currency associated with the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		public Microsoft.Xrm.Sdk.EntityReference TransactionCurrencyId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("transactioncurrencyid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("transactioncurrencyid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyidname")]
		public string TransactionCurrencyIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("transactioncurrencyid"))
				{
					return this.FormattedValues["transactioncurrencyid"];
				}
				else
				{
					return default(string);
				}
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.FormattedValues["transactioncurrencyid"] = value;
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("traversedpath")]
		public string TraversedPath
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("traversedpath");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("traversedpath", value);
			}
		}
		
		/// <summary>
		/// Version number of the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("versionnumber", value);
			}
		}
		
		/// <summary>
		/// Pronunciation of the full name of the team, written in phonetic hiragana or katakana characters.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("yominame")]
		public string YomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("yominame");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("yominame", value);
			}
		}
		
		/// <summary>
		/// 1:N team_customapi
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_customapi")]
		public System.Collections.Generic.IEnumerable<CloudAwesome.Dataverse.Core.EarlyBoundModels.CustomApi> Team_CustomApi
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.CustomApi>("team_customapi", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.CustomApi>("team_customapi", null, value);
			}
		}
		
		/// <summary>
		/// 1:N team_environmentvariabledefinition
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_environmentvariabledefinition")]
		public System.Collections.Generic.IEnumerable<CloudAwesome.Dataverse.Core.EarlyBoundModels.EnvironmentVariableDefinition> Team_EnvironmentVariableDefinition
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.EnvironmentVariableDefinition>("team_environmentvariabledefinition", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.EnvironmentVariableDefinition>("team_environmentvariabledefinition", null, value);
			}
		}
		
		/// <summary>
		/// 1:N team_msdyn_customapirulesetconfiguration
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_msdyn_customapirulesetconfiguration")]
		public System.Collections.Generic.IEnumerable<CloudAwesome.Dataverse.Core.EarlyBoundModels.Msdyn_CustomApiRuleSetConfiguration> Team_Msdyn_CustomApiRuleSetConfiguration
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Msdyn_CustomApiRuleSetConfiguration>("team_msdyn_customapirulesetconfiguration", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Msdyn_CustomApiRuleSetConfiguration>("team_msdyn_customapirulesetconfiguration", null, value);
			}
		}
		
		/// <summary>
		/// 1:N team_msdyn_slakpi
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_msdyn_slakpi")]
		public System.Collections.Generic.IEnumerable<CloudAwesome.Dataverse.Core.EarlyBoundModels.Msdyn_SlaKpi> Team_Msdyn_SlaKpi
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Msdyn_SlaKpi>("team_msdyn_slakpi", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Msdyn_SlaKpi>("team_msdyn_slakpi", null, value);
			}
		}
		
		/// <summary>
		/// 1:N team_slaBase
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_slaBase")]
		public System.Collections.Generic.IEnumerable<CloudAwesome.Dataverse.Core.EarlyBoundModels.Sla> Team_SlaBase
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Sla>("team_slaBase", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Sla>("team_slaBase", null, value);
			}
		}
		
		/// <summary>
		/// 1:N team_workflow
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("team_workflow")]
		public System.Collections.Generic.IEnumerable<CloudAwesome.Dataverse.Core.EarlyBoundModels.Workflow> Team_Workflow
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Workflow>("team_workflow", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Workflow>("team_workflow", null, value);
			}
		}
		
		/// <summary>
		/// N:N teamroles_association
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("teamroles_association")]
		public System.Collections.Generic.IEnumerable<CloudAwesome.Dataverse.Core.EarlyBoundModels.Role> TeamRoles_Association
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Role>("teamroles_association", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<CloudAwesome.Dataverse.Core.EarlyBoundModels.Role>("teamroles_association", null, value);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public Team(object anonymousType) : 
				this()
		{
            foreach (var p in anonymousType.GetType().GetProperties())
            {
                var value = p.GetValue(anonymousType, null);
                var name = p.Name.ToLower();
            
                if (value != null && name.EndsWith("enum") && value.GetType().BaseType == typeof(System.Enum))
                {
                    value = new Microsoft.Xrm.Sdk.OptionSetValue((int) value);
                    name = name.Remove(name.Length - "enum".Length);
                }
            
                switch (name)
                {
                    case "id":
                        base.Id = (System.Guid)value;
                        Attributes["teamid"] = base.Id;
                        break;
                    case "teamid":
                        var id = (System.Nullable<System.Guid>) value;
                        if(id == null){ continue; }
                        base.Id = id.Value;
                        Attributes[name] = base.Id;
                        break;
                    case "formattedvalues":
                        // Add Support for FormattedValues
                        FormattedValues.AddRange((Microsoft.Xrm.Sdk.FormattedValueCollection)value);
                        break;
                    default:
                        Attributes[name] = value;
                        break;
                }
            }
		}
	}
}
#pragma warning restore CS1591
