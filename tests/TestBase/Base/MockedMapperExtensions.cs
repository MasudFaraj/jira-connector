namespace Neusta.UserService.TestBase.Base;

using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;

public static class MockedMapperExtensions
{
    public static IDictionary<string, object> CaptureItems<TTarget>(this Mock<IMapper> mockedMapper)
    {
        IDictionary<string, object> items = new Dictionary<string, object>();
        mockedMapper
            .Setup(x => x.Map(It.IsAny<object>(), It.IsAny<Action<IMappingOperationOptions<object, TTarget>>>()))
            .Callback<object, Action<IMappingOperationOptions<object, TTarget>>>((source, optionAction) =>
            {
                MappingOperationOptions<object, TTarget> options = CreateOptions<TTarget>();
                optionAction(options);

                foreach (KeyValuePair<string, object> item in options.Items)
                {
                    items.Add(item);
                }
            });

        return items;
    }

    private static MappingOperationOptions<object, TTarget> CreateOptions<TTarget>()
    {
        return new MappingOperationOptions<object, TTarget>(x => null);
    }
}