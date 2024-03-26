﻿namespace DotNetToolbox.Http;

public enum AuthenticationType {
    None,
    ApiKey,
    BasicToken,
    BearerToken,
    Password,
    Jwt,
    OAuth2,
    //Client,
    //ByCode,
    //Account,
    //Digest,
    //Windows,
}
