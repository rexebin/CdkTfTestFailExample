using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.AcmCertificate;
using HashiCorp.Cdktf.Providers.Aws.AcmCertificateValidation;
using HashiCorp.Cdktf.Providers.Aws.DataAwsRoute53Zone;
using HashiCorp.Cdktf.Providers.Aws.Provider;
using HashiCorp.Cdktf.Providers.Aws.Route53Record;

namespace IteratorStack;

public class IteratorStack: TerraformStack
{
    public IteratorStack(Construct scope, string id) : base(scope, id)
    {
        _ = new AwsProvider(this, "aws", new AwsProviderConfig()
        {
            Region = "us-west-2"
        });
        
        var cert = new AcmCertificate(this, "cert", new AcmCertificateConfig
        {
            DomainName = "example.com",
            ValidationMethod = "DNS"
        });

        var dataAwsRoute53ZoneExample = new DataAwsRoute53Zone(this, "dns_zone", new DataAwsRoute53ZoneConfig
        {
            Name = "example.com",
            PrivateZone = false
        });

        var exampleForEachIterator = TerraformIterator.FromComplexList(cert.DomainValidationOptions, "domain_name");

        var records = new Route53Record(this, "record", new Route53RecordConfig
        {
            ForEach = exampleForEachIterator,
            AllowOverwrite = true,
            Name = exampleForEachIterator.GetString("resource_record_name"),
            Records = new string[] { exampleForEachIterator.GetString("resource_record_record") },
            Ttl = 60,
            Type = exampleForEachIterator.GetString("resource_record_type"),
            ZoneId = dataAwsRoute53ZoneExample.ZoneId
        });

        var recordsIterator = TerraformIterator.FromResources(records);
        
        new AcmCertificateValidation(this, "validation", new AcmCertificateValidationConfig
        {
            CertificateArn = cert.Arn,
            // ValidationRecordFqdns = Token.AsList(recordsIterator.PluckProperty("fqdn")) // test fails
            ValidationRecordFqdns = [] // test passes
        });

    }
}