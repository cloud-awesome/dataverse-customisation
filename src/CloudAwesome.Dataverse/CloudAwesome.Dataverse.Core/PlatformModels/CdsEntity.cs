using System.ServiceModel;
using System.Xml.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Core.PlatformModels
{
    public class CdsEntity
    {
        private string _solutionName;
        private string _publisherPrefix;
        private string _pluralName;
        private string _description;
        private string _primaryAttributeName;
        private int _primaryAttributeMaxLength;
        private string _primaryAttributeDescription;

        public string DisplayName { get; set; }
        public string SchemaName { get; set; }

        public string Description
        {
            get => _description ?? "A custom entity";
            set => _description = string.IsNullOrEmpty(value) ? "A custom entity" : value;
        }
        public OwnershipTypes OwnershipType { get; set; }

        public string PluralName
        {
            get => _pluralName ?? this.DisplayName.Pluralise();
            set => _pluralName = string.IsNullOrEmpty(value) ? this.DisplayName.Pluralise() : value;
        }
        
        public string PrimaryAttributeName
        {
            get => _primaryAttributeName;
            set => _primaryAttributeName = value ?? "Name";
        }

        public int? PrimaryAttributeMaxLength
        {
            get => _primaryAttributeMaxLength == 0? 50 : _primaryAttributeMaxLength;
            set => _primaryAttributeMaxLength = value ?? 50;
        }

        public string PrimaryAttributeDescription
        {
            get => _primaryAttributeDescription ?? "Primary name attribute for this entity";
            set => _primaryAttributeDescription = value ?? "Primary name attribute for this entity";
        }
        public bool? IsActivity { get; set; }
        public bool? HasActivities { get; set; }
        public bool? HasNotes { get; set; }
        public bool? IsQuickCreateEnabled { get; set; }
        public bool? IsAuditEnabled { get; set; }
        public bool? IsDuplicateDetectionEnabled { get; set; }
        public bool? IsBusinessProcessEnabled { get; set; }
        public bool? IsDocumentManagementEnabled { get; set; }
        public bool? IsValidForQueue { get; set; }
        public bool? ChangeTrackingEnabled { get; set; }
        
        public string NavigationColour { get; set; }

        public bool AllBusinessRules { get; set; }
        
        /*public CdsAttribute[] Attributes { get; set; }

        public CdsEntityPermission[] EntityPermissions { get; set; }*/

        
        /// <summary>
        /// Logic for updating boolean values in CRM.
        /// </summary>
        /// <remarks>
        /// If no new value is passed through, leave as is;
        /// If new value is TRUE then allow it regardless of existing value;
        /// If the new value is false but the existing value is true then check if this transition is permitted
        /// (e.g. can't switch off activities once activated)
        /// </remarks>
        /// <param name="newValue">Value pass through from manifest</param>
        /// <param name="existingValue">Value from existing CRM metadata</param>
        /// <param name="allowTrueToFalseTransition">Does CRM allow setting a TRUE to FALSE for this field?</param>
        /// <returns></returns>
        private bool GetBooleanValue(bool? newValue, bool? existingValue, bool allowTrueToFalseTransition = true)
        {
            switch (newValue)
            {
                case null:
                    return existingValue ?? false;
                case false:
                {
                    if (existingValue == true)
                    {
                        return !allowTrueToFalseTransition;
                    }
                    break;
                }
                default:
                    return true;
            }
            return false;
        }
    }
}
