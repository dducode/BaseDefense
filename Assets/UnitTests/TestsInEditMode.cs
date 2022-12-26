using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestsInEditMode
{
    [Test]
    public void CheckAddingListeners()
    {
        BroadcastMessages.AddAllListeners();
        Assert.Greater(BroadcastMessages.dict[MessageType.DEATH_PLAYER].listeners.Count, 0);
        Assert.Greater(BroadcastMessages.dict[MessageType.RESTART].listeners.Count, 0);
        BroadcastMessages.dict.Clear();
    }
}
