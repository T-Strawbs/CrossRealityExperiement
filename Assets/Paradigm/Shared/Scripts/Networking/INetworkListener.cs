using System;
using System.Collections.Generic;

public interface INetworkListener
{
    public void SubscribeToConnectionManager();
    public void SetupNetworkMessages();
}