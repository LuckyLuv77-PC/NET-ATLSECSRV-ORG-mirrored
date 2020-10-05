// Copyright (c) Microsoft. All rights reserved.
//
// Licensed under the MIT license.

namespace NuGetCredentialProvider.CredentialProviders.Vsts
{
    public interface IAdalTokenProviderFactory
    {
        IAdalTokenProvider Get(string authority);
    }
}