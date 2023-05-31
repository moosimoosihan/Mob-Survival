// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;

namespace olimsko
{
    public interface IConfigurationProvider
    {
        Configuration GetConfiguration(Type type);
    }
}
