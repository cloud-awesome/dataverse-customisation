using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Customisation.Tests;

[TestFixture]
public class SetEnvironmentVariableTests
{
    [Test]
    public void Run_creates_values_for_all_manifest_variables_without_existing_values()
    {
        var firstDefinitionId = Guid.NewGuid();
        var secondDefinitionId = Guid.NewGuid();
        var service = new InMemoryOrganizationService(
            Definition(firstDefinitionId, "ca_first"),
            Definition(secondDefinitionId, "ca_second"));

        var manifest = new SetEnvironmentVariableManifest
        {
            Variables =
            [
                new KeyValuePair { Key = "ca_first", Value = "first-value" },
                new KeyValuePair { Key = "ca_second", Value = "second-value" }
            ]
        };

        var process = new SetEnvironmentVariable();
        process.Run(service, Tracer(), manifest);

        var createdValues = service.Created
            .Where(e => e.LogicalName == EnvironmentVariableValue.EntityLogicalName)
            .ToList();

        Assert.That(createdValues, Has.Count.EqualTo(2));
        Assert.That(createdValues.Select(e => e.GetAttributeValue<string>(EnvironmentVariableValue.Fields.Value)),
            Is.EquivalentTo(new[] { "first-value", "second-value" }));
        Assert.That(
            createdValues.Select(e => e.GetAttributeValue<EntityReference>(EnvironmentVariableValue.Fields.EnvironmentVariableDefinitionId).Id),
            Is.EquivalentTo(new[] { firstDefinitionId, secondDefinitionId }));
    }

    private static EnvironmentVariableDefinition Definition(Guid id, string schemaName)
    {
        return new EnvironmentVariableDefinition
        {
            Id = id,
            SchemaName = schemaName
        };
    }

    private static TracingHelper Tracer()
    {
        return new TracingHelper(new LoggingConfiguration
        {
            LoggerConfigurationType = LoggingConfigurationType.Console,
            LogLevelToTrace = LogLevel.None
        });
    }

// TODO - Note for Wednesday! Remove this whole class and replace with dataverse-simulate.
    private sealed class InMemoryOrganizationService(params Entity[] seedRecords) : IOrganizationService
    {
        private readonly List<Entity> _records = seedRecords.ToList();

        public List<Entity> Created { get; } = [];

        public Guid Create(Entity entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            var stored = Clone(entity);
            Created.Add(stored);
            _records.Add(stored);

            return entity.Id;
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            if (query is not QueryExpression queryExpression)
            {
                throw new NotSupportedException("Only QueryExpression is supported by this test service.");
            }

            return new EntityCollection(_records
                .Where(e => e.LogicalName == queryExpression.EntityName)
                .Select(Clone)
                .ToList());
        }

        public void Update(Entity entity)
        {
            var index = _records.FindIndex(e => e.LogicalName == entity.LogicalName && e.Id == entity.Id);
            if (index < 0)
            {
                throw new InvalidOperationException("Cannot update a record that does not exist in the test service.");
            }

            _records[index] = Clone(entity);
        }

        public void Delete(string entityName, Guid id)
        {
            _records.RemoveAll(e => e.LogicalName == entityName && e.Id == id);
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return Clone(_records.Single(e => e.LogicalName == entityName && e.Id == id));
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            throw new NotSupportedException();
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotSupportedException();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotSupportedException();
        }

        private static Entity Clone(Entity source)
        {
            var clone = new Entity(source.LogicalName)
            {
                Id = source.Id
            };

            foreach (var attribute in source.Attributes)
            {
                clone[attribute.Key] = attribute.Value;
            }

            return clone;
        }
    }
}
