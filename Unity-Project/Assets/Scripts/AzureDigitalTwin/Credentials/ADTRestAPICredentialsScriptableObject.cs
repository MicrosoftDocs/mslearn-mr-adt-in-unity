// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ADTRestAPICredentials", menuName = "Scriptable Objects/Credentials/ADT Rest API Credentials")]
public class ADTRestAPICredentialsScriptableObject : ScriptableObject
{
    /// <summary>
    /// URL of the Azure Digital Twins instance (https://[adt-instance-hostname])
    /// </summary>
    [Header("ADT Connection Info")]
    public string adtInstanceUrl;

    /// <summary>
    /// Client ID of the service principal used to connect to the ADT Rest API.
    /// The service principal must have the "Azure Digital Twins Data Owner" role assigned on the ADT instance.
    /// </summary>
    public string clientId;

    /// <summary>
    /// Client Secret of the service principal used to connect to the ADT Rest API.
    /// The service principal must have the "Azure Digital Twins Data Owner" role assigned on the ADT instance.
    /// </summary>
    public string clientSecret;

    /// <summary>
    /// Tenant ID of the service principal used to connect to the ADT Rest API.
    /// The service principal must have the "Azure Digital Twins Data Owner" role assigned on the ADT instance.
    /// </summary>
    public string tenantId;




}
