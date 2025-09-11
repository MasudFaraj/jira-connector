namespace Neusta.UserService.TestBase.Base;

using System.Collections.Generic;
using AutoMapper;
using NUnit.Framework;

public abstract class MapperTestBase<TProfile>
    where TProfile : Profile, new()
{
    private IConfigurationProvider configuration;
    protected Mapper Mapper;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        this.configuration = new MapperConfiguration(this.GetConfiguration);
        this.Mapper = new Mapper(this.configuration);
    }

    [Test]
    public void AutoMapperProfileConfigurationTest()
    {
        this.configuration.AssertConfigurationIsValid();
    }

    protected virtual IList<Profile> GetProfiles()
    {
        return new List<Profile> { new TProfile() };
    }

    protected TTarget Map<TTarget>(object source, IDictionary<string, object> items = null)
    {
        return this.Mapper.Map<TTarget>(
            source,
            opts =>
            {
                if (items != null)
                {
                    foreach (KeyValuePair<string, object> item in items)
                    {
                        opts.Items.Add(item);
                    }
                }
            });
    }

    private void GetConfiguration(IMapperConfigurationExpression cfg)
    {
        cfg.AddProfiles(this.GetProfiles());
    }
}